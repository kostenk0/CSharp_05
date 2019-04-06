using CSharp_05.Models;
using System.Diagnostics;

namespace CSharp_05.ViewModel
{
    internal class ModulesListViewModel
    {
        public ProcessModuleCollection ProcessModules { get; private set; }

        public ModulesListViewModel(MyProcess process)
        {
            ProcessModules = process.Process.Modules;
        }
    }
}
