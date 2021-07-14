using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using System.Linq;

namespace HW7
{
    public class DocumentsReceiver: IDisposable
    {

        private readonly string _targetDirectory;
        private readonly int _waitingInterval;
        private readonly List<string> _fileNames = new List<string>();

        public EventHandler<EventArgs> DocumentsReady;
        public EventHandler<EventArgs> TimedOut;

        private Timer _timer;
        private FileSystemWatcher _watcher;
        private bool disposedValue;

        public DocumentsReceiver(string targetDirectory, int waitingInterval, IEnumerable<string> fileNames)
        {
            _targetDirectory = targetDirectory;
            _waitingInterval = waitingInterval;
            _fileNames = fileNames.ToList();
        }

        public void Start()
        {

            if (IsDocumentsReady())
            {
                if (DocumentsReady != null)
                {
                    DocumentsReady(this, EventArgs.Empty);
                    return;
                }
            }

            _watcher = new FileSystemWatcher(_targetDirectory) { EnableRaisingEvents = true };
            _watcher.Changed += WatcherChanged;

            _timer = new Timer(_waitingInterval);
            _timer.Elapsed += TimerElapsed;
            _timer.Enabled = true;
        }

        public bool IsDocumentsReady()
        {
            var existingFiles = Directory.EnumerateFiles(_targetDirectory).ToList();

            var isReady = true;

            foreach (var fileName in _fileNames)
            {
                var fullPath = Path.Combine(_targetDirectory, fileName);

                if (!existingFiles.Contains(fullPath))
                {
                    isReady = false;
                    break;
                }
            }

            return isReady;
        }

        private void WatcherChanged(object sender, FileSystemEventArgs e)
        {
            if (IsDocumentsReady())
            {
                if (DocumentsReady != null)
                {
                    DocumentsReady(this, e);
                }

                _timer.Enabled = false;
            }
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {

            _timer.Enabled = false;

            if (IsDocumentsReady())
            {
                return;
            }

            if (TimedOut != null)
            {
                TimedOut(this, e);
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_watcher != null)
                    {
                        _watcher.Changed -= WatcherChanged;
                        _watcher.Dispose();
                    }

                    if (_timer != null)
                    {
                        _timer.Elapsed -= TimerElapsed;
                        _timer.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
