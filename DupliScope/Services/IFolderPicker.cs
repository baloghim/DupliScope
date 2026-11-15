namespace DupliScope.Services;

public interface IFolderPicker
{
    /// <returns>The selected, unmodified path, or null if the dialog was cancelled.</returns>
    string? PickFolder(string? initialDirectory);
}
