using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nop.Core;

namespace Nop.Plugin.Widgets.ProProductDocuments.Infrastructure;

/// <summary>
/// Represents a helper to serve product document files
/// </summary>
public static class ProductDocumentFileHelper
{
    #region Utilities

    /// <summary>
    /// Get a lower-cased file extension (with the leading dot)
    /// </summary>
    /// <param name="extension">Stored file extension</param>
    /// <param name="fileName">Original file name</param>
    /// <returns>File extension</returns>
    public static string GetFileExtension(string extension, string fileName)
    {
        if (!string.IsNullOrEmpty(extension))
            return extension.ToLowerInvariant();

        return string.IsNullOrEmpty(fileName) ? string.Empty : Path.GetExtension(fileName).ToLowerInvariant();
    }

    /// <summary>
    /// Get a value indicating whether the file type can be displayed inline by a browser
    /// </summary>
    /// <param name="extension">File extension</param>
    /// <returns>True if the file type can be displayed inline; otherwise false</returns>
    public static bool CanDisplayInline(string extension)
    {
        return extension is ".pdf" or ".txt" or ".jpg" or ".jpeg" or ".png" or ".webp";
    }

    /// <summary>
    /// Resolve a content type by file extension
    /// </summary>
    /// <param name="extension">File extension</param>
    /// <param name="storedContentType">Content type stored when the file was uploaded</param>
    /// <returns>Content type</returns>
    public static string GetContentType(string extension, string storedContentType)
    {
        return extension switch
        {
            ".pdf" => MimeTypes.ApplicationPdf,
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => !string.IsNullOrWhiteSpace(storedContentType)
                ? storedContentType
                : MimeTypes.ApplicationOctetStream
        };
    }

    /// <summary>
    /// Set the Content-Disposition header
    /// </summary>
    /// <param name="response">HTTP response</param>
    /// <param name="inline">Whether the file should be displayed inline (true) or downloaded (false)</param>
    /// <param name="fileName">File name</param>
    public static void SetContentDisposition(HttpResponse response, bool inline, string fileName)
    {
        var contentDisposition = new ContentDispositionHeaderValue(inline ? "inline" : "attachment");
        contentDisposition.SetHttpFileName(fileName);
        response.Headers.ContentDisposition = contentDisposition.ToString();
    }

    #endregion
}
