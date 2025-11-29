using System;
using System.Text.RegularExpressions;
using Ganss.Xss;

namespace PoemApp.Infrastructure.Services;

public static class HtmlSanitizerHelper
{
    private static readonly HtmlSanitizer _sanitizer = CreateSanitizer();

    private static HtmlSanitizer CreateSanitizer()
    {
        var sanitizer = new HtmlSanitizer();

        // Start from a conservative baseline
        sanitizer.AllowedTags.Clear();

        // Common inline text formatting
        sanitizer.AllowedTags.Add("b");
        sanitizer.AllowedTags.Add("i");
        sanitizer.AllowedTags.Add("u");
        sanitizer.AllowedTags.Add("strong");
        sanitizer.AllowedTags.Add("em");
        sanitizer.AllowedTags.Add("span");
        sanitizer.AllowedTags.Add("div");

        // Structural / paragraph
        sanitizer.AllowedTags.Add("p");
        sanitizer.AllowedTags.Add("br");
        sanitizer.AllowedTags.Add("h1");
        sanitizer.AllowedTags.Add("h2");
        sanitizer.AllowedTags.Add("h3");
        sanitizer.AllowedTags.Add("h4");
        sanitizer.AllowedTags.Add("h5");
        sanitizer.AllowedTags.Add("h6");

        // Lists
        sanitizer.AllowedTags.Add("ul");
        sanitizer.AllowedTags.Add("ol");
        sanitizer.AllowedTags.Add("li");

        // Links and images
        sanitizer.AllowedTags.Add("a");
        sanitizer.AllowedTags.Add("img");

        // Code / preformatted
        sanitizer.AllowedTags.Add("pre");
        sanitizer.AllowedTags.Add("code");
        sanitizer.AllowedTags.Add("blockquote");

        // Tables (optional)
        sanitizer.AllowedTags.Add("table");
        sanitizer.AllowedTags.Add("thead");
        sanitizer.AllowedTags.Add("tbody");
        sanitizer.AllowedTags.Add("tr");
        sanitizer.AllowedTags.Add("th");
        sanitizer.AllowedTags.Add("td");

        // Allowed attributes
        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedAttributes.Add("href");
        sanitizer.AllowedAttributes.Add("src");
        sanitizer.AllowedAttributes.Add("alt");
        sanitizer.AllowedAttributes.Add("title");
        sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedAttributes.Add("width");
        sanitizer.AllowedAttributes.Add("height");

        // Do not allow arbitrary data- attributes by default
        sanitizer.AllowDataAttributes = false;

        // Allowed URI schemes for href/src
        sanitizer.AllowedSchemes.Clear();
        sanitizer.AllowedSchemes.Add("http");
        sanitizer.AllowedSchemes.Add("https");
        sanitizer.AllowedSchemes.Add("mailto");
        // Allow data: URIs for images if needed
        sanitizer.AllowedSchemes.Add("data");

        // Optional: further CSS sanitization can be configured here if allowing style attributes
        // For now we do not allow the style attribute to avoid CSS-based XSS vectors.

        return sanitizer;
    }

    public static string SanitizeHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        // Use HtmlSanitizer to sanitize and return result
        return _sanitizer.Sanitize(html);
    }
}
