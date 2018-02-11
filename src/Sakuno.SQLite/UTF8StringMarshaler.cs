using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sakuno.SQLite
{
    class UTF8StringMarshaler : ICustomMarshaler
    {
        static Encoding _utf8 = Encoding.UTF8;

        public static UTF8StringMarshaler Instance { get; } = new UTF8StringMarshaler();

        public static ICustomMarshaler GetInstance(string cookie = null) => Instance;

        public IntPtr MarshalManagedToNative(object managedObject) => MarshalManagedToNative(managedObject, out _);
        public unsafe IntPtr MarshalManagedToNative(object managedObject, out int length)
        {
            length = 0;

            if (managedObject == null)
                return IntPtr.Zero;

            var str = managedObject as string ?? throw new MarshalDirectiveException();
            length = _utf8.GetByteCount(str);
            var nativeData = SQLiteNativeMethods.sqlite3_malloc(length + 1);
            var buffer = (byte*)nativeData;

            if (length > 0)
                fixed (char* strPtr = str)
                    _utf8.GetBytes(strPtr, str.Length, buffer, length);

            buffer[length] = 0;

            return nativeData;
        }

        public object MarshalNativeToManaged(IntPtr nativeData) => MarshalFromNative(nativeData);
        public static unsafe string MarshalFromNative(IntPtr nativeData)
        {
            if (nativeData == IntPtr.Zero)
                return null;

            var tail = (byte*)nativeData;
            if (*tail == 0)
                return string.Empty;

            do { } while (*(++tail) != 0);

            return new string((sbyte*)nativeData, 0, (int)(tail - (byte*)nativeData), _utf8);
        }

        public void CleanUpManagedData(object managedObject) { }
        public void CleanUpNativeData(IntPtr nativeData)
        {
            if (nativeData == IntPtr.Zero)
                return;

            SQLiteNativeMethods.sqlite3_free(nativeData);
        }

        public int GetNativeDataSize() => -1;
    }

    class UTF8StringConstantMarshaler : ICustomMarshaler
    {
        static UTF8StringConstantMarshaler _instance = new UTF8StringConstantMarshaler();

        public static ICustomMarshaler GetInstance(string cookie = null) => _instance;

        public IntPtr MarshalManagedToNative(object managedObject) => IntPtr.Zero;

        public object MarshalNativeToManaged(IntPtr nativeData) => UTF8StringMarshaler.MarshalFromNative(nativeData);

        public void CleanUpManagedData(object managedObject) { }
        public void CleanUpNativeData(IntPtr nativeData) { }

        public int GetNativeDataSize() => -1;
    }
}
