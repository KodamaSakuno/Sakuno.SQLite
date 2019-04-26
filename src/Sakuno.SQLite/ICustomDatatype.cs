using System;

namespace Sakuno.SQLite
{
    public interface ICustomDatatype<T>
    {
        bool SupportInteger { get; }
        bool SupportFloat { get; }
        bool SupportText { get; }
        bool SupportBlob { get; }

        SQLiteDatatype DefaultDatatype { get; }

        T FromInteger(long value);
        T FromFloat(double value);
        T FromText(string value);
        T FromBlob(ReadOnlySpan<byte> value);

        long ToInteger(T value);
        double ToFloat(T value);
        string ToText(T value);
        ReadOnlyMemory<byte> ToBlob(T value);
    }
}
