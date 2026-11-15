using System.Windows;

namespace DupliScope.Services;

public sealed class WpfApplicationLifetime : IApplicationLifetime
{
    public void Shutdown() => Application.Current.Shutdown();
}
