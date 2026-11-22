# Pro product documents plugin

Adds a **Downloads** block to the product details page. Store admins can upload any number of
documents (manuals, datasheets, PDF files, etc.) to a product and manage them right from the
product edit page — similar to a "Downloads" tab on many product pages.

## Requirements

* nopCommerce 4.90

## Installation

1. Build the solution (or just `Nop.Plugin.Widgets.ProProductDocuments.csproj`).
   The plugin is copied to `Presentation/Nop.Web/Plugins/Widgets.ProProductDocuments`.
2. In the admin area go to **Configuration → Local plugins**, find **Pro product documents** and click **Install**.
3. The plugin is activated automatically (it renders itself in fixed widget zones and is not managed
   on the **Widgets** page).

## Usage

1. Go to **Catalog → Products** and edit a product.
2. Open the **Pro product documents** card.
3. Choose a file (optionally enter a title) and click **Upload document**.
4. Edit the title, display order or "Is active" flag of an existing document and click the save icon,
   or delete it with the trash icon.
5. The product details page displays a **Downloads** block with all active documents:
   clicking the file name downloads the file; the **View** button opens previewable files (PDF, images, text)
   inline in a new browser tab. The View button is hidden for non-previewable types (docx, zip, CAD, ...).

Documents are served from `~/productdocuments/download/{id}` (inline for previewable types) and
`~/productdocuments/download/{id}?download=true` (forced download) without authentication.

## Configuration

* Maximum file size: 20 MB (`ProductDocumentsDefaults.MaxFileSizeBytes`).
* Allowed extensions: `ProductDocumentsDefaults.AllowedFileExtensions`
  (PDF, Office documents, archives, CAD, text and image files).

Both values are compiled into the plugin; change them and rebuild if needed.

## Notes

* Files are stored in the core `Download` table (`DownloadBinary`), the same way nopCommerce stores
  downloadable product files.
* Documents are public. Do not upload confidential files unless you also restrict the
  `ProductDocumentController.Download` action (for example to authenticated or purchasing customers).
* Localization resources are prefixed with `Plugins.Widgets.ProProductDocuments`
  (English defaults plus Chinese values when a Chinese language exists on install).
