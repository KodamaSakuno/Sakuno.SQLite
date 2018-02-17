using System;
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

        public int RowsAffected { get; private set; }

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

        public void Execute()
        {
            foreach (var statement in _statements)
            {
                var resultCode = statement.Execute();

                switch (resultCode)
                {
                    case SQLiteResultCode.Done:
                        RowsAffected += _database.Changes;
                        break;

                    case SQLiteResultCode.Row:
                        break;

                    default:
                        throw new SQLiteException(resultCode);
                }

                statement.Reset();
            }
        }
        public T Execute<T>() => Execute<T>(0);
        public T Execute<T>(int column)
        {
            var result = default(T);

            foreach (var statement in _statements)
            {
                var resultCode = statement.Execute();

                switch (resultCode)
                {
                    case SQLiteResultCode.Done:
                        RowsAffected += _database.Changes;
                        break;

                    case SQLiteResultCode.Row:
                        result = statement.Get<T>(column);
                        break;

                    default:
                        throw new SQLiteException(resultCode);
                }

                statement.Reset();
            }

            return result;
        }

        public void Bind<T>(string parameter, T value)
        {
            if (Datatype.Of<T>.Bind == null)
                throw new NotSupportedException();

            foreach (var statement in _statements)
                statement.Bind(parameter, value);
        }

        public void Bind(string parameter, object value)
        {
            foreach (var statement in _statements)
                statement.Bind(parameter, value);
        }

        public void ClearBindings()
        {
            foreach (var statement in _statements)
                statement.ClearBindings();
        }
    }
}
