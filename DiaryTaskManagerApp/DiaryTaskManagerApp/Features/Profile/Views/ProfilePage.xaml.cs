using DiaryTaskManagerApp.Features.Profile.ViewModels;

namespace DiaryTaskManagerApp.Features.Profile.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfilePageViewModel _vm;

    public ProfilePage(ProfilePageViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        _vm.RequestClose += OnRequestClose;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.Load();
    }

    protected override void OnDisappearing()
    {
        _vm.RequestClose -= OnRequestClose;
        base.OnDisappearing();
    }

    private async void OnRequestClose()
    {
        await Navigation.PopModalAsync();
    }
}



