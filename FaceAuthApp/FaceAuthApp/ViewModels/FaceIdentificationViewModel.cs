using System.Windows.Input;
using System.Threading.Tasks;

using Xamarin.Forms;

using FaceAuthApp.Services;

namespace FaceAuthApp.ViewModels
{
    public class FaceIdentificationViewModel : BasePhotoViewModel
    {
        public ICommand IdentifyPersonCommand { get; private set; }

        private string name;

        public string Name
        {
            get => name;
            set { SetProperty(ref name, value); }
        }

        public FaceIdentificationViewModel()
        {
            TakePhotoCommand = new Command(async () => await TakePhoto());
            IdentifyPersonCommand = new Command(async () => await IdentifyPerson());
        }

        private async Task TakePhoto()
        {
            var image = await ImageService.GetPhoto(ImageMode.Camera);
            photo1Path = await CopyImage(image);
            Photo = image;
        }

        private async Task IdentifyPerson()
        {
            IsBusy = true;
            Name = await ServicioFace.IdentifyPerson(ReadFile(photo1Path));
            IsBusy = false;

            await App.Current.MainPage.DisplayAlert("Success!", $"Person identified: {Name}!", "OK");
        }
    }
}