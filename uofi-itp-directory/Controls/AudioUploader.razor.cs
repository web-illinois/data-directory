using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using uofi_itp_directory_data.Uploads;

namespace uofi_itp_directory.Controls {

    public partial class AudioUploader {
        private const string _tempName = "-temp";

        private readonly int _maxAllowedSize = 5000;

        private string _originalAudioUrl = "";

        public string AudioResultsText { get; set; } = "";

        [Parameter]
        public EventCallback Delete { get; set; } = default!;

        public string Filename { get; set; } = "";

        [Parameter]
        public string FileUrl { get; set; } = "";

        [Parameter]
        public bool IsDisabled { get; set; } = false;

        [Parameter]
        public string ItemId { get; set; } = "";

        [Parameter]
        public EventCallback Save { get; set; } = default!;

        public string UploaderId => "uploadaudio";

        public UploaderStatusEnum UploaderStatus { get; set; } = UploaderStatusEnum.Untouched;

        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        protected UploadStorage UploadStorage { get; set; } = default!;

        public async Task<bool> DeleteFile() {
            if (string.IsNullOrEmpty(_originalAudioUrl)) {
                _originalAudioUrl = FileUrl;
            }
            await Delete.InvokeAsync();
            AudioResultsText = "Recording deleted, make sure to save";
            UploaderStatus = UploaderStatusEnum.Deleted;
            return true;
        }

        public async Task RemoveMessage() => _ = await JsRuntime.InvokeAsync<bool>("removeAlertOnScreen");

        private static string GetContentType(IBrowserFile file) => Path.GetExtension(file.Name).ToLowerInvariant() switch {
            ".mp3" => "audio/mpeg",
            ".m4a" => "audio/mp4",
            ".wav" => "audio/wav",
            ".ogg" => "audio/ogg",
            ".webm" => "audio/webm",
            _ => file.ContentType
        };

        public async Task<bool> SaveFile() {
            if (UploaderStatus == UploaderStatusEnum.Uploaded) {
                Filename = await UploadStorage.Move(Filename.Replace(_tempName, ""), FileUrl, false);
                FileUrl = UploadStorage.GetFullPath(Filename, false);
                AudioResultsText = "Recording saved";
                return true;
            } else if (UploaderStatus == UploaderStatusEnum.Deleted) {
                _ = await UploadStorage.Delete(_originalAudioUrl, false);
                AudioResultsText = "Recording deleted";
                return true;
            }
            return false;
        }

        public async Task<bool> SaveFileToTemp(InputFileChangeEventArgs e) {
            if (string.IsNullOrEmpty(_originalAudioUrl)) {
                _originalAudioUrl = FileUrl;
            }
            if (!e.File.ContentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase)) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "File must be an audio file");
                return false;
            }
            if (e.File.Size > 1024 * _maxAllowedSize) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", $"File is too large -- size of file is {float.Round(e.File.Size / (float) (1024 * 1000), 2)}MB and maximum size is {_maxAllowedSize / 1000}MB");
                return false;
            }
            await Save.InvokeAsync();
            Filename = await UploadStorage.Upload(ItemId + _tempName + Path.GetExtension(e.File.Name), GetContentType(e.File), e.File.OpenReadStream(maxAllowedSize: 1024 * _maxAllowedSize), false);
            if (string.IsNullOrEmpty(Filename)) {
                _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Upload failed - check that the audio container is configured");
                return false;
            }
            FileUrl = UploadStorage.GetFullPath(Filename, false);
            _ = await JsRuntime.InvokeAsync<bool>("alertOnScreen", "Recording uploaded - make sure to save");
            AudioResultsText = "New recording, make sure to save";
            UploaderStatus = UploaderStatusEnum.Uploaded;
            return !string.IsNullOrEmpty(FileUrl);
        }
    }
}
