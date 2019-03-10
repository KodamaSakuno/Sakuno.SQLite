using System;
using System.Threading;

namespace Sakuno.SQLite
{
    public abstract class SQLiteObject : IDisposable
    {
        volatile int _isDisposed;
        public bool IsClosed => _isDisposed != 0;

        internal protected SQLiteObject() { }

        ~SQLiteObject() => Dispose(false);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (_isDisposed != 0 || Interlocked.CompareExchange(ref _isDisposed, 1, 0) != 0 || !disposing)
                return;

            DisposeManagedResource();
        }
        protected virtual void DisposeManagedResource() { }
    }
}
