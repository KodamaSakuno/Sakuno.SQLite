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
                if (_statements.Length == 1)
                    return _statements[0].SQL;

                var array = new string[_statements.Length * 2 - 1];
                for (var i = 0; i < _statements.Length; i++)
                {
                    if (i > 0)
                        array[i * 2 - 1] = Environment.NewLine;

                    array[i * 2] = _statements[i].SQL;
                }

                return string.Concat(array);
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
            if (_statements.Length == 1)
                return _statements[0].GetExpandedSQL();

            var array = new string[_statements.Length * 2 - 1];
            for (var i = 0; i < _statements.Length; i++)
            {
                if (i > 0)
                    array[i * 2 - 1] = Environment.NewLine;

                array[i * 2] = _statements[i].GetExpandedSQL();
            }

            return string.Concat(array);
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
            if (Datatype.OfCustom<T>.FromValue != null)
            {
                Bind(parameter, value, Datatype.OfCustom<T>.DefaultDatatype);
                return;
            }

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

        public void Bind<T>(string parameter, T value, SQLiteDatatype underlyingDatatype)
        {
            switch (underlyingDatatype)
            {
                case SQLiteDatatype.Integer:
                    Bind<T, long>(parameter, value);
                    break;

                case SQLiteDatatype.Float:
                    Bind<T, double>(parameter, value);
                    break;

                case SQLiteDatatype.Text:
                    Bind<T, string>(parameter, value);
                    break;

                case SQLiteDatatype.Blob:
                    Bind<T, ReadOnlyMemory<byte>>(parameter, value);
                    break;

                default: throw new ArgumentException(nameof(underlyingDatatype));
            }

        }
        void Bind<T, TUnderlying>(string parameter, T value)
        {
            foreach (var statement in _statements)
                statement.Bind(parameter, Datatype.OfCustom<T>.As<TUnderlying>.Get(value));
        }

        public void ClearBindings()
        {
            foreach (var statement in _statements)
                statement.ClearBindings();
        }
    }
}
