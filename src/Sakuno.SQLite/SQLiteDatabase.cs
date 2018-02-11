using System.Collections.Generic;
using System.Text;

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

        public unsafe SQLiteQuery CreateQuery(string sql)
        {
            var statements = new List<SQLiteStatement>();

            var marshaler = UTF8StringMarshaler.Instance;
            var nativeData = marshaler.MarshalManagedToNative(sql, out var length);
            var current = (byte*)nativeData;

            try
            {
                do
                {
                    var resultCode = SQLiteNativeMethods.sqlite3_prepare_v2(_handle, current, length + 1, out var statementHandle, out var remaining);
                    if (resultCode != SQLiteResultCode.OK)
                    {
                        statementHandle.Dispose();
                        throw new SQLiteException(resultCode);
                    }

                    statements.Add(new SQLiteStatement(this, statementHandle));

                    length -= (int)(remaining - current);
                    current = remaining;

                } while (length > 0);
            }
            finally
            {
                marshaler.CleanUpNativeData(nativeData);
            }

            return new SQLiteQuery(this, statements);
        }
    }
}
