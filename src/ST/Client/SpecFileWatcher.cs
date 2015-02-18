using System;
using System.IO;

namespace ST.Client
{
    public class SpecFileWatcher : ISpecFileWatcher
    {
        private FileSystemWatcher _watcher;

        public void Dispose()
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
        }

        public class Latch : IDisposable
        {
            private readonly SpecFileWatcher _parent;

            public Latch(SpecFileWatcher parent, string file)
            {
                _parent = parent;
                _parent._watcher.EnableRaisingEvents = false;
            }

            public void Dispose()
            {
                _parent._watcher.EnableRaisingEvents = true;
            }
        }

        public IDisposable LatchFile(string file)
        {
            return new Latch(this, file);
        }

        public void StartWatching(string path, ISpecFileObserver observer)
        {
            _watcher = new FileSystemWatcher(path, "*.xml");
            _watcher.Changed += (sender, args) =>
            {
                observer.Changed(args.FullPath);
            };

            _watcher.Deleted += (sender, args) =>
            {
                observer.Deleted(args.FullPath);
            };

            _watcher.Created += (sender, args) =>
            {
                observer.Added(args.FullPath);
            };
        }
    }
}