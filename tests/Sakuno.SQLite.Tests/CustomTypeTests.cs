using System;
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
            public DateTimeOffset FromBlob(byte[] value) => throw new NotSupportedException();
            public byte[] ToBlob(DateTimeOffset value) => throw new NotSupportedException();
            public double ToFloat(DateTimeOffset value) => throw new NotSupportedException();
        }
    }
}
