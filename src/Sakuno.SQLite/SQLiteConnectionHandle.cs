using System;
using System.Runtime.InteropServices;

namespace Sakuno.SQLite
{
    class SQLiteConnectionHandle : CriticalHandle
    {
        public override bool IsInvalid => handle == IntPtr.Zero;

        public SQLiteConnectionHandle() : base(IntPtr.Zero) { }

        protected override bool ReleaseHandle() => SQLiteNativeMethods.sqlite3_close_v2(handle) == SQLiteResultCode.OK;
    }
}
