﻿using System;
using Xunit;

namespace Sakuno.SQLite.Tests
{
    public class ParameterBindingTests : IClassFixture<MemoryDatabaseFixture>
    {
        SQLiteDatabase _database;

        byte[] _bytes = new byte[] { 0, 1, 2, 3 };

        public ParameterBindingTests(MemoryDatabaseFixture fixture)
        {
            _database = fixture.Database;
        }

        [Fact]
        public void SimpleType()
        {
            using var query = _database.CreateQuery("SELECT @first, @second, @third, @fourth, @fifth;");

            query.Bind("@first", 1234);
            query.Bind("@second", 9223372036854775807);
            query.Bind("@third", 13.14);
            query.Bind("@fourth", _bytes);
            query.Bind("@fifth", "test");

            Assert.Equal("SELECT 1234, 9223372036854775807, 13.14, x'00010203', 'test';", query.GetExpandedSQL());

            Assert.Equal(1234, query.Execute<int>(0));
            Assert.Equal(1234, query.Execute<int?>(0));
            Assert.Equal(9223372036854775807, query.Execute<long>(1));
            Assert.Equal(9223372036854775807, query.Execute<long?>(1));
            Assert.Equal(13.14, query.Execute<double>(2));
            Assert.Equal(13.14, query.Execute<double?>(2));
            Assert.Equal(_bytes, query.Execute<byte[]>(3));
            Assert.Equal("test", query.Execute<string>(4));

            var span = query.Execute<BlobInfo>(3).GetSpan();
            Assert.True(MemoryExtensions.SequenceEqual(_bytes, span));
        }

        [Fact]
        public void NonGeneric()
        {
            using var query = _database.CreateQuery("SELECT @first, @second, @third, @fourth, @fifth;");

            query.Bind("@first", (object)1234);
            query.Bind("@second", (object)9223372036854775807);
            query.Bind("@third", (object)13.14);
            query.Bind("@fourth", (object)_bytes);
            query.Bind("@fifth", (object)"test");

            Assert.Equal("SELECT 1234, 9223372036854775807, 13.14, x'00010203', 'test';", query.GetExpandedSQL());

            Assert.Equal(1234, query.Execute<int>(0));
            Assert.Equal(9223372036854775807, query.Execute<long>(1));
            Assert.Equal(13.14, query.Execute<double>(2));
            Assert.Equal(_bytes, query.Execute<byte[]>(3));
            Assert.Equal("test", query.Execute<string>(4));

            var span = query.Execute<BlobInfo>(3).GetSpan();
            Assert.True(MemoryExtensions.SequenceEqual(_bytes, span));
        }
    }
}
