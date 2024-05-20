namespace NetCore.WorkItemService.Handler.WorkItems
{
    using NetCore.WorkItemService.Dto.External;
    using NetCore.WorkItemService.Dto.Internal.OnPrem;
    using NetCore.WorkItemService.Handler.Clients.OnPrem.Extensions;

    /// <summary>
    /// WorkItemPayloadHandler.
    /// </summary>
    public class WorkItemPayloadHandler : IWorkItemPayloadHandler
    {
        private static readonly char[] Separators = new char[] { ',', ';' };
        private readonly ILogger<WorkItemPayloadHandler> logger;

        private static readonly HashSet<string> BugBinaryTypes = new HashSet<string>
        {
            "Mobile Only",
            "Analog Only",
            "One Core Only",
        };

        private static readonly IreadonlyDictionary<string, char> StringToInvalidChar = new Dictionary<string, char>
        {
            {"#quote#", '"'},
            {"#lessThan#", '<'},
            {"#greaterThan#", '>'},
            {"#orsymbol#", '|'},
            {"#nullchar#", '\0'},
            {"#colon#", ':'},
            {"#star#", '*'},
            {"#question#", '?'}
        };

        private static readonly IreadonlyDictionary<char, string> InvalidCharToString = StringToInvalidChar.ToDictionary(x => x.Value, x => x.Key);

        /// <summary>
        /// WorkItemPayloadHandler.
        /// </summary>
        /// <param name="logger">ILogger.</param>
        public WorkItemPayloadHandler(ILogger<WorkItemPayloadHandler> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public IEnumerable<WorkItemPayload> GetOnPremBugPayloads(IEnumerable<OnPremBugPayload> OnPremBugPayloads)
        {
            ConcurrentBag<WorkItemPayload> result = new ConcurrentBag<WorkItemPayload>();

            Parallel.ForEach(OnPremBugPayloads, bug =>
            {
                var payload = this.GetOnPremBugPayload(bug);
                result.Add(payload);
            });

            return result;
        }

        /// <inheritdoc/>
        public WorkItemPayload GetOnPremBugPayload(OnPremBugPayload OnPremBugPayload)
        {
            return this.GetWorkItemPayload(OnPremBugPayload.Id, OnPremBugPayload.BinaryFiles, OnPremBugPayload.ReproSteps, OnPremBugPayload.Release);
        }

        /// <inheritdoc/>
        public WorkItemPayload GetOnPremWorkItemPayload(OnPremWorkItem OnPremWorkItem)
        {
            return this.GetWorkItemPayload(OnPremWorkItem.Id, OnPremWorkItem.BinaryFiles, OnPremWorkItem.ReproSteps, OnPremWorkItem.Release);
        }

        #region Mostly extracted from old MetadataProvider Code base.

        private WorkItemPayload GetWorkItemPayload(int workItemId, string binaryFiles, string binaryFilesFromReproSteps, string release)
        {
            WorkItemPayload payload = new WorkItemPayload
            {
                Id = workItemId,
                Release = release,
                Binaries = Enumerable.Empty<WorkItemBinary>(),
                Components = Enumerable.Empty<WorkItemComponent>(),
                PayloadItems = Enumerable.Empty<string>(),
            };

            if (string.IsNullOrWhiteSpace(binaryFiles))
            {
                return payload;
            }

            if (this.IsMobileOnly(binaryFiles))
            {
                return payload;
            }

            try
            {
                string formattedBinary = binaryFiles.ToLowerInvariant().Trim().Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
                if (ShouldIgnorePayload(formattedBinary))
                {
                    return payload;
                }

                string binaries;
                if (formattedBinary.Contains(OnPremConstants.SeeReproSteps, StringComparison.OrdinalIgnoreCase))
                {
                    binaries = EncodingHelper.EncodeUnicodeToASCII(this.SanitizeReproSteps(binaryFilesFromReproSteps), this.logger);
                }
                else
                {
                    binaries = EncodingHelper.EncodeUnicodeToASCII(binaryFiles, this.logger);
                }

                var payloadItems = this.SplitPayloadEntries(binaries).ToList();
                payload.PayloadItems = payloadItems;
                (payload.Binaries, payload.Components) = this.ParseIntoBinariesAndComponents(payloadItems, workItemId);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning("Error parsing binaries from WorkItem ID: {WorkItemId}: {Exception}", workItemId, ex);
            }

            return payload;
        }

        private static bool ShouldIgnorePayload(string formattedBinary)
        {
            return string.Equals("N/A", formattedBinary, StringComparison.OrdinalIgnoreCase)
                || string.Equals("NA", formattedBinary, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsMobileOnly(string binaryField)
        {
            binaryField = binaryField.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();

            if (string.Compare(OnPremConstants.MobileOnlyWithSpace, binaryField, StringComparison.OrdinalIgnoreCase) == 0 ||
                string.Compare(OnPremConstants.MobileOnlyWithoutSpace, binaryField, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }

        private string SanitizeReproSteps(string reproSteps)
        {
            if (string.IsNullOrEmpty(reproSteps) || string.IsNullOrWhiteSpace(reproSteps))
            {
                return string.Empty;
            }

            reproSteps = Regex.Replace(reproSteps, "<br>", ","); // replace line breaks with separators
            reproSteps = Regex.Replace(reproSteps, "<br/>", ","); // replace line breaks with separators
            reproSteps = Regex.Replace(reproSteps, "<p>", ","); // replace paragraph tags with separators
            reproSteps = Regex.Replace(reproSteps, "</p>", ","); // replace paragraph tags with separators
            reproSteps = reproSteps.Replace("<span>/</span>", "/", StringComparison.OrdinalIgnoreCase);   // replace <span>/</span> which could be inside closing binary tag, in preparation for next step
            reproSteps = Regex.Replace(reproSteps, "<(?!/?binary).*?>", string.Empty); // replace all tags other than <binary> or </binary> tags
            reproSteps = WebUtility.HtmlDecode(reproSteps);

            var firstIndexBinaryTag = reproSteps.IndexOf("<binary>", StringComparison.OrdinalIgnoreCase);
            if (firstIndexBinaryTag > -1)
            {
                reproSteps = reproSteps.Substring(firstIndexBinaryTag + 8);
                var endIndexBinaryTag = reproSteps.IndexOf("<binary>", StringComparison.OrdinalIgnoreCase);

                if (endIndexBinaryTag == -1)
                {
                    endIndexBinaryTag = reproSteps.IndexOf("</binary>", StringComparison.OrdinalIgnoreCase);
                }

                if (endIndexBinaryTag == -1)
                {
                    return string.Empty;
                }

                var binaryString = reproSteps.Substring(0, endIndexBinaryTag).Trim();
                binaryString = Regex.Replace(binaryString, @"\r\n?|\n", ",");
                return binaryString;
            }

            firstIndexBinaryTag = reproSteps.IndexOf("<binaries>", StringComparison.OrdinalIgnoreCase);
            if (firstIndexBinaryTag == -1)
            {
                return string.Empty;
            }

            reproSteps = reproSteps.Substring(firstIndexBinaryTag + 10);
            var endIndexBinaryTag1 = reproSteps.IndexOf("<binaries>", StringComparison.OrdinalIgnoreCase);

            if (endIndexBinaryTag1 == -1)
            {
                endIndexBinaryTag1 = reproSteps.IndexOf("</binaries>", StringComparison.OrdinalIgnoreCase);
            }

            if (endIndexBinaryTag1 == -1)
            {
                return string.Empty;
            }

            var binaryString1 = reproSteps.Substring(0, endIndexBinaryTag1).Trim();
            binaryString1 = Regex.Replace(binaryString1, @"\r\n?|\n", ",");
            return binaryString1;
        }

        private IEnumerable<string> SplitPayloadEntries(string binaryField)
        {
            if (string.IsNullOrWhiteSpace(binaryField))
            {
                return Enumerable.Empty<string>();
            }

            return binaryField
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(s => !this.IsPathWithoutBinaryName(s))
                .ToList();
        }

        private bool IsPathWithoutBinaryName(string s)
        {
            if (string.IsNullOrEmpty(Path.GetDirectoryName(s)))
            {
                return false;
            }
            if (string.IsNullOrEmpty(Path.GetFileName(s)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Read [BinaryFileName] field which will contains multiple filenames
        ///     separated by ',' or ';' or ' '/space or multiple such delimiter characters.
        ///     -Id: identity
        ///     -BugId: bug id
        ///     -BinaryFilename:
        ///     -Remove path if any and get only the file name
        ///     -Remove parenthesis if any
        ///     -a.dll => a.dll
        ///     -P1/p2/a.dll => a.dll
        ///     -a.dll (MobileOnly) => a.dll
        ///     -BinaryType
        ///     -If binary has parentheses, then read the value within parentheses
        ///     -Record the value from within the parentheses
        ///     -a.dll (MobileOnly) => Mobile Only
        ///     -a.dll (IoT Only) => IoT Only
        ///     -P1/p2/a.dll (IOTOnly) => IoT Only
        ///     -a.dll => Desktop (if present in the [BlueprintBinary] or [BlueprintComponent])
        ///     -a.dll => Unknown (otherwise).
        /// </summary>
        /// <param name="payloadItems">Payload items mentioned in workitem.</param>
        /// <param name="workItemId">WorkItem Id.</param>
        /// <returns>Collection of WokItemBinary.</returns>
        private Tuple<List<WorkItemBinary>, List<WorkItemComponent>> ParseIntoBinariesAndComponents(IEnumerable<string> payloadItems, int workItemId)
        {
            var binaryMappings = new List<WorkItemBinary>();
            var componentMappings = new List<WorkItemComponent>();

            foreach (var payloadItem in payloadItems)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(payloadItem))
                    {
                        continue;
                    }

                    var binaryMapping = new WorkItemBinary();
                    var binaryFullName = payloadItem.Trim();

                    if (binaryFullName.Length >= 248)
                    {
                        continue;
                    }

                    var inValidPath = false;
                    var match = Regex.Match(binaryFullName, @"\(([^)]*)\)");
                    if (match.Success)
                    {
                        var parenthesisText = match.Groups[1].Value;
                        if (!string.IsNullOrEmpty(parenthesisText))
                        {
                            inValidPath = Path.GetDirectoryName(parenthesisText).Length != 0;
                            bool notDesktop = false;
                            foreach (var type in BugBinaryTypes)
                            {
                                notDesktop = parenthesisText.IndexOf(type, StringComparison.OrdinalIgnoreCase) >= 0
                                    || parenthesisText.IndexOf(type.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase), StringComparison.OrdinalIgnoreCase) >= 0;

                                if (notDesktop)
                                {
                                    binaryFullName = binaryFullName.Replace(match.Groups[0].Value, string.Empty, StringComparison.OrdinalIgnoreCase);
                                    break;
                                }
                            }
                        }
                    }

                    binaryFullName = InvalidCharToString.Keys.Aggregate(binaryFullName,
                        (current, key) => current.Replace(key.ToString(), InvalidCharToString[key], StringComparison.OrdinalIgnoreCase));

                    if (string.IsNullOrEmpty(binaryFullName) || binaryFullName.Length >= 248)
                    {
                        continue;
                    }

                    var invalidPathChars = Path.GetInvalidPathChars().Any(c => binaryFullName.Contains(c, StringComparison.OrdinalIgnoreCase));
                    var path = inValidPath || invalidPathChars ? string.Empty : Path.GetDirectoryName(binaryFullName);

                    string binaryName;
                    try
                    {
                        binaryName = inValidPath ? binaryFullName : Path.GetFileName(binaryFullName);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogWarning("Exception trying to get the filename from [{BinaryFullName}]. {Exception}", binaryFullName, ex);
                        binaryName = binaryFullName.Replace(string.IsNullOrEmpty(path) ? " " : path, string.Empty, StringComparison.OrdinalIgnoreCase);
                    }

                    if (!string.IsNullOrEmpty(path))
                    {
                        path = StringToInvalidChar.Keys.Aggregate(path,
                            (current, key) => current.Replace(key, StringToInvalidChar[key].ToString(), StringComparison.OrdinalIgnoreCase));
                    }
                    binaryName = StringToInvalidChar.Keys.Aggregate(binaryName,
                        (current, key) => current.Replace(key, StringToInvalidChar[key].ToString(), StringComparison.OrdinalIgnoreCase));

                    /* check for abnormal binary name containing kb and package version
                         some binary names are in this format - example "ksc.nlp:4344152:1.000"
                         binary name can be alphanumeric and contain any number of "." characters
                         so use [\w.]* instead of a-z, \d+ is for the kb match
                         \d{1}\.{1}\d{3} is for the package version match.
                    */
                    match = Regex.Match(binaryName, @"([\w\.]*):{1}(\d+):{1}(\d{1}\.{1}\d{3})");
                    if (match.Success)
                    {
                        // only store the binary name, don't store kb and package version
                        binaryName = match.Groups[1].Value;
                    }

                    path = path == @"\" || path == "." ? @".\" : path;
                    path = !string.IsNullOrEmpty(path) && !path.EndsWith(@"\", StringComparison.OrdinalIgnoreCase) ? path.Trim() + @"\" : path;

                    if (Path.GetInvalidPathChars().Any(c => path.Contains(c, StringComparison.OrdinalIgnoreCase)))
                    {
                        this.logger.LogWarning("{WorkItemId} Path [{Path}] contains invalid characters ", workItemId, path);
                        path = string.Empty;
                    }

                    if (Path.GetInvalidFileNameChars().Any(c => binaryName.Contains(c, StringComparison.OrdinalIgnoreCase)))
                    {
                        this.logger.LogWarning("{WorkItemId} BinaryName [{BinaryName}] contains invalid characters ", workItemId, binaryName);
                        binaryName = string.Empty;
                    }

                    binaryMapping.Name = binaryName.Trim();
                    binaryMapping.Path = path;

                    if (!string.IsNullOrWhiteSpace(binaryMapping.Name))
                    {
                        string extension = Path.GetExtension(binaryMapping.Name);
                        bool isComponent = string.IsNullOrWhiteSpace(binaryMapping.Path) && (string.IsNullOrWhiteSpace(extension) || extension.Equals(".Resources", StringComparison.OrdinalIgnoreCase));

                        // TODO: This is the best case logic here and so it will not always be correct.
                        // For e.g. "ipmidrv.inf" is the component name, but current logic will treat it as binary.
                        // For accurate binary/component identification, the name should be compared with serviceability data.
                        if (isComponent)
                        {
                            componentMappings.Add(new WorkItemComponent() { Name = binaryMapping.Name });
                        }
                        else
                        {
                            binaryMappings.Add(binaryMapping);
                        }
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogWarning("Exception trying to parse vso signal from bug: {WorkItemId} payload: {PayloadItems}. Exception: {Exception}", workItemId, string.Join(", ", payloadItems), e);
                }
            }
            return new Tuple<List<WorkItemBinary>, List<WorkItemComponent>>(binaryMappings, componentMappings);
        }

        #endregion Mostly extracted from old MetadataProvider Code base.
    }
}
