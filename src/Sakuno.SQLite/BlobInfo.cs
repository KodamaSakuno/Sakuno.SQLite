using System;

namespace Sakuno.SQLite
{
    public readonly unsafe struct BlobInfo
    {
        public byte* Pointer { get; }
        public int Length { get; }

        internal BlobInfo(byte* pointer, int length)
        {
            Pointer = pointer;
            Length = length;
        }

        public unsafe ReadOnlySpan<byte> GetSpan() => new ReadOnlySpan<byte>(Pointer, Length);
    }
}
