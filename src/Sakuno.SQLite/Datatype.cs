using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sakuno.SQLite
{
    static class Datatype
    {
        public static void Initialize()
        {
            Of<int>.FromStatement = SQLiteNativeMethods.sqlite3_column_int;
            Of<long>.FromStatement = SQLiteNativeMethods.sqlite3_column_int64;
            Of<double>.FromStatement = SQLiteNativeMethods.sqlite3_column_double;
            Of<string>.FromStatement = SQLiteNativeMethods.sqlite3_column_text;
            Of<bool>.FromStatement = GetBoolean;
            Of<byte>.FromStatement = GetByte;
            Of<short>.FromStatement = GetInt16;
            Of<byte[]>.FromStatement = GetBytes;
            Of<ReadOnlyMemory<byte>>.FromStatement = GetReadOnlyMemory;
            Of<float>.FromStatement = GetSingleFloat;
            Of<sbyte>.FromStatement = OfCast<sbyte, byte>.FromStatement;
            Of<ushort>.FromStatement = OfCast<ushort, short>.FromStatement;
            Of<uint>.FromStatement = OfCast<uint, int>.FromStatement;
            Of<ulong>.FromStatement = OfCast<ulong, long>.FromStatement;

            Of<object>.FromStatement = GetNonGeneric;

            Of<int>.FromValue = SQLiteNativeMethods.sqlite3_value_int;
            Of<long>.FromValue = SQLiteNativeMethods.sqlite3_value_int64;
            Of<double>.FromValue = SQLiteNativeMethods.sqlite3_value_double;
            Of<string>.FromValue = SQLiteNativeMethods.sqlite3_value_text;
            Of<bool>.FromValue = GetBoolean;
            Of<byte>.FromValue = GetByte;
            Of<short>.FromValue = GetInt16;
            Of<float>.FromValue = GetSingleFloat;
            Of<byte[]>.FromValue = GetBytes;
            Of<ReadOnlyMemory<byte>>.FromValue = GetReadOnlyMemory;
            Of<sbyte>.FromValue = OfCast<sbyte, byte>.FromValue;
            Of<ushort>.FromValue = OfCast<ushort, short>.FromValue;
            Of<uint>.FromValue = OfCast<uint, int>.FromValue;
            Of<ulong>.FromValue = OfCast<ulong, long>.FromValue;

            Of<int>.Bind = SQLiteNativeMethods.sqlite3_bind_int;
            Of<long>.Bind = SQLiteNativeMethods.sqlite3_bind_int64;
            Of<double>.Bind = SQLiteNativeMethods.sqlite3_bind_double;
            Of<string>.Bind = BindText;
            Of<bool>.Bind = BindBoolean;
            Of<byte>.Bind = BindByte;
            Of<short>.Bind = BindInt16;
            Of<byte[]>.Bind = BindBytes;
            Of<ReadOnlyMemory<byte>>.Bind = BindReadOnlyMemory;
            Of<float>.Bind = BindSingleFloat;
            Of<sbyte>.Bind = OfCast<sbyte, byte>.Bind;
            Of<ushort>.Bind = OfCast<ushort, short>.Bind;
            Of<uint>.Bind = OfCast<uint, int>.Bind;
            Of<ulong>.Bind = OfCast<ulong, long>.Bind;

            Of<object>.Bind = BindNonGeneric;
        }

        static bool GetBoolean(SQLiteStatementHandle handle, int column) => Of<int>.FromStatement(handle, column) != 0;
        static byte GetByte(SQLiteStatementHandle handle, int column) => (byte)Of<int>.FromStatement(handle, column);
        static short GetInt16(SQLiteStatementHandle handle, int column) => (short)Of<int>.FromStatement(handle, column);
        static float GetSingleFloat(SQLiteStatementHandle handle, int column) => (float)Of<double>.FromStatement(handle, column);

        static unsafe byte[] GetBytes(SQLiteStatementHandle handle, int column)
        {
            var bytes = SQLiteNativeMethods.sqlite3_column_blob(handle, column);
            if (bytes == IntPtr.Zero)
                return null;

            var length = SQLiteNativeMethods.sqlite3_column_bytes(handle, column);
            var result = new byte[length];

            fixed (byte* buffer = result)
                Unsafe.CopyBlock(buffer, (void*)bytes, (uint)length);

            return result;
        }
        static unsafe ReadOnlyMemory<byte> GetReadOnlyMemory(SQLiteStatementHandle handle, int column) =>
            Of<byte[]>.FromStatement(handle, column);

        static object GetNonGeneric(SQLiteStatementHandle handle, int column) =>
            SQLiteNativeMethods.sqlite3_column_type(handle, column) switch
            {
                SQLiteDatatype.Integer => SQLiteNativeMethods.sqlite3_column_int64(handle, column),
                SQLiteDatatype.Float => SQLiteNativeMethods.sqlite3_column_double(handle, column),
                SQLiteDatatype.Text => SQLiteNativeMethods.sqlite3_column_text(handle, column),
                SQLiteDatatype.Blob => GetBytes(handle, column),
                SQLiteDatatype.Null => (object)null,
                _ => throw new InvalidOperationException(),
            };

        static bool GetBoolean(SQLiteValueHandle handle) => Of<int>.FromValue(handle) != 0;
        static byte GetByte(SQLiteValueHandle handle) => (byte)Of<int>.FromValue(handle);
        static short GetInt16(SQLiteValueHandle handle) => (short)Of<int>.FromValue(handle);
        static float GetSingleFloat(SQLiteValueHandle handle) => (float)Of<double>.FromValue(handle);

        static unsafe byte[] GetBytes(SQLiteValueHandle handle)
        {
            var bytes = SQLiteNativeMethods.sqlite3_value_blob(handle);
            if (bytes == IntPtr.Zero)
                return null;

            var length = SQLiteNativeMethods.sqlite3_value_bytes(handle);
            var result = new byte[length];

            fixed (byte* buffer = result)
                Unsafe.CopyBlock(buffer, (void*)bytes, (uint)length);

            return result;
        }
        static unsafe ReadOnlyMemory<byte> GetReadOnlyMemory(SQLiteValueHandle handle) =>
            Of<byte[]>.FromValue(handle);

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
        static SQLiteResultCode BindReadOnlyMemory(SQLiteStatementHandle handle, int index, ReadOnlyMemory<byte> value) =>
            SQLiteNativeMethods.sqlite3_bind_blob(handle, index, MemoryMarshal.GetReference(value.Span), value.Length, IntPtr.Zero);

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
            public static Func<SQLiteStatementHandle, int, T> FromStatement;
            public static Func<SQLiteValueHandle, T> FromValue;

            public static Func<SQLiteStatementHandle, int, T, SQLiteResultCode> Bind;

            static Of()
            {
                var type = typeof(T);

                var fromStatementField = default(FieldInfo);
                var fromValueField = default(FieldInfo);

                var bindField = default(FieldInfo);

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var typeArgument = type.GenericTypeArguments[0];
                    var cacheType = typeof(OfNullable<>).MakeGenericType(typeArgument);

                    fromStatementField = cacheType.GetField(nameof(FromStatement));
                    fromValueField = cacheType.GetField(nameof(FromValue));

                    bindField = cacheType.GetField(nameof(Bind));
                }
                else if (type.IsEnum)
                {
                    var underlyingType = type.GetEnumUnderlyingType();
                    var cacheType = typeof(OfCast<,>).MakeGenericType(type, underlyingType);

                    fromStatementField = cacheType.GetField(nameof(FromStatement));
                    fromValueField = cacheType.GetField(nameof(FromValue));

                    bindField = cacheType.GetField(nameof(Bind));
                }

                FromStatement = (Func<SQLiteStatementHandle, int, T>)fromStatementField?.GetValue(null);
                FromValue = (Func<SQLiteValueHandle, T>)fromValueField?.GetValue(null);

                Bind = (Func<SQLiteStatementHandle, int, T, SQLiteResultCode>)bindField?.GetValue(null);
            }
        }
        static class OfNullable<T> where T : struct
        {
            public static readonly Func<SQLiteStatementHandle, int, T?> FromStatement = FromStatementCore;
            public static readonly Func<SQLiteValueHandle, T?> FromValue = FromValueCore;

            public static readonly Func<SQLiteStatementHandle, int, T?, SQLiteResultCode> Bind = BindCore;

            static T? FromStatementCore(SQLiteStatementHandle handle, int column)
            {
                if (SQLiteNativeMethods.sqlite3_column_type(handle, column) == SQLiteDatatype.Null)
                    return default;

                return Of<T>.FromStatement(handle, column);
            }
            static T? FromValueCore(SQLiteValueHandle handle)
            {
                if (SQLiteNativeMethods.sqlite3_value_type(handle) == SQLiteDatatype.Null)
                    return default;

                return Of<T>.FromValue(handle);
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
            public static readonly Func<SQLiteStatementHandle, int, T> FromStatement = FromStatementCore;
            public static readonly Func<SQLiteValueHandle, T> FromValue = FromValueCore;

            public static readonly Func<SQLiteStatementHandle, int, T, SQLiteResultCode> Bind = BindCore;

            static T FromStatementCore(SQLiteStatementHandle handle, int column)
            {
                var value = Of<TUnderlying>.FromStatement(handle, column);
                return Unsafe.As<TUnderlying, T>(ref value);
            }

            static T FromValueCore(SQLiteValueHandle handle)
            {
                var value = Of<TUnderlying>.FromValue(handle);
                return Unsafe.As<TUnderlying, T>(ref value);
            }

            static SQLiteResultCode BindCore(SQLiteStatementHandle handle, int column, T value) =>
                Of<TUnderlying>.Bind(handle, column, Unsafe.As<T, TUnderlying>(ref value));
        }

        public static class OfCustom<T>
        {
            public static Func<SQLiteValue, T> FromValue;

            static Func<long, T> _fromInteger;
            static Func<double, T> _fromFloat;
            static Func<string, T> _fromText;
            static Func<ReadOnlyMemory<byte>, T> _fromBlob;

            public static SQLiteDatatype DefaultDatatype;

            static OfCustom()
            {
                var type = typeof(T);

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var typeArgument = type.GenericTypeArguments[0];

                    var predefinedTypeCache = typeof(Of<>).MakeGenericType(typeArgument);
                    if (predefinedTypeCache.GetField(nameof(FromValue)).GetValue(null) != null)
                        return;

                    var nullableCacheType = typeof(OfCustomNullable<>).MakeGenericType(typeArgument);
                    var field = nullableCacheType.GetField(nameof(FromValue));

                    FromValue = (Func<SQLiteValue, T>)field.GetValue(null);
                }
            }

            public static void Register(ICustomDatatype<T> datatype)
            {
                if (datatype.SupportInteger)
                {
                    _fromInteger = datatype.FromInteger;
                    As<long>.Get = datatype.ToInteger;
                }
                if (datatype.SupportFloat)
                {
                    _fromFloat = datatype.FromFloat;
                    As<double>.Get = datatype.ToFloat;
                }
                if (datatype.SupportText)
                {
                    _fromText = datatype.FromText;
                    As<string>.Get = datatype.ToText;
                }
                if (datatype.SupportBlob)
                {
                    _fromBlob = datatype.FromBlob;
                    As<ReadOnlyMemory<byte>>.Get = datatype.ToBlob;
                }

                DefaultDatatype = datatype.DefaultDatatype;

                FromValue = FromValueCore;
            }

            static T FromValueCore(SQLiteValue value) => value.Type switch
            {
                SQLiteDatatype.Integer => _fromInteger(value.Get<long>()),
                SQLiteDatatype.Float => _fromFloat(value.Get<double>()),
                SQLiteDatatype.Text => _fromText(value.Get<string>()),
                SQLiteDatatype.Blob => _fromBlob(value.Get<ReadOnlyMemory<byte>>()),
                SQLiteDatatype.Null => default,
                _ => throw new InvalidOperationException(),
            };

            public static class As<TUnderlying>
            {
                public static Func<T, TUnderlying> Get;
            }
        }
        static class OfCustomNullable<T> where T : struct
        {
            public static readonly Func<SQLiteValue, T?> FromValue = FromValueCore;

            static T? FromValueCore(SQLiteValue value)
            {
                if (value.Type == SQLiteDatatype.Null)
                    return default;

                return OfCustom<T>.FromValue(value);
            }
        }
    }
}
