using System;
using System.Runtime.InteropServices;

namespace Sakuno.SQLite
{
    class SQLiteValueHandle : CriticalHandle
    {
        public override bool IsInvalid => handle == IntPtr.Zero;

        public SQLiteValueHandle() : base(IntPtr.Zero) { }

        protected override bool ReleaseHandle()
        {
            SQLiteNativeMethods.sqlite3_value_free(handle);

            return true;
        }
    }
}
