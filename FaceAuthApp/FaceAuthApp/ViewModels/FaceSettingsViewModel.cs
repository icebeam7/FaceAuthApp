using System.Windows.Input;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Essentials;

using FaceAuthApp.Helpers;
using FaceAuthApp.Services;

namespace FaceAuthApp.ViewModels
{
    public class FaceSettingsViewModel : BaseViewModel
    {
        public ICommand CreateGroupCommand { get; private set; }

        private string group;

        public string Group
        {
            get => group;
            set { SetProperty(ref group, value); }
        }

        private string id;

        public string Id
        {
            get => id;
            set { SetProperty(ref id, value); }
        }

        public FaceSettingsViewModel()
        {
            CreateGroupCommand = new Command(async () => await CreateGroup());
            Id = Preferences.Get(Constants.GroupKey, string.Empty);
        }

        private async Task CreateGroup()
        {
            IsBusy = true;

            Id = await ServicioFace.GetGroupId(Group);
            await App.Current.MainPage.DisplayAlert("Success!", "Group Created!", "OK");
            IsBusy = false;
        }
    }
}