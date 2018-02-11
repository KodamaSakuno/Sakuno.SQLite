using System;

namespace Sakuno.SQLite
{
    public class SQLiteException : Exception
    {
        public SQLiteResultCode ErrorCode { get; }

        public SQLiteException(SQLiteResultCode errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}
