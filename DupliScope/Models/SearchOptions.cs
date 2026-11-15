namespace DupliScope.Models;

/// <summary>An immutable snapshot for a future asynchronous search service.</summary>
public sealed record SearchOptions(
    SearchMode Mode, bool IncludeSubfolders, bool UseFileTypeFilter,
    bool UseExcludedFolders, string? FirstLocation, string? SecondLocation);
