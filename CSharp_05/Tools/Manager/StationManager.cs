using System;
using System.Windows;

namespace CSharp_05.Tools.Manager
{
    internal static class StationManager
    {
        public static event Action StopThreads;

        internal static void CloseApp()
        {
            MessageBox.Show("Stop");
            try
            {
                StopThreads?.Invoke();
            }
            catch
            {

            }
            Environment.Exit(1);
        }
    }
}
