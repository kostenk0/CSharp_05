using CSharp_05.Models;
using CSharp_05.ViewModel;
using System.Windows;

namespace CSharp_05.Windows
{
    /// <summary>
    /// Interaction logic for ThreadsListView.xaml
    /// </summary>
    public partial class ThreadsListView : Window
    {
        public ThreadsListView(MyProcess process)
        {
            InitializeComponent();
            DataContext = new ThreadsListViewModel(process);
        }
    }
}
