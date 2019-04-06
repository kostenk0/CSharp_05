using CSharp_05.Models;
using CSharp_05.ViewModel;
using System.Windows;

namespace CSharp_05.Windows
{
    /// <summary>
    /// Interaction logic for ModulesListView.xaml
    /// </summary>
    public partial class ModulesListView : Window
    {
        public ModulesListView(MyProcess process)
        {
            InitializeComponent();
            DataContext = new ModulesListViewModel(process);
        }
    }
}
