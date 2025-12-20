using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace DiaryTaskManagerApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            SQLitePCL.Batteries_V2.Init();
            
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "tasks.db3");
            builder.Services.AddSingleton(new Data.TaskDatabase(dbPath));
            builder.Services.AddSingleton<Data.ITaskRepository, Data.SqliteTaskRepository>();

            builder.Services.AddTransient<Features.Tasks.ViewModels.TasksPageViewModel>();
            builder.Services.AddTransient<Features.Tasks.Views.TasksPage>();
            builder.Services.AddTransient<Features.Profile.ViewModels.ProfilePageViewModel>();
            builder.Services.AddTransient<Features.Profile.Views.ProfilePage>();
            builder.Services.AddSingleton<AppShell>();

            return builder.Build();
        }
    }
}
