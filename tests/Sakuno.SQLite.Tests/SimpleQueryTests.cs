﻿using System;
using Xunit;

namespace Sakuno.SQLite.Tests
{
    public class SimpleQueryTests : IClassFixture<MemoryDatabaseFixture>
    {
        SQLiteDatabase _database;

        byte[] _bytes = new byte[] { 0, 1, 2, 3 };

        public SimpleQueryTests(MemoryDatabaseFixture fixture)
        {
            _database = fixture.Database;
        }

        [Fact]
        public void SelectSingleColumn()
        {
            Assert.Equal(1234, _database.Execute<int>("SELECT 1234;"));
            Assert.Equal(1234, _database.Execute<int?>("SELECT 1234;"));

            Assert.Equal(9223372036854775807, _database.Execute<long>("SELECT 9223372036854775807;"));
            Assert.Equal(9223372036854775807, _database.Execute<long?>("SELECT 9223372036854775807;"));

            Assert.Equal(13.14, _database.Execute<double>("SELECT 13.14;"));
            Assert.Equal(13.14, _database.Execute<double?>("SELECT 13.14;"));

            Assert.Equal(_bytes, _database.Execute<byte[]>("SELECT x'00010203';"));

            var span = _database.Execute<BlobInfo>("SELECT x'00010203';").GetSpan();
            Assert.True(MemoryExtensions.SequenceEqual(_bytes, span));

            Assert.Equal("test", _database.Execute<string>("SELECT 'test';"));
        }

        [Fact]
        public void SelectMultipleColumns()
        {
            using var query = _database.CreateQuery("SELECT 1234, 9223372036854775807, 13.14, x'00010203', 'test';");

            Assert.Equal(1234, query.Execute<int>(0));
            Assert.Equal(1234, query.Execute<int?>(0));

            Assert.Equal(9223372036854775807, query.Execute<long>(1));
            Assert.Equal(9223372036854775807, query.Execute<long?>(1));

            Assert.Equal(13.14, query.Execute<double>(2));
            Assert.Equal(13.14, query.Execute<double?>(2));

            Assert.Equal(_bytes, query.Execute<byte[]>(3));

            var span = query.Execute<BlobInfo>(3).GetSpan();
            Assert.True(MemoryExtensions.SequenceEqual(_bytes, span));

            Assert.Equal("test", query.Execute<string>(4));
        }

        [Fact]
        public void Modification()
        {
            _database.Execute("CREATE TABLE IF NOT EXISTS simple_statment_test(id INTEGER PRIMARY KEY);");

            _database.Execute("INSERT INTO simple_statment_test VALUES(0);");

            Assert.Equal(1, _database.Changes);
            Assert.Equal(1, _database.TotalChanges);

            _database.Execute("INSERT INTO simple_statment_test VALUES(1);");
            _database.Execute("INSERT INTO simple_statment_test VALUES(2);");

            Assert.Equal(1, _database.Changes);
            Assert.Equal(3, _database.TotalChanges);
        }

        [Fact]
        public void SelectSingleColumnNonGeneric()
        {
            Assert.Null(_database.Execute<object>("SELECT NULL;"));

            Assert.Equal(1234L, _database.Execute<object>("SELECT 1234;"));

            Assert.Equal(9223372036854775807, _database.Execute<object>("SELECT 9223372036854775807;"));

            Assert.Equal(13.14, _database.Execute<object>("SELECT 13.14;"));

            Assert.Equal(_bytes, _database.Execute<object>("SELECT x'00010203';"));

            Assert.Equal("test", _database.Execute<object>("SELECT 'test';"));
        }
    }
}
