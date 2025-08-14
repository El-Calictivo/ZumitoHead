using System;
using Payosky.Utilities.Colors;
using UnityEngine;

namespace Payosky.Utilities.Logging
{
    /// <summary>
    /// Contains extension methods for generating and formatting logging tags with optional colorized output, tailored for Unity logs.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Generates a logging tag for the specified object, type, or text, with optional formatting using brackets.
        /// </summary>
        /// <param name="logOwner">The object for which to generate the logging tag.</param>
        /// <param name="withBrackets">Indicates whether the generated logging tag should include brackets. Default is true.</param>
        /// <returns>A formatted logging tag as a string.</returns>
        public static string GetLoggingTag<T>(this T logOwner, bool withBrackets = true)
        {
            return GetLoggingTag(logOwner.GetType().Name, withBrackets);
        }

        /// <summary>
        /// Generates a logging tag based on the type name or specific text provided.
        /// Optionally includes enclosing brackets for better readability in logging outputs.
        /// </summary>
        /// <param name="type">
        /// The text or type name to be used as the logging tag.
        /// </param>
        /// <param name="withBrackets">
        /// Indicates whether the logging tag should be enclosed in brackets. Defaults to true.
        /// </param>
        /// <returns>
        /// A formatted logging tag string.
        /// </returns>
        public static string GetLoggingTag(this Type type, bool withBrackets = true)
        {
            return GetLoggingTag(type.Name, withBrackets);
        }

        /// <summary>
        /// Generates a formatted logging tag from the given string.
        /// </summary>
        /// <param name="text">The text to be used as the basis for the logging tag.</param>
        /// <param name="withBrackets">A boolean value indicating whether the logging tag should include brackets.</param>
        /// <returns>A formatted string containing the logging tag, styled with a color derived from the input text.</returns>
        public static string GetLoggingTag(this string text, bool withBrackets = true)
        {
            return FormattedTag(text, withBrackets);
        }

        /// <summary>
        /// Formats a given tag string with optional colorization and brackets for use in logging.
        /// </summary>
        /// <param name="tag">The string to be formatted as a logging tag.</param>
        /// <param name="withBrackets">A boolean indicating whether the tag should include brackets. Defaults to true.</param>
        /// <returns>A formatted string with color and optional brackets, suitable for logging.</returns>
        private static string FormattedTag(string tag, bool withBrackets = true)
        {
            var color = tag.StringToColor().EnsurePastelForDarkBg();
            var displayTag = withBrackets ? $"[{tag}]" : tag;
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}><b>{displayTag}</b></color>";
        }
    }
}