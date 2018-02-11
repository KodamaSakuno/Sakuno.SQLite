using System.Collections.Generic;

namespace Sakuno.SQLite
{
    public class SQLiteDatabase : SQLiteObject
    {
        SQLiteConnectionHandle _handle;

        public SQLiteResultCode LastErrorCode => SQLiteNativeMethods.sqlite3_extended_errcode(_handle);

        public SQLiteDatabase(string filename) : this(filename, OpenDatabaseOptions.ReadWrite | OpenDatabaseOptions.Create) { }
        public SQLiteDatabase(string filename, OpenDatabaseOptions options)
        {
            var resultCode = SQLiteNativeMethods.sqlite3_open_v2(filename, out _handle, options, null);

            SQLiteNativeMethods.sqlite3_extended_result_codes(_handle, true);

            if (resultCode != SQLiteResultCode.OK)
            {
                _handle.Dispose();
                throw new SQLiteException(resultCode);
            }
        }

        public static SQLiteDatabase OpenMemoryDatabase() => new SQLiteDatabase(":memory:");

        protected override void DisposeManagedResource()
        {
            _handle.Dispose();
        }
    }
}
