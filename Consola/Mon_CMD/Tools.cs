using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mon.Tools
{
    static class Strings
    {
        /// <summary>
        /// Replaces all invalid path characters in a string with spaces and removes extra spaces.
        /// </summary>
        /// <param name="path">The original path string to be sanitized.</param>
        /// <returns>A sanitized string with all invalid path characters replaced by spaces and multiple spaces reduced to a single space.</returns>
        /// <remarks>
        /// This method replaces the following characters with a space:
        /// \ / : * ? " < > | ' . , ! @ # $ % ^ & ( ) _ + = [ ] { } ; : ' , < > / ? ` ~ -
        /// After replacing these characters, it uses a regular expression to replace multiple spaces with a single space.
        /// Finally, it trims leading and trailing spaces from the resulting string.
        /// </remarks>
        public static string StringToCorrectPathName(string path)
        {
            return System.Text.RegularExpressions.Regex.Replace(path.Replace("\\", " ")
                                .Replace("/", " ")
                                .Replace(":", " ")
                                .Replace("*", " ")
                                .Replace("?", " ")
                                .Replace("\"", " ")
                                .Replace("<", " ")
                                .Replace(">", " ")
                                .Replace("|", " ")
                                .Replace("'", " ")
                                .Replace(".", " ")
                                .Replace(",", " ")
                                .Replace("!", " ")
                                .Replace("@", " ")
                                .Replace("#", " ")
                                .Replace("$", " ")
                                .Replace("%", " ")
                                .Replace("^", " ")
                                .Replace("&", " ")
                                .Replace("(", " ")
                                .Replace(")", " ")
                                .Replace("_", " ")
                                .Replace("+", " ")
                                .Replace("=", " ")
                                .Replace("[", " ")
                                .Replace("]", " ")
                                .Replace("{", " ")
                                .Replace("}", " ")
                                .Replace(";", " ")
                                .Replace(":", " ")
                                .Replace("'", " ")
                                .Replace(",", " ")
                                .Replace("<", " ")
                                .Replace(">", " ")
                                .Replace("/", " ")
                                .Replace("?", " ")
                                .Replace("`", " ")
                                .Replace("~", " ")
                                .Replace("-", " "), @"\s+", " ").Trim();
        }

        /// <summary>
        /// Converts a file path into a string containing only the folders in the path.
        /// </summary>
        /// <param name="path">The original file path to be processed.</param>
        /// <param name="pathSymbol">The symbol used to separate folders in the path. Defaults to "/".</param>
        /// <returns>A string containing the folders in the path, separated by '/'.</returns>
        /// <remarks>
        /// This method splits the provided file path into its folder components based on the specified pathSymbol.
        /// It removes the last component (assumed to be the file name) and then concatenates the remaining folder names.
        /// Each folder name is followed by a '/' except for the last folder.
        /// </remarks>
        public static string FilePathToFolders(string path , string pathSymbol = "/")
        {
            var folders = path.Split(pathSymbol).ToList();
            folders.Remove(folders.Last());

            string result = "";

            for (int i = 0; i < folders.Count; i++) result += i < folders.Count - 1 ? folders[i] + '/' : folders[i];

            return result;
        }

        /// <summary>
        /// Extracts the file name from a given file path.
        /// </summary>
        /// <param name="path">The file path from which to extract the file name.</param>
        /// <returns>The file name from the provided file path.</returns>
        /// <remarks>
        /// This method splits the provided file path into its components based on the '/' symbol
        /// and returns the last component, which is assumed to be the file name.
        /// </remarks>
        public static string FilePathToFileName(string path) => path.Split('/').ToList().Last();
    }
}
