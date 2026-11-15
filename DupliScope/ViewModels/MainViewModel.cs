using System.IO;
using System.Runtime.InteropServices;
using DupliScope.Models;
using DupliScope.Services;

namespace DupliScope.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly IFolderPicker folderPicker;
    private SearchMode selectedMode;
    private bool isWelcomeScreen = true;
    private bool includeSubfolders = true;
    private bool useFileTypeFilter;
    private bool useExcludedFolders;
    private string? firstLocation;
    private string? secondLocation;
    private string? errorMessage;

    public MainViewModel(IFolderPicker folderPicker, IApplicationLifetime applicationLifetime)
    {
        this.folderPicker = folderPicker;
        NextCommand = new RelayCommand(() => SetScreen(false), () => IsWelcomeScreen);
        BackCommand = new RelayCommand(() => SetScreen(true), () => !IsWelcomeScreen);
        BrowseFirstCommand = new RelayCommand(() => Browse(false), () => !IsWelcomeScreen);
        BrowseSecondCommand = new RelayCommand(() => Browse(true), () => !IsWelcomeScreen && IsTwoLocations);
        ExitCommand = new RelayCommand(applicationLifetime.Shutdown);
    }

    public SearchMode SelectedMode
    {
        get => selectedMode;
        private set
        {
            if (!SetProperty(ref selectedMode, value)) return;
            OnPropertyChanged(nameof(IsSingleLocation));
            OnPropertyChanged(nameof(IsTwoLocations));
            OnPropertyChanged(nameof(SelectedModeTitle));
            OnPropertyChanged(nameof(LocationDescription));
            OnPropertyChanged(nameof(FirstLocationTitle));
            BrowseSecondCommand.NotifyCanExecuteChanged();
        }
    }

    public bool IsSingleLocation
    {
        get => SelectedMode == SearchMode.SingleLocation;
        set { if (value) SelectedMode = SearchMode.SingleLocation; }
    }
    public bool IsTwoLocations
    {
        get => SelectedMode == SearchMode.TwoLocations;
        set { if (value) SelectedMode = SearchMode.TwoLocations; }
    }
    // The project Version is the single source for the assembly and the UI badge.
    public string ApplicationVersion => typeof(MainViewModel).Assembly.GetName().Version?.ToString(2) ?? string.Empty;
    public bool IsWelcomeScreen => isWelcomeScreen;
    public bool IsLocationScreen => !isWelcomeScreen;
    public string SelectedModeTitle => IsSingleLocation ? "Keresés egy helyen belül" : "Keresés két hely között";
    public string LocationDescription => IsSingleLocation
        ? "Válaszd ki azt a meghajtót vagy mappát, amelyben majd keresni szeretnél."
        : "Válaszd ki az összehasonlítandó két meghajtót vagy mappát.";
    public string FirstLocationTitle => IsSingleLocation ? "Keresési hely" : "A hely";

    public bool IncludeSubfolders { get => includeSubfolders; set => SetProperty(ref includeSubfolders, value); }
    public bool UseFileTypeFilter { get => useFileTypeFilter; set => SetProperty(ref useFileTypeFilter, value); }
    public bool UseExcludedFolders { get => useExcludedFolders; set => SetProperty(ref useExcludedFolders, value); }
    public string? FirstLocation { get => firstLocation; private set => SetProperty(ref firstLocation, value); }
    public string? SecondLocation { get => secondLocation; private set => SetProperty(ref secondLocation, value); }
    public string? ErrorMessage
    {
        get => errorMessage;
        private set
        {
            if (SetProperty(ref errorMessage, value)) OnPropertyChanged(nameof(HasError));
        }
    }
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public RelayCommand NextCommand { get; }
    public RelayCommand BackCommand { get; }
    public RelayCommand BrowseFirstCommand { get; }
    public RelayCommand BrowseSecondCommand { get; }
    public RelayCommand ExitCommand { get; }

    public SearchOptions CreateSearchOptions() => new(
        SelectedMode, IncludeSubfolders, UseFileTypeFilter, UseExcludedFolders,
        FirstLocation, IsTwoLocations ? SecondLocation : null);

    private void SetScreen(bool welcome)
    {
        if (!SetProperty(ref isWelcomeScreen, welcome, nameof(IsWelcomeScreen))) return;
        OnPropertyChanged(nameof(IsLocationScreen));
        ErrorMessage = null;
        NextCommand.NotifyCanExecuteChanged();
        BackCommand.NotifyCanExecuteChanged();
        BrowseFirstCommand.NotifyCanExecuteChanged();
        BrowseSecondCommand.NotifyCanExecuteChanged();
    }

    private void Browse(bool second)
    {
        ErrorMessage = null;
        try
        {
            var path = folderPicker.PickFolder(second ? SecondLocation : FirstLocation);
            if (string.IsNullOrEmpty(path)) return;
            // Keep Unicode, drive roots and long paths intact. No enumeration or normalization here.
            if (second) SecondLocation = path;
            else FirstLocation = path;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException
                                        or COMException or InvalidOperationException or ArgumentException)
        {
            ErrorMessage = "A mappaválasztó nem nyitható meg, vagy a hely nem érhető el. Próbáld újra a kiválasztást.";
        }
    }
}
