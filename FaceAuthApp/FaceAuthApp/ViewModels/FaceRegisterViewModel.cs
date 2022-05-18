using System.Windows.Input;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Essentials;

using FaceAuthApp.Services;

namespace FaceAuthApp.ViewModels
{
    public class FaceRegisterViewModel : BasePhotoViewModel
    {
        public ICommand RegisterPersonCommand { get; private set; }
        public ICommand RegisterPhotosCommand { get; private set; }
        public ICommand TrainModelCommand { get; private set; }

        private string name;

        public string Name
        {
            get => name;
            set { SetProperty(ref name, value); }
        }

        private string id;

        public string Id
        {
            get => id;
            set { SetProperty(ref id, value); }
        }

        private FileResult photo2;

        public FileResult Photo2
        {
            get => photo2;
            set { SetProperty(ref photo2, value); OnPropertyChanged("PhotoStream2"); }
        }

        public ImageSource PhotoStream2
        {
            get
            {
                if (this.photo2 != null)
                {
                    var stream = ReadFile(photo2Path);
                    return ImageSource.FromStream(() => stream);
                }
                else
                    return null;
            }
        }

        private FileResult photo3;

        public FileResult Photo3
        {
            get => photo3;
            set { SetProperty(ref photo3, value); OnPropertyChanged("PhotoStream3"); }
        }

        private string photo2Path;
        private string photo3Path;

        public ImageSource PhotoStream3
        {
            get
            {
                if (this.photo3 != null)
                {
                    var stream = ReadFile(photo3Path);
                    return ImageSource.FromStream(() => stream);
                }
                else
                    return null;
            }
        }

        public FaceRegisterViewModel()
        {
            RegisterPersonCommand = new Command(async () => await RegisterPerson());
            RegisterPhotosCommand = new Command(async () => await RegisterPhotos());
            TrainModelCommand = new Command(async () => await TrainModel());
            TakePhotoCommand = new Command(async () => await TakePhoto());
        }

        private async Task TakePhoto()
        {
            var image = await ImageService.GetPhoto(ImageMode.Camera);

            if (Photo == null)
            {
                photo1Path = await CopyImage(image);
                Photo = image;
            }
            else
            {
                if (Photo2 == null)
                {
                    photo2Path = await CopyImage(image);
                    Photo2 = image;
                }
                else
                {
                    photo3Path = await CopyImage(image);
                    Photo3 = image;
                }
            }
        }

        private async Task RegisterPerson()
        {
            IsBusy = true;

            var guid = await ServicioFace.RegisterPerson(Name);
            Id = guid.ToString();

            IsBusy = false;

            await App.Current.MainPage.DisplayAlert("Success!", "Person Registered!", "OK");
        }

        private async Task RegisterPhotos()
        {
            if (Photo != null && Photo2 != null && Photo3 != null)
            {
                IsBusy = true;

                await ServicioFace.RegisterFace(Id, ReadFile(photo1Path));
                await ServicioFace.RegisterFace(Id, ReadFile(photo2Path));
                await ServicioFace.RegisterFace(Id, ReadFile(photo3Path));

                await App.Current.MainPage.DisplayAlert("Success!", "Photos registered!", "OK");

                IsBusy = false;
            }
        }

        private async Task TrainModel()
        {
            IsBusy = true;

            await ServicioFace.TrainGroup();
            await App.Current.MainPage.DisplayAlert("Success!", "Training model...", "OK");
            IsBusy = false;

        }
    }
}