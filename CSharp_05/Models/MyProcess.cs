using CSharp_05.Properties;
using CSharp_05.Tools.Manager;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace CSharp_05.Models
{
    public class MyProcess : INotifyPropertyChanged
    {
        private readonly Process _process;
        private readonly string _name;
        private readonly int _id;
        private readonly string _user;
        private readonly string _folder;
        private readonly DateTime _launchDateTime;
        private bool _isActive;
        private double _cpu;
        private double _memory;
        private int _threadsCount;
        private Thread _workingThread;
        private CancellationToken _token;
        private bool _kill;
        private CancellationTokenSource _tokenSource;


        internal MyProcess(Process process)
        {
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StationManager.StopThreads += StopWorkingThread;
            _kill = false;
            _process = process;
            _name = process.ProcessName;
            _id = process.Id;
            _user = process.StartInfo.UserName;
            _folder = process.MainModule.FileName;
            _launchDateTime = process.StartTime;
            StartWorkingThread();
        }

        public string Name => _name;
        public int Id => _id;
        public string User => _user;
        public string Folder => _folder;
        public DateTime LaunchDateTime => _launchDateTime;
        public bool Kill
        {
            get
            {
                return _kill;
            }
            set
            {
                _kill = value;
                StopWorkingThread();
                StationManager.StopThreads -= StopWorkingThread;
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }
        public Process Process => _process;

        private void StartWorkingThread()
        {
            _workingThread = new Thread(GetUsage);
            _workingThread.Start();
        }

        private void GetUsage()
        {

            while (!_token.IsCancellationRequested && !Kill)
            {
                try
                {
                    var cpu = new PerformanceCounter("Process", "% Processor Time", _process.ProcessName, true);
                    var ram = new PerformanceCounter("Process", "Private Bytes", _process.ProcessName, true);

                    // Getting first initial values
                    cpu.NextValue();
                    ram.NextValue();
                    // If system has multiple cores, that should be taken into account
                    CPU = Math.Round(cpu.NextValue() / Environment.ProcessorCount, 2);
                    // Returns number of MB consumed by application
                    MEM = Math.Round(ram.NextValue() / 1024 / 1024, 2);
                    IsActive = Process.Responding;
                    ThreadsCnt = Process.Threads.Count;
                }
                catch (Exception)
                {
                }
                for (int j = 0; j < 2; j++)
                {
                    Thread.Sleep(1000);
                    if (_token.IsCancellationRequested && Kill)
                        break;
                }
                if (_token.IsCancellationRequested && Kill)
                    break;
            }

        }
        public double CPU
        {
            get
            {
                return _cpu;
            }
            private set
            {
                _cpu = value;
                OnPropertyChanged();
            }
        }

        public double MEM
        {
            get
            {
                return _memory;
            }
            private set
            {
                _memory = value;
                OnPropertyChanged();
            }
        }
        public int ThreadsCnt
        {
            get
            {
                return _threadsCount;
            }
            set
            {
                _threadsCount = value;
                OnPropertyChanged();
            }
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
