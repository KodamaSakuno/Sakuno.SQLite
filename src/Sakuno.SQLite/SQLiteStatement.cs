using System.Collections.Generic;

namespace Sakuno.SQLite
{
    public class SQLiteStatement : SQLiteObject
    {
        SQLiteDatabase _database;

        SQLiteStatementHandle _handle;

        public string SQL => SQLiteNativeMethods.sqlite3_sql(_handle);

        public bool IsReadOnly => SQLiteNativeMethods.sqlite3_stmt_readonly(_handle) != 0;

        internal SQLiteStatement(SQLiteDatabase database, SQLiteStatementHandle handle)
        {
            _database = database;
            _handle = handle;
        }

        protected override void DisposeManagedResource()
        {
            _handle.Dispose();
        }

        public string GetExpandedSQL() => SQLiteNativeMethods.sqlite3_expanded_sql(_handle);
    }
}
