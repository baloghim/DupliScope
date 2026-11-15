using System.Windows;
using DupliScope.Services;
using DupliScope.ViewModels;

namespace DupliScope;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // Keep the first window within the available desktop even at high DPI.
        var workArea = SystemParameters.WorkArea;
        MinWidth = Math.Min(MinWidth, workArea.Width);
        MinHeight = Math.Min(MinHeight, workArea.Height);
        Width = Math.Min(Width, workArea.Width);
        Height = Math.Min(Height, workArea.Height);
        DataContext = new MainViewModel(new WindowsFolderPicker(), new WpfApplicationLifetime());
    }
}
