using DiaryTaskManagerApp.Features.Tasks.Views;
using DiaryTaskManagerApp.Features.Folders.Views;
using DiaryTaskManagerApp.Features.Settings.Views;
using DiaryTaskManagerApp.Features.Profile.Views;

namespace DiaryTaskManagerApp
{
    public partial class AppShell : Shell
    {
        public AppShell(TasksPage tasksPage, FoldersPage foldersPage, SettingsPage settingsPage)
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute("FolderDetails", typeof(FolderDetailsPage));

            Items.Add(new FlyoutItem
            {
                Title = "Main",
                Route = "Main",
                Icon = "home_icon.svg",
                Items = { new ShellContent { Content = tasksPage } }
            });
            Items.Add(new FlyoutItem
            {
                Title = "Folders",
                Route = "Folders",
                Icon = "folder_icon.svg",
                Items = { new ShellContent { Content = foldersPage } }
            });
            Items.Add(new FlyoutItem
            {
                Title = "Settings",
                Route = "Settings",
                Icon = "settings_icon.svg",
                Items = { new ShellContent { Content = settingsPage } }
            });
        }
    }
}
