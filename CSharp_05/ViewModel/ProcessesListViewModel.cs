using CSharp_05.Models;
using CSharp_05.Properties;
using CSharp_05.Tools;
using CSharp_05.Tools.Manager;
using CSharp_05.Windows;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Data;

namespace CSharp_05.ViewModel
{
    internal class ProcessesListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MyProcess> Processes { get; private set; }
        private Thread _workingThread;
        private CancellationToken _token;
        private CancellationTokenSource _tokenSource;
        private bool _isControlEnabled;
        private RelayCommand<object> _modulesShow;
        private RelayCommand<object> _threadsShow;
        private RelayCommand<object> _toFolder;
        private RelayCommand<object> _terminate;
        public CollectionViewSource ViewSource { get; private set; }
        public MyProcess SelectedProcess { get; set; }

        internal ProcessesListViewModel()
        {
            Processes = new ObservableCollection<MyProcess>();
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StationManager.StopThreads += StopWorkingThread;
            this.ViewSource = new CollectionViewSource();
            ViewSource.Source = this.Processes;
            IsControlEnabled = false;
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    Processes.Add(new MyProcess(process));
                }
                catch (Exception e)
                {

                }
            }
            IsControlEnabled = true;
            Thread.Sleep(5000);
            StartWorkingThread();
        }

        public bool IsControlEnabled
        {
            get
            {
                return _isControlEnabled;
            }
            set
            {
                _isControlEnabled = value;
                OnPropertyChanged();
            }
        }

        private void StartWorkingThread()
        {
            _workingThread = new Thread(WorkingThreadProcess);
            _workingThread.Start();
        }

        private void WorkingThreadProcess()
        {

            while (!_token.IsCancellationRequested)
            {
                for (int j = 0; j < 5; j++)
                {
                    Thread.Sleep(1000);
                    if (_token.IsCancellationRequested)
                        break;
                }
                if (_token.IsCancellationRequested)
                    break;
                IsControlEnabled = false;
                var t = Process.GetProcesses().Select(p => p.Id).ToList();
                var currentProcesses = Processes.Select(p => p.Id).ToList();
                var newProcesses = t.Except(currentProcesses).ToList();
                var processesToRemove = currentProcesses.Except(t).ToList();
                foreach (int id in newProcesses)
                {
                    try
                    {
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            Processes.Add(new MyProcess(Process.GetProcessById(id)));
                        });
                    }
                    catch (Exception)
                    {
                    }
                }
                foreach (int id in processesToRemove)
                {
                    try
                    {
                        var proces = from proc in Processes
                                     where proc.Id == id
                                     select proc;

                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                        {
                            proces.First().Kill = true;
                            Processes.RemoveAt(Processes.IndexOf(proces.First()));
                        });
                    }
                    catch (Exception e)
                    {
                    }
                }
                IsControlEnabled = true;

            }
        }

        public RelayCommand<object> ModulesShow
        {
            get
            {
                return _modulesShow ?? (_modulesShow = new RelayCommand<object>(
                           ModulesShowImplementation));
            }
        }

        public RelayCommand<object> ThreadsShow
        {
            get
            {
                return _threadsShow ?? (_threadsShow = new RelayCommand<object>(
                           ThreadsShowImplementation));
            }
        }

        public RelayCommand<object> ToFolder
        {
            get
            {
                return _toFolder ?? (_toFolder = new RelayCommand<object>(
                           ToFolderImplementation));
            }
        }

        public RelayCommand<object> Terminate
        {
            get
            {
                return _terminate ?? (_terminate = new RelayCommand<object>(
                           TerminateImplementation));
            }
        }

        private void ModulesShowImplementation(object obj)
        {
            ModulesListView modules = new ModulesListView(SelectedProcess);
            modules.Show();
        }

        private void ThreadsShowImplementation(object obj)
        {
            ThreadsListView threads = new ThreadsListView(SelectedProcess);
            threads.Show();
        }

        private void ToFolderImplementation(object obj)
        {
            System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(SelectedProcess.Folder));
        }

        private void TerminateImplementation(object obj)
        {
            SelectedProcess.Process.Kill();
        }

        internal void StopWorkingThread()
        {
            _tokenSource.Cancel();
            _workingThread.Join(2000);
            _workingThread.Abort();
            _workingThread = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
