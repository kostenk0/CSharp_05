using CSharp_05.Models;
using System.Diagnostics;

namespace CSharp_05.ViewModel
{
    internal class ThreadsListViewModel
    {
        public ProcessThreadCollection ProcessThreads { get; private set; }

        public ThreadsListViewModel(MyProcess process)
        {
            ProcessThreads = process.Process.Threads;
        }
    }
}
