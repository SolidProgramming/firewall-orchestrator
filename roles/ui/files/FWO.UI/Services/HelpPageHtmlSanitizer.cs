using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace FWO.Ui.Services
{
    /// <summary>
    /// Sanitizes localized help-page fragments so trusted internal help links remain clickable
    /// while arbitrary markup from text sources is HTML-encoded.
    /// </summary>
    public static partial class HelpPageHtmlSanitizer
    {
        /// <summary>
        /// Encodes all markup except simple internal help links.
        /// </summary>
        /// <param name="html">Localized HTML fragment.</param>
        /// <returns>Sanitized HTML fragment that can safely be rendered with <c>Html.Raw</c>.</returns>
        public static string SanitizeLocalizedHtml(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            StringBuilder sanitized = new();
            int currentIndex = 0;

            foreach (Match match in AllowedHelpAnchorRegex().Matches(html))
            {
                sanitized.Append(HtmlEncoder.Default.Encode(html[currentIndex..match.Index]));

                string href = match.Groups["href"].Value;
                string linkText = HtmlEncoder.Default.Encode(WebUtility.HtmlDecode(match.Groups["text"].Value));

                sanitized.Append($"""<a href="{href}">{linkText}</a>""");
                currentIndex = match.Index + match.Length;
            }

            sanitized.Append(HtmlEncoder.Default.Encode(html[currentIndex..]));
            return sanitized.ToString();
        }

        [GeneratedRegex("""<a\s+href="(?<href>/help/[A-Za-z0-9_\-/?=&]+)"\s*>(?<text>.*?)</a>""",
            RegexOptions.IgnoreCase | RegexOptions.Singleline)]
        private static partial Regex AllowedHelpAnchorRegex();
    }
}
