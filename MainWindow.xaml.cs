using System.Windows;
using PortfolioTracker.ViewModels;

namespace PortfolioTracker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
