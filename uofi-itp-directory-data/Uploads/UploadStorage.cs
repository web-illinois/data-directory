using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace uofi_itp_directory_data.Uploads {

    public class UploadStorage {
        private readonly string _azureAccountKey = "";
        private readonly string _azureAccountName = "";
        private readonly string _azureClientUrl = "";
        private readonly string _azureCvContainerName = "";
        private readonly string _azureImageContainerName = "";

        public UploadStorage() {
        }

        public UploadStorage(string? azureClientUrl, string? azureAccountName, string? azureAccountKey, string? azureImageContainerName, string? azureCvContainerName) {
            _azureClientUrl = azureClientUrl ?? "";
            _azureAccountName = azureAccountName ?? "";
            _azureAccountKey = azureAccountKey ?? "";
            _azureImageContainerName = azureImageContainerName ?? "";
            _azureCvContainerName = azureCvContainerName ?? "";
        }

        public async Task<bool> Delete(string url, bool isImage) {
            var filename = url.Split('/').Last();
            var blobServiceClient = GetServiceClient();
            var containerClient = blobServiceClient.GetBlobContainerClient(GetContainer(isImage));
            var blobClient = containerClient.GetBlobClient(filename);
            return await blobClient.DeleteIfExistsAsync();
        }

        public string GetFullPath(string filename, bool isImage) => $"{_azureClientUrl}/{GetContainer(isImage)}/{filename}";

        public async Task<string> Move(string newFilename, string oldUrl, bool isImage) {
            var blobServiceClient = GetServiceClient();
            var containerClient = blobServiceClient.GetBlobContainerClient(GetContainer(isImage));
            var destBlobClient = containerClient.GetBlobClient(newFilename);
            _ = await destBlobClient.StartCopyFromUriAsync(new Uri(oldUrl));
            return newFilename;
        }

        public async Task<string> Upload(string name, string contentType, Stream stream, bool isImage) {
            try {
                var filename = isImage ? $"{name}{StaticLookup.SupportedImageTypes[contentType]}" :
                    name;
                var blobServiceClient = GetServiceClient();
                var containerClient = blobServiceClient.GetBlobContainerClient(GetContainer(isImage));
                var blobClient = containerClient.GetBlobClient(filename);
                _ = await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } });
                return filename;
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
                return "";
            }
        }

        private string GetContainer(bool isImage) => isImage ? _azureImageContainerName : _azureCvContainerName;

        private BlobServiceClient GetServiceClient() => new BlobServiceClient(new Uri(_azureClientUrl), new StorageSharedKeyCredential(_azureAccountName, _azureAccountKey));
    }
}