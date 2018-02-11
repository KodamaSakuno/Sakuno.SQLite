using System;
using System.Reflection;

namespace Sakuno.SQLite
{
    static class Datatype
    {
        public static void Initialize()
        {
            Of<int>.Get = SQLiteNativeMethods.sqlite3_column_int;
            Of<long>.Get = SQLiteNativeMethods.sqlite3_column_int64;
            Of<double>.Get = SQLiteNativeMethods.sqlite3_column_double;
            Of<string>.Get = SQLiteNativeMethods.sqlite3_column_text;
            Of<bool>.Get = GetBoolean;
            Of<short>.Get = GetInt16;
            Of<byte[]>.Get = GetBytes;
        }

        static bool GetBoolean(SQLiteStatementHandle handle, int column) => Of<int>.Get(handle, column) != 0;
        static short GetInt16(SQLiteStatementHandle handle, int column) => (short)Of<int>.Get(handle, column);

        static unsafe byte[] GetBytes(SQLiteStatementHandle handle, int column)
        {
            var bytes = SQLiteNativeMethods.sqlite3_column_blob(handle, column);
            if (bytes == IntPtr.Zero)
                return null;

            var length = SQLiteNativeMethods.sqlite3_column_bytes(handle, column);
            var result = new byte[length];

            fixed (byte* buffer = result)
                UnsafeOperations.CopyMemory((void*)bytes, buffer, length);

            return result;
        }

        public static class Of<T>
        {
            public static Func<SQLiteStatementHandle, int, T> Get;

            static Of()
            {
                var type = typeof(T);
                var field = default(FieldInfo);

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var typeArgument = type.GenericTypeArguments[0];
                    var cacheType = typeof(OfNullable<>).MakeGenericType(typeArgument);

                    field = cacheType.GetField(nameof(Get));
                }
                else if (type.IsEnum)
                {
                    var underlyingType = type.GetEnumUnderlyingType();
                    var cacheType = typeof(OfCast<,>).MakeGenericType(type, underlyingType);

                    field = cacheType.GetField(nameof(Get));
                }

                Get = (Func<SQLiteStatementHandle, int, T>)field?.GetValue(null);
            }
        }
        static class OfNullable<T> where T : struct
        {
            public static readonly Func<SQLiteStatementHandle, int, T?> Get = GetCore;

            static T? GetCore(SQLiteStatementHandle handle, int column)
            {
                if (SQLiteNativeMethods.sqlite3_column_type(handle, column) == SQLiteDatatype.Null)
                    return default;

                return Of<T>.Get(handle, column);
            }
        }
        static class OfCast<T, TUnderlying> where T : struct where TUnderlying : struct
        {
            public static readonly Func<SQLiteStatementHandle, int, T> Get = GetCore;

            static T GetCore(SQLiteStatementHandle handle, int column) =>
                UnsafeOperations.As<TUnderlying, T>(Of<TUnderlying>.Get(handle, column));
        }
    }
}
