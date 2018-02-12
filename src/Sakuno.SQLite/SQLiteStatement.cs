using System;
using System.Collections.Generic;

namespace Sakuno.SQLite
{
    public class SQLiteStatement : SQLiteObject
    {
        SQLiteDatabase _database;

        SQLiteStatementHandle _handle;

        public string SQL => SQLiteNativeMethods.sqlite3_sql(_handle);

        public bool IsReadOnly => SQLiteNativeMethods.sqlite3_stmt_readonly(_handle) != 0;

        SortedList<string, int> _parameterIndexes;

        public int ColumnCount => SQLiteNativeMethods.sqlite3_column_count(_handle);

        internal SQLiteStatement(SQLiteDatabase database, SQLiteStatementHandle handle)
        {
            _database = database;
            _handle = handle;

            var parameterCount = SQLiteNativeMethods.sqlite3_bind_parameter_count(_handle);
            if (parameterCount == 0)
                return;

            _parameterIndexes = new SortedList<string, int>(StringComparer.Ordinal);

            for (var i = 1; i <= parameterCount; i++)
            {
                var parameterName = SQLiteNativeMethods.sqlite3_bind_parameter_name(_handle, i);

                _parameterIndexes[parameterName] = i;
            }
        }

        protected override void DisposeManagedResource()
        {
            _handle.Dispose();
        }

        public string GetExpandedSQL() => SQLiteNativeMethods.sqlite3_expanded_sql(_handle);

        public SQLiteResultCode Execute() => SQLiteNativeMethods.sqlite3_step(_handle);

        public void Reset() => SQLiteNativeMethods.sqlite3_reset(_handle);

        public T Get<T>(int column)
        {
            var call = Datatype.Of<T>.Get ?? throw new NotSupportedException();

            return call(_handle, column);
        }

        public string GetColumnName(int column) => SQLiteNativeMethods.sqlite3_column_name(_handle, column);

        public string GetDatabaseName(int column) => SQLiteNativeMethods.sqlite3_column_database_name(_handle, column);
        public string GetTableName(int column) => SQLiteNativeMethods.sqlite3_column_table_name(_handle, column);
        public string GetOriginName(int column) => SQLiteNativeMethods.sqlite3_column_origin_name(_handle, column);

        static class Cache<T>
        {
            public static Func<SQLiteStatement, SQLiteQuery, T> Call = null;
        }
    }
}
