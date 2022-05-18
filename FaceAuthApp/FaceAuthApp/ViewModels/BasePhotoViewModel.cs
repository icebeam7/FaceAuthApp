using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Essentials;

namespace FaceAuthApp.ViewModels
{
    public class BasePhotoViewModel : BaseViewModel
    {
        public ICommand TakePhotoCommand { get; set; }

        private FileResult photo;

        public FileResult Photo
        {
            get => photo;
            set { photo = value; OnPropertyChanged(); OnPropertyChanged("PhotoStream"); }
        }

        protected string photo1Path;

        public ImageSource PhotoStream
        {
            get
            {
                if (this.photo != null)
                {
                    var stream = ReadFile(photo1Path);
                    return ImageSource.FromStream(() => stream);
                }
                else
                    return null;
            }
        }

        protected Stream ReadFile(string path)
        {
            var stream = new FileStream(path, FileMode.Open);
            return stream;
        }

        protected async Task<string> CopyImage(FileResult photo)
        {
            var copy = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

            using (var stream = await photo.OpenReadAsync())
            {
                using (var newStream = File.OpenWrite(copy))
                {
                    await stream.CopyToAsync(newStream);
                }
            }

            return copy;
        }

        public BasePhotoViewModel()
        {
        }
    }
}