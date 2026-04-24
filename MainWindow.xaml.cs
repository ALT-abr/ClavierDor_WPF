using System.Windows;
using System.Windows.Controls;

namespace clavierdor;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigate(new Views.Pages.Home());
    }
}
