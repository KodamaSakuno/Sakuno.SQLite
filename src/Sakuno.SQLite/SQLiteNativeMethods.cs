#pragma warning disable IDE1006

using System;
using System.Runtime.InteropServices;

namespace Sakuno.SQLite
{
    static unsafe class SQLiteNativeMethods
    {
        const string DllName = "sqlite3";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sqlite3_malloc(int n);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sqlite3_free(IntPtr p);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_open_v2([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))] string filename, out SQLiteConnectionHandle ppDb, OpenDatabaseOptions flags, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))] string zVfs);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_close_v2(IntPtr db);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_extended_result_codes(SQLiteConnectionHandle db, bool onoff);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_extended_errcode(SQLiteConnectionHandle db);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_prepare_v2(SQLiteConnectionHandle db, byte* zSql, int nByte, out SQLiteStatementHandle ppStmt, out byte* pzTail);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_step(SQLiteStatementHandle pStmt);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_reset(SQLiteStatementHandle pStmt);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_finalize(IntPtr pStmt);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_bind_parameter_count(SQLiteStatementHandle pStmt);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_bind_parameter_index(SQLiteStatementHandle pStmt, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))] string zName);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringConstantMarshaler))]
        public static extern string sqlite3_bind_parameter_name(SQLiteStatementHandle pStmt, int i);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_bind_null(SQLiteStatementHandle pStmt, int i);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_bind_text(SQLiteStatementHandle pStmt, int i, IntPtr zData, int nData, IntPtr xDel);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_bind_int(SQLiteStatementHandle pStmt, int i, int iValue);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_bind_int64(SQLiteStatementHandle pStmt, int i, long iValue);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_bind_double(SQLiteStatementHandle pStmt, int i, double iValue);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_bind_blob(SQLiteStatementHandle pStmt, int i, byte[] zData, int nData, IntPtr xDel);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResultCode sqlite3_clear_bindings(SQLiteStatementHandle pStmt);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringConstantMarshaler))]
        public static extern string sqlite3_sql(SQLiteStatementHandle pStmt);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringMarshaler))]
        public static extern string sqlite3_expanded_sql(SQLiteStatementHandle pStmt);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_stmt_readonly(SQLiteStatementHandle pStmt);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_changes(SQLiteConnectionHandle db);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_total_changes(SQLiteConnectionHandle db);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_column_count(SQLiteStatementHandle pStmt);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteDatatype sqlite3_column_type(SQLiteStatementHandle pStmt, int iCol);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringConstantMarshaler))]
        public static extern string sqlite3_column_name(SQLiteStatementHandle pStmt, int iCol);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringConstantMarshaler))]
        public static extern string sqlite3_column_database_name(SQLiteStatementHandle pStmt, int iCol);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringConstantMarshaler))]
        public static extern string sqlite3_column_table_name(SQLiteStatementHandle pStmt, int iCol);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringConstantMarshaler))]
        public static extern string sqlite3_column_origin_name(SQLiteStatementHandle pStmt, int iCol);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sqlite3_column_blob(SQLiteStatementHandle pStmt, int iCol);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_column_bytes(SQLiteStatementHandle pStmt, int iCol);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern double sqlite3_column_double(SQLiteStatementHandle pStmt, int iCol);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_column_int(SQLiteStatementHandle pStmt, int iCol);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long sqlite3_column_int64(SQLiteStatementHandle pStmt, int iCol);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringConstantMarshaler))]
        public static extern string sqlite3_column_text(SQLiteStatementHandle pStmt, int iCol);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteValueHandle sqlite3_column_value(SQLiteStatementHandle pStmt, int iCol);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sqlite3_value_blob(SQLiteValueHandle pVal);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_value_bytes(SQLiteValueHandle pVal);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sqlite3_value_int(SQLiteValueHandle pVal);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long sqlite3_value_int64(SQLiteValueHandle pVal);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern double sqlite3_value_double(SQLiteValueHandle pVal);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(UTF8StringConstantMarshaler))]
        public static extern string sqlite3_value_text(SQLiteValueHandle pVal);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteDatatype sqlite3_value_type(SQLiteValueHandle pVal);
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sqlite3_value_free(IntPtr pVal);

    }
}
#pragma warning restore IDE1006
