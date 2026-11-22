namespace Nop.Plugin.Widgets.ProProductDocuments;

/// <summary>
/// Represents plugin constants
/// </summary>
public static class ProductDocumentsDefaults
{
    /// <summary>
    /// Gets a plugin system name
    /// </summary>
    public static string SystemName => "Widgets.ProProductDocuments";

    /// <summary>
    /// Gets a name of the public download route
    /// </summary>
    public static string DownloadRouteName => "Plugin.Widgets.ProProductDocuments.Download";

    /// <summary>
    /// Gets a maximum allowed file size (20 MB)
    /// </summary>
    public static long MaxFileSizeBytes => 20 * 1024 * 1024;

    /// <summary>
    /// Gets file extensions allowed for uploading
    /// </summary>
    public static string[] AllowedFileExtensions => new[]
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
        ".zip", ".dwg", ".dxf", ".step", ".stp", ".igs", ".iges",
        ".txt", ".csv", ".jpg", ".jpeg", ".png", ".webp"
    };
}
