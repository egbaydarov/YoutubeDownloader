using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using YoutubeDownloader.Framework;
using YoutubeDownloader.Services;
using YoutubeDownloader.Utils;
using YoutubeDownloader.ViewModels.Components;

namespace YoutubeDownloader.ViewModels;

public partial class MainViewModel(
    ViewModelManager viewModelManager,
    DialogManager dialogManager,
    SettingsService settingsService
) : ViewModelBase
{
    public string Title { get; } = $"{Program.Name} v{Program.VersionString}";

    public DashboardViewModel Dashboard { get; } = viewModelManager.CreateDashboardViewModel();

    private async Task ShowDevelopmentBuildMessageAsync()
    {
        if (!Program.IsDevelopmentBuild)
            return;

        // If debugging, the user is likely a developer
        if (Debugger.IsAttached)
            return;

        var dialog = viewModelManager.CreateMessageBoxViewModel(
            "Unstable build warning",
            """
            You're using a development build of the application. These builds are not thoroughly tested and may contain bugs.

            Auto-updates are disabled for development builds. If you want to switch to a stable release, please download it manually.
            """,
            "SEE RELEASES",
            "CLOSE"
        );

        if (await dialogManager.ShowDialogAsync(dialog) == true)
            ProcessEx.StartShellExecute(Program.ProjectReleasesUrl);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        await ShowDevelopmentBuildMessageAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Save settings
            settingsService.Save();
        }

        base.Dispose(disposing);
    }
}
