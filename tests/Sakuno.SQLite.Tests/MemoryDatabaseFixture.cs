using System;

namespace Sakuno.SQLite.Tests
{
    public class MemoryDatabaseFixture : IDisposable
    {
        public SQLiteDatabase Database { get; }

        public MemoryDatabaseFixture()
        {
            Database = SQLiteDatabase.OpenMemoryDatabase();
        }

        public void Dispose() => Database.Dispose();
    }
}
