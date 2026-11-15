using System.Windows;
using Microsoft.Win32;

namespace DupliScope.Services;

public sealed class WindowsFolderPicker : IFolderPicker
{
    public string? PickFolder(string? initialDirectory)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Mappa vagy meghajtó kiválasztása",
            Multiselect = false,
            InitialDirectory = initialDirectory ?? string.Empty
        };
        return dialog.ShowDialog(Application.Current.MainWindow) == true ? dialog.FolderName : null;
    }
}
