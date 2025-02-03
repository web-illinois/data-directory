using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace uofi_itp_directory_data.Uploads {

    public class ImageScaler {
        private const double rescalePercentage = 1.25;

        public async Task<(Stream?, string)> Scale(Stream stream, int height, int width, string contentType) {
            if (height == 0 && width == 0) {
                return (stream, "Picture uploaded");
            }
            if (!StaticLookup.SupportedImageTypes.ContainsKey(contentType)) {
                return (null, "Image is not recognized");
            }
            var img = await Image.LoadAsync(stream);
            if ((img.Width * rescalePercentage) < width && (img.Height * rescalePercentage) < height) {
                return (null, $"Error: Picture does not match minimum dimensions {width} width and {height} height (actual file is {img.Width}x{img.Height})");
            } else if (img.Width == width && img.Height == height) {
                var memoryStreamWebPOriginal = new MemoryStream();
                await img.SaveAsWebpAsync(memoryStreamWebPOriginal);
                return (memoryStreamWebPOriginal, "Picture uploaded - make sure to save");
            }
            var originalHeight = img.Height;
            var originalWidth = img.Width;
            if ((float) img.Width / img.Height > (float) width / height) {
                img.Mutate(i => i.Resize(0, height));
                var startX = Math.Abs(img.Width - width) / 2;
                img.Mutate(i => i.Crop(new Rectangle(startX, 0, width, height)));
            } else {
                img.Mutate(i => i.Resize(width, 0));
                var startY = Math.Abs(img.Height - height) / 2;
                img.Mutate(i => i.Crop(new Rectangle(0, startY, width, height)));
            }
            var memoryStreamWebP = new MemoryStream();
            await img.SaveAsWebpAsync(memoryStreamWebP);
            return (memoryStreamWebP, $"Picture rescaled to {width} width and {height} height (actual file is {originalWidth}x{originalHeight}) - if this cropped correctly, make sure to save");
        }
    }
}