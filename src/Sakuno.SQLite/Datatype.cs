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
            Of<byte>.Get = GetByte;
            Of<short>.Get = GetInt16;
            Of<byte[]>.Get = GetBytes;
            Of<float>.Get = GetSingleFloat;
            Of<sbyte>.Get = OfCast<sbyte, byte>.Get;
            Of<ushort>.Get = OfCast<ushort, short>.Get;
            Of<uint>.Get = OfCast<uint, int>.Get;
            Of<ulong>.Get = OfCast<ulong, long>.Get;

            Of<object>.Get = GetNonGeneric;

            Of<int>.Bind = SQLiteNativeMethods.sqlite3_bind_int;
            Of<long>.Bind = SQLiteNativeMethods.sqlite3_bind_int64;
            Of<double>.Bind = SQLiteNativeMethods.sqlite3_bind_double;
            Of<string>.Bind = BindText;
            Of<bool>.Bind = BindBoolean;
            Of<byte>.Bind = BindByte;
            Of<short>.Bind = BindInt16;
            Of<byte[]>.Bind = BindBytes;
            Of<float>.Bind = BindSingleFloat;
            Of<sbyte>.Bind = OfCast<sbyte, byte>.Bind;
            Of<ushort>.Bind = OfCast<ushort, short>.Bind;
            Of<uint>.Bind = OfCast<uint, int>.Bind;
            Of<ulong>.Bind = OfCast<ulong, long>.Bind;

            Of<object>.Bind = BindNonGeneric;
        }

        static bool GetBoolean(SQLiteStatementHandle handle, int column) => Of<int>.Get(handle, column) != 0;
        static byte GetByte(SQLiteStatementHandle handle, int column) => (byte)Of<int>.Get(handle, column);
        static short GetInt16(SQLiteStatementHandle handle, int column) => (short)Of<int>.Get(handle, column);
        static float GetSingleFloat(SQLiteStatementHandle handle, int column) => (float)Of<double>.Get(handle, column);

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

        static object GetNonGeneric(SQLiteStatementHandle handle, int column)
        {
            var type = SQLiteNativeMethods.sqlite3_column_type(handle, column);

            switch (type)
            {
                case SQLiteDatatype.Integer:
                    return SQLiteNativeMethods.sqlite3_column_int64(handle, column);

                case SQLiteDatatype.Float:
                    return SQLiteNativeMethods.sqlite3_column_double(handle, column);

                case SQLiteDatatype.Text:
                    return SQLiteNativeMethods.sqlite3_column_text(handle, column);

                case SQLiteDatatype.Blob:
                    return GetBytes(handle, column);

                case SQLiteDatatype.Null:
                    return null;
            }

            throw new InvalidOperationException();
        }

        static SQLiteResultCode BindText(SQLiteStatementHandle handle, int index, string value)
        {
            var marshaler = UTF8StringMarshaler.Instance;
            var nativeData = marshaler.MarshalManagedToNative(value, out var length);

            try
            {
                return SQLiteNativeMethods.sqlite3_bind_text(handle, index, nativeData, length, (IntPtr)(-1));
            }
            finally
            {
                marshaler.CleanUpNativeData(nativeData);
            }
        }

        static SQLiteResultCode BindBoolean(SQLiteStatementHandle handle, int column, bool value) =>
            Of<int>.Bind(handle, column, value ? 1 : 0);
        static SQLiteResultCode BindByte(SQLiteStatementHandle handle, int column, byte value) =>
            Of<int>.Bind(handle, column, value);
        static SQLiteResultCode BindInt16(SQLiteStatementHandle handle, int column, short value) =>
            Of<int>.Bind(handle, column, value);
        static SQLiteResultCode BindSingleFloat(SQLiteStatementHandle handle, int column, float value) =>
            Of<double>.Bind(handle, column, value);
        static SQLiteResultCode BindBytes(SQLiteStatementHandle handle, int index, byte[] value) =>
            SQLiteNativeMethods.sqlite3_bind_blob(handle, index, value, value.Length, IntPtr.Zero);

        static SQLiteResultCode BindNonGeneric(SQLiteStatementHandle handle, int index, object value)
        {
            var typeCode = Type.GetTypeCode(value.GetType());

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return BindBoolean(handle, index, (bool)value);

                case TypeCode.Single:
                case TypeCode.Double:
                    return Of<double>.Bind(handle, index, Convert.ToDouble(value));

                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return Of<long>.Bind(handle, index, Convert.ToInt64(value));

                case TypeCode.String:
                    return BindText(handle, index, (string)value);
            }

            if (value is byte[] bytes)
                return BindBytes(handle, index, bytes);

            throw new NotSupportedException();
        }

        public static class Of<T>
        {
            public static Func<SQLiteStatementHandle, int, T> Get;

            public static Func<SQLiteStatementHandle, int, T, SQLiteResultCode> Bind;

            static Of()
            {
                var type = typeof(T);

                var getField = default(FieldInfo);
                var bindField = default(FieldInfo);

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var typeArgument = type.GenericTypeArguments[0];
                    var cacheType = typeof(OfNullable<>).MakeGenericType(typeArgument);

                    getField = cacheType.GetField(nameof(Get));

                    bindField = cacheType.GetField(nameof(Bind));
                }
                else if (type.IsEnum)
                {
                    var underlyingType = type.GetEnumUnderlyingType();
                    var cacheType = typeof(OfCast<,>).MakeGenericType(type, underlyingType);

                    getField = cacheType.GetField(nameof(Get));

                    bindField = cacheType.GetField(nameof(Bind));
                }

                Get = (Func<SQLiteStatementHandle, int, T>)getField?.GetValue(null);

                Bind = (Func<SQLiteStatementHandle, int, T, SQLiteResultCode>)bindField?.GetValue(null);
            }
        }
        static class OfNullable<T> where T : struct
        {
            public static readonly Func<SQLiteStatementHandle, int, T?> Get = GetCore;

            public static readonly Func<SQLiteStatementHandle, int, T?, SQLiteResultCode> Bind = BindCore;

            static T? GetCore(SQLiteStatementHandle handle, int column)
            {
                if (SQLiteNativeMethods.sqlite3_column_type(handle, column) == SQLiteDatatype.Null)
                    return default;

                return Of<T>.Get(handle, column);
            }

            static SQLiteResultCode BindCore(SQLiteStatementHandle handle, int column, T? value)
            {
                if (SQLiteNativeMethods.sqlite3_column_type(handle, column) == SQLiteDatatype.Null)
                    return default;

                if (!value.HasValue)
                    return SQLiteNativeMethods.sqlite3_bind_null(handle, column);

                return Of<T>.Bind(handle, column, value.Value);
            }
        }
        static class OfCast<T, TUnderlying> where T : struct where TUnderlying : struct
        {
            public static readonly Func<SQLiteStatementHandle, int, T> Get = GetCore;

            public static readonly Func<SQLiteStatementHandle, int, T, SQLiteResultCode> Bind = BindCore;

            static T GetCore(SQLiteStatementHandle handle, int column) =>
                UnsafeOperations.As<TUnderlying, T>(Of<TUnderlying>.Get(handle, column));

            static SQLiteResultCode BindCore(SQLiteStatementHandle handle, int column, T value) =>
                Of<TUnderlying>.Bind(handle, column, UnsafeOperations.As<T, TUnderlying>(value));
        }
    }
}
