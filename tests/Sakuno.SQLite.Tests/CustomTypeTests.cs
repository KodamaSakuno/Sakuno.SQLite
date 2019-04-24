using System;
using System.Runtime.InteropServices;
using Xunit;

namespace Sakuno.SQLite.Tests
{
    public class CustomTypeTests : IClassFixture<MemoryDatabaseFixture>
    {
        SQLiteDatabase _database;

        public CustomTypeTests(MemoryDatabaseFixture fixture)
        {
            _database = fixture.Database;

            SQLiteDatabase.RegisterDatatype(new DateTimeOffsetDatatype());
            SQLiteDatabase.RegisterDatatype(new GuidDatatype());
        }

        [Fact]
        public void DateTimeOffsetFromTimestamp()
        {
            using var query = _database.CreateQuery("SELECT @timestamp;");

            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            query.Bind("@timestamp", timestamp);

            var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);

            Assert.Equal(dateTime, query.Execute<DateTimeOffset>());
            Assert.Equal(dateTime, query.Execute<DateTimeOffset?>());
        }

        [Fact]
        public void DateTimeOffsetFromNull()
        {
            using var query = _database.CreateQuery("SELECT NULL;");

            Assert.Equal(DateTimeOffset.MinValue, query.Execute<DateTimeOffset>());
            Assert.NotEqual(DateTimeOffset.MinValue, query.Execute<DateTimeOffset?>());
            Assert.Equal(default, query.Execute<DateTimeOffset?>());
        }

        [Fact]
        public void BindWithDateTimeOffset()
        {
            using var query = _database.CreateQuery("SELECT @timestamp;");

            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);

            query.Bind("@timestamp", dateTime);

            Assert.Equal(dateTime, query.Execute<DateTimeOffset>());
            Assert.Equal(dateTime, query.Execute<DateTimeOffset?>());
        }

        [Fact]
        public void GuidFromText()
        {
            using var query = _database.CreateQuery("SELECT @guid;");

            var guid = Guid.NewGuid();

            query.Bind("@guid", guid.ToString());

            Assert.Equal(guid, query.Execute<Guid>());
            Assert.Equal(guid, query.Execute<Guid?>());
        }
        [Fact]
        public void GuidFromBlob()
        {
            using var query = _database.CreateQuery("SELECT @guid;");

            var guid = Guid.NewGuid();

            query.Bind("@guid", guid.ToByteArray());

            Assert.Equal(guid, query.Execute<Guid>());
            Assert.Equal(guid, query.Execute<Guid?>());
        }

        [Fact]
        public void GuidFromNull()
        {
            using var query = _database.CreateQuery("SELECT NULL;");

            Assert.Equal(Guid.Empty, query.Execute<Guid>());
            Assert.NotEqual(Guid.Empty, query.Execute<Guid?>());
            Assert.Equal(default, query.Execute<Guid?>());
        }

        [Fact]
        public void BindWithGuid()
        {
            using var query = _database.CreateQuery("SELECT @guid;");

            var guid = Guid.NewGuid();
            var guidBlob = guid.ToByteArray();

            query.Bind("@guid", guid);

            Assert.Equal(guid, query.Execute<Guid>());
            Assert.Equal(guid, query.Execute<Guid?>());
            Assert.Equal(guidBlob, query.Execute<byte[]>());

            Assert.Equal(guidBlob, MemoryMarshal.ToEnumerable(query.Execute<ReadOnlyMemory<byte>>()));
            Assert.Equal(guidBlob, MemoryMarshal.ToEnumerable(query.Execute<ReadOnlyMemory<byte>?>().GetValueOrDefault()));
        }

        class DateTimeOffsetDatatype : ICustomDatatype<DateTimeOffset>
        {
            public bool SupportInteger => true;
            public bool SupportText => true;
            public bool SupportFloat => false;
            public bool SupportBlob => false;

            public SQLiteDatatype DefaultDatatype => SQLiteDatatype.Integer;

            public DateTimeOffset FromInteger(long value) => DateTimeOffset.FromUnixTimeSeconds(value);
            public DateTimeOffset FromText(string value) => DateTimeOffset.Parse(value);

            public long ToInteger(DateTimeOffset value) => value.ToUnixTimeSeconds();
            public string ToText(DateTimeOffset value) => value.ToString();

            public DateTimeOffset FromFloat(double value) => throw new NotSupportedException();
            public DateTimeOffset FromBlob(ReadOnlyMemory<byte> value) => throw new NotSupportedException();
            public ReadOnlyMemory<byte> ToBlob(DateTimeOffset value) => throw new NotSupportedException();
            public double ToFloat(DateTimeOffset value) => throw new NotSupportedException();
        }
        class GuidDatatype : ICustomDatatype<Guid>
        {
            public bool SupportInteger => false;
            public bool SupportText => true;
            public bool SupportFloat => false;
            public bool SupportBlob => true;

            public SQLiteDatatype DefaultDatatype => SQLiteDatatype.Blob;

#if !NET461
            public Guid FromBlob(ReadOnlyMemory<byte> value) => new Guid(value.Span);
#else
            public Guid FromBlob(ReadOnlyMemory<byte> value) => new Guid(value.ToArray());
#endif
            public Guid FromText(string value) => Guid.Parse(value);

            public ReadOnlyMemory<byte> ToBlob(Guid value) => value.ToByteArray();
            public string ToText(Guid value) => value.ToString();

            public Guid FromInteger(long value) => throw new NotSupportedException();
            public Guid FromFloat(double value) => throw new NotSupportedException();
            public long ToInteger(Guid value) => throw new NotSupportedException();
            public double ToFloat(Guid value) => throw new NotSupportedException();
        }
    }
}
