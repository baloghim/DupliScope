using System.Reflection;
using System.Resources;
using DupliScope.Models;
using DupliScope.Services;
using DupliScope.ViewModels;

var checks = new List<(string Name, Action Run)>
{
    ("Initial mode, settings and command availability", () =>
    {
        var (vm, _, lifetime) = Create();
        Require(vm.IsWelcomeScreen && !vm.IsLocationScreen && vm.IsSingleLocation && !vm.IsTwoLocations);
        Require(vm.IncludeSubfolders && !vm.UseFileTypeFilter && !vm.UseExcludedFolders);
        Require(vm.NextCommand.CanExecute(null) && !vm.BackCommand.CanExecute(null));
        Require(!vm.BrowseFirstCommand.CanExecute(null) && !vm.BrowseSecondCommand.CanExecute(null));
        Require(lifetime.Calls == 0);
    }),
    ("Exclusive mode selection and binding notifications", () =>
    {
        var (vm, _, _) = Create();
        var changed = new List<string?>();
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);
        vm.IsTwoLocations = true;
        Require(vm.SelectedMode == SearchMode.TwoLocations && !vm.IsSingleLocation && vm.IsTwoLocations);
        Require(new[] { "IsSingleLocation", "IsTwoLocations", "SelectedModeTitle", "FirstLocationTitle" }.All(changed.Contains));
        vm.IsTwoLocations = false; // WPF group unchecks must not select an unintended mode.
        Require(vm.IsTwoLocations);
        vm.IsSingleLocation = true;
        Require(vm.IsSingleLocation && !vm.IsTwoLocations);
    }),
    ("Navigation keeps mode and all three options", () =>
    {
        var (vm, _, _) = Create();
        vm.IsTwoLocations = true;
        vm.IncludeSubfolders = false;
        vm.UseFileTypeFilter = vm.UseExcludedFolders = true;
        vm.NextCommand.Execute(null);
        Require(vm.IsLocationScreen && !vm.IsWelcomeScreen && vm.BackCommand.CanExecute(null));
        Require(!vm.NextCommand.CanExecute(null) && vm.BrowseSecondCommand.CanExecute(null));
        vm.BackCommand.Execute(null);
        Require(vm.IsWelcomeScreen && vm.IsTwoLocations && !vm.IncludeSubfolders);
        Require(vm.UseFileTypeFilter && vm.UseExcludedFolders);
    }),
    ("Unavailable commands do not browse", () =>
    {
        var (vm, picker, _) = Create();
        vm.BrowseFirstCommand.Execute(null);
        vm.BrowseSecondCommand.Execute(null);
        vm.NextCommand.Execute(null);
        vm.BrowseSecondCommand.Execute(null);
        Require(picker.Calls == 0);
    }),
    ("Drive root selection, cancel and initial directory", () =>
    {
        var (vm, picker, _) = Create();
        vm.NextCommand.Execute(null);
        picker.Result = @"D:\";
        vm.BrowseFirstCommand.Execute(null);
        Require(vm.FirstLocation == @"D:\");
        picker.Result = null;
        vm.BrowseFirstCommand.Execute(null);
        Require(vm.FirstLocation == @"D:\" && picker.InitialDirectory == @"D:\");
    }),
    ("Unicode and long paths remain unchanged", () =>
    {
        var (vm, picker, _) = Create();
        vm.IsTwoLocations = true;
        vm.NextCommand.Execute(null);
        var path = @"\\server\mentés\" + string.Concat(Enumerable.Repeat("Árvíztűrő_日本語\\", 30));
        picker.Result = path;
        vm.BrowseSecondCommand.Execute(null);
        Require(path.Length > 260 && vm.SecondLocation == path && vm.CreateSearchOptions().SecondLocation == path);
    }),
    ("Both locations survive back, next and mode switches", () =>
    {
        var (vm, picker, _) = Create();
        vm.IsTwoLocations = true;
        vm.NextCommand.Execute(null);
        picker.Result = @"C:\A";
        vm.BrowseFirstCommand.Execute(null);
        picker.Result = @"E:\B";
        vm.BrowseSecondCommand.Execute(null);
        vm.BackCommand.Execute(null);
        vm.IsSingleLocation = true;
        vm.NextCommand.Execute(null);
        Require(vm.CreateSearchOptions().SecondLocation is null);
        vm.BackCommand.Execute(null);
        vm.IsTwoLocations = true;
        vm.NextCommand.Execute(null);
        Require(vm.FirstLocation == @"C:\A" && vm.SecondLocation == @"E:\B");
    }),
    ("Picker errors preserve selection and allow recovery", () =>
    {
        var (vm, picker, _) = Create();
        vm.NextCommand.Execute(null);
        picker.Result = @"C:\Good";
        vm.BrowseFirstCommand.Execute(null);
        picker.Error = new UnauthorizedAccessException();
        vm.BrowseFirstCommand.Execute(null);
        Require(vm.HasError && vm.FirstLocation == @"C:\Good");
        picker.Error = null;
        picker.Result = @"C:\New";
        vm.BrowseFirstCommand.Execute(null);
        Require(!vm.HasError && vm.FirstLocation == @"C:\New");
    }),
    ("Snapshot is immutable and excludes inactive second location", () =>
    {
        var (vm, _, _) = Create();
        var snapshot = vm.CreateSearchOptions();
        vm.IncludeSubfolders = false;
        vm.UseFileTypeFilter = true;
        Require(snapshot.IncludeSubfolders && !snapshot.UseFileTypeFilter && snapshot.SecondLocation is null);
        Require(!vm.CreateSearchOptions().IncludeSubfolders && vm.CreateSearchOptions().UseFileTypeFilter);
    }),
    ("Command state changes notify WPF", () =>
    {
        var (vm, _, _) = Create();
        int next = 0, back = 0, browse = 0;
        vm.NextCommand.CanExecuteChanged += (_, _) => next++;
        vm.BackCommand.CanExecuteChanged += (_, _) => back++;
        vm.BrowseSecondCommand.CanExecuteChanged += (_, _) => browse++;
        vm.IsTwoLocations = true;
        vm.NextCommand.Execute(null);
        vm.BackCommand.Execute(null);
        Require(next == 2 && back == 2 && browse == 3);
    }),
    ("Exit delegates to application lifetime", () =>
    {
        var (vm, _, lifetime) = Create();
        vm.ExitCommand.Execute(null);
        Require(lifetime.Calls == 1);
    }),
    ("RelayCommand respects CanExecute", () =>
    {
        int calls = 0;
        var command = new RelayCommand(() => calls++, () => false);
        command.Execute(null);
        Require(calls == 0);
    })
};

foreach (var path in args)
{
    checks.Add(($"Embedded BAML, icons and dictionaries: {path}", () =>
    {
        // Inspect the compiled resources without instantiating WPF on Linux.
        var assembly = Assembly.LoadFile(Path.GetFullPath(path));
        using var stream = assembly.GetManifestResourceStream("DupliScope.g.resources")
            ?? throw new Exception("Missing WPF resources");
        using var resources = new ResourceReader(stream);
        var names = new HashSet<string>();
        var enumerator = resources.GetEnumerator();
        while (enumerator.MoveNext()) names.Add((string)enumerator.Key);
        string[] expected = ["mainwindow.baml", "views/welcomeview.baml", "views/locationsview.baml", "resources/styles.baml",
            "assets/icons/app/dupliscope.ico", "assets/icons/app/dupliscope.png",
            "assets/icons/oxylite/folder.png", "assets/icons/oxylite/kr_comparedirs.png",
            "assets/icons/oxylite/application-exit.png", "assets/icons/oxylite/go-next.png", "assets/icons/oxylite/go-previous.png"];
        foreach (var name in expected)
        {
            Require(names.Contains(name), $"Missing resource: {name}");
            resources.GetResourceData(name, out _, out var data);
            Require(data.Length > 0, $"Empty resource: {name}");
        }
    }));
}

int failed = 0;
foreach (var (name, run) in checks)
{
    try { run(); Console.WriteLine($"PASS {name}"); }
    catch (Exception ex) { failed++; Console.Error.WriteLine($"FAIL {name}: {ex.Message}"); }
}
Console.WriteLine($"{checks.Count - failed}/{checks.Count} checks passed; {failed} failed.");
return failed == 0 ? 0 : 1;

static void Require(bool condition, string message = "Assertion failed")
{
    if (!condition) throw new Exception(message);
}

static (MainViewModel, FakePicker, FakeLifetime) Create()
{
    var picker = new FakePicker();
    var lifetime = new FakeLifetime();
    return (new MainViewModel(picker, lifetime), picker, lifetime);
}

sealed class FakePicker : IFolderPicker
{
    public string? Result { get; set; }
    public Exception? Error { get; set; }
    public string? InitialDirectory { get; private set; }
    public int Calls { get; private set; }
    public string? PickFolder(string? initialDirectory)
    {
        Calls++;
        InitialDirectory = initialDirectory;
        if (Error is not null) throw Error;
        return Result;
    }
}

sealed class FakeLifetime : IApplicationLifetime
{
    public int Calls { get; private set; }
    public void Shutdown() => Calls++;
}
