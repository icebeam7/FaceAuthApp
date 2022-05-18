using System.Threading.Tasks;
using Xamarin.Essentials;

namespace FaceAuthApp.Services
{
    public enum ImageMode
    {
        Camera,
        Gallery
    }

    public class ImageService
    {
        public static async Task<FileResult> GetPhoto(ImageMode mode)
        {
            switch (mode)
            {
                case ImageMode.Camera:
                    if (MediaPicker.IsCaptureSupported)
                    {
                        var photo = await MediaPicker.CapturePhotoAsync();
                        return photo;
                    }
                    break;

                case ImageMode.Gallery:
                    var picture = await MediaPicker.PickPhotoAsync();
                    return picture;
                default:
                    break;
            }

            return null;
        }
    }
}