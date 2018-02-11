using System;
using System.Runtime.InteropServices;

namespace Sakuno.SQLite
{
    class SQLiteStatementHandle : CriticalHandle
    {
        public override bool IsInvalid => handle == IntPtr.Zero;

        public SQLiteStatementHandle() : base(IntPtr.Zero) { }

        protected override bool ReleaseHandle() => SQLiteNativeMethods.sqlite3_finalize(handle) == SQLiteResultCode.OK;
    }
}
