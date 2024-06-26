﻿using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace App
{
    public static class Helpers
    {
        public static string SanitizeHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(input);

            // Remove all script and style nodes
            document.DocumentNode.Descendants()
                .Where(n => n.Name == "script" || n.Name == "style")
                .ToList()
                .ForEach(n => n.Remove());

            // Remove all attributes that can contain harmful content
            foreach (var node in document.DocumentNode.Descendants())
            {
                node.Attributes
                    .Where(a => a.Name.StartsWith("on", StringComparison.OrdinalIgnoreCase) ||
                                a.Name.Equals("style", StringComparison.OrdinalIgnoreCase) ||
                                a.Name.Equals("href", StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .ForEach(a => a.Remove());
            }

            return SanitizeSimpleString(document.DocumentNode.InnerHtml);
        }

        private static string SanitizeSimpleString(string input)
        {
            // Remove potentially harmful characters
            return Regex.Replace(input, @"[<>""'%;)(&+]", string.Empty);
        }
    }
}