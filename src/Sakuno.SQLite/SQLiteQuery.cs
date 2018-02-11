using System.Collections.Generic;

namespace Sakuno.SQLite
{
    public class SQLiteQuery : SQLiteObject
    {
        SQLiteDatabase _database;

        SQLiteStatement[] _statements;

        public string SQL
        {
            get
            {
                var builder = StringBuilderCache.Acquire();

                foreach (var statement in _statements)
                {
                    if (builder.Length > 0)
                        builder.AppendLine();

                    builder.Append(statement.SQL);
                }

                return builder.ToString();
            }
        }

        internal SQLiteQuery(SQLiteDatabase database, List<SQLiteStatement> statements)
        {
            _database = database;
            _statements = statements.ToArray();
        }

        protected override void DisposeManagedResource()
        {
            foreach (var statement in _statements)
                statement.Dispose();

            _statements = null;
        }

        public string GetExpandedSQL()
        {
            var builder = StringBuilderCache.Acquire();

            foreach (var statement in _statements)
            {
                if (builder.Length > 0)
                    builder.AppendLine();

                builder.Append(statement.GetExpandedSQL());
            }

            return builder.ToString();
        }
    }
}
