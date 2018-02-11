using System;

namespace Sakuno.SQLite
{
    [Flags]
    public enum OpenDatabaseOptions
    {
        ReadOnly = 0x00000001,
        ReadWrite = 0x00000002,
        Create = 0x00000004,
        DeleteOnClose = 0x00000008,
        Exclusive = 0x00000010,
        AutoProxy = 0x00000020,
        Uri = 0x00000040,
        Memory = 0x00000080,
        MainDatabase = 0x00000100,
        TemporaryDatabase = 0x00000200,
        TransientDatabase = 0x00000400,
        MainJournal = 0x00000800,
        TemporaryJournal = 0x000010000,
        SubJournal = 0x00002000,
        MasterJournal = 0x00004000,
        NoMutex = 0x00008000,
        FullMutex = 0x00010000,
        SharedCache = 0x00020000,
        PrivateCache = 0x00040000,
        WAL = 0x00080000,
    }
}
