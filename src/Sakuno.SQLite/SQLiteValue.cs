using System;

namespace Sakuno.SQLite
{
    public class SQLiteValue : SQLiteObject
    {
        SQLiteValueHandle _handle;

        public SQLiteDatatype Type { get; }

        internal SQLiteValue(SQLiteValueHandle handle)
        {
            _handle = handle;

            Type = SQLiteNativeMethods.sqlite3_value_type(_handle);
        }

        protected override void DisposeManagedResource()
        {
            _handle.Dispose();
        }

        public T Get<T>()
        {
            var call = Datatype.Of<T>.FromValue ?? throw new InvalidOperationException();

            return call(_handle);
        }
    }
}
