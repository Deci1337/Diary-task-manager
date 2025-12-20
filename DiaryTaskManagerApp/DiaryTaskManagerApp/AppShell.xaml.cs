using DiaryTaskManagerApp.Features.Tasks.Views;
using DiaryTaskManagerApp.Features.Profile.Views;

namespace DiaryTaskManagerApp
{
    public partial class AppShell : Shell
    {
        public AppShell(TasksPage tasksPage)
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));

            Items.Add(new ShellContent
            {
                Route = "Tasks",
                Content = tasksPage
            });
        }
    }
}
