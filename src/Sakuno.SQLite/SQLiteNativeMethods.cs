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

    }
}
