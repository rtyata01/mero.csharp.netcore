namespace NetCore.WorkItemService.Handler.WorkItems
{
    /// <summary>
    /// EncodingHelper.
    /// </summary>
    public static class EncodingHelper
    {
        /// <summary>
        /// Encodes a given string to ASCII.
        /// </summary>
        /// <param name="unicodeString">Unicode encoded string.</param>
        /// <param name="logger">ILogger.</param>
        /// <returns>ASCII encoded string.</returns>
        public static string EncodeUnicodeToASCII(string unicodeString, ILogger logger)
        {
            string asciiString = string.Empty;

            if (!string.IsNullOrWhiteSpace(unicodeString))
            {
                logger.LogInformation("Convert string from Unicode to ASCII");

                // https://docs.NetCore.com/en-us/dotnet/api/system.string.trim?view=netframework-4.6.1
                // the Trim() method in the .NET Framework 3.5 SP1 and earlier versions removes two characters,
                // ZERO WIDTH SPACE (U+200B) and ZERO WIDTH NO-BREAK SPACE (U+FEFF),
                // that the Trim() method in the .NET Framework 4 and later versions does not remove
                unicodeString = unicodeString.Replace("\u200B", string.Empty, StringComparison.Ordinal);
                unicodeString = unicodeString.Replace("\uFEFF", string.Empty, StringComparison.Ordinal);

                // Create two different encodings.
                Encoding ascii = Encoding.ASCII;
                Encoding unicode = Encoding.Unicode;

                // Convert the string into a byte array.
                byte[] unicodeBytes = unicode.GetBytes(unicodeString);

                // Perform the conversion from one encoding to the other.
                byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

                // Convert the new byte[] into a char[] and then into a string.
                char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
                ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
                asciiString = new string(asciiChars);

                logger.LogInformation("Successfully converted string from Unicode to ASCII");
            }
            return asciiString;
        }
    }
}
