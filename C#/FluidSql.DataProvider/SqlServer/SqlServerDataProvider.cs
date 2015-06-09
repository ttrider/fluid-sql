using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using TTRider.FluidSql.Providers;
using TTRider.FluidSql.Providers.SqlServer;

namespace TTRider.FluidSql.DataProvider.SqlServer
{
    public class SqlServerDataProvider : IDataProvider
    {
        static readonly IProvider QueryProvider = new SqlServerProvider();

        public IDataResponse ProcessRequest(IDataRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (request.Statement == null) throw new ArgumentException("request.Statement");
            if (request.ConnectionString == null) throw new ArgumentException("request.ConnectionString");

            var connection = QueryProvider.GetConnection(request.ConnectionString);

            connection.Open();

            var command = QueryProvider.GetCommand(request.Statement);
            command.Connection = connection;

            return DataResponse.GetResponse(request, command);
        }

        class DataRecordEnumerable : IEnumerable<object[]>
        {
            private readonly IDataRequest request;
            private readonly IDataReader reader;
            private readonly Action onCompletedAction;

            public DataRecordEnumerable(IDataRequest request, IDataReader reader, Action onCompletedAction)
            {
                this.reader = reader;
                this.request = request;
                this.onCompletedAction = onCompletedAction;
            }


            public IEnumerator<object[]> GetEnumerator()
            {
                var enumerator = new DataRecordEnumerator(request, reader, () =>
                {
                    // check if we have more recordsets
                    if (!reader.NextResult())
                    {
                        onCompletedAction();
                    }
                });
                return enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
      

        class DataRecordEnumerator : IEnumerator<object[]>
        {
            private readonly IDataRequest request;
            private readonly IDataReader reader;
            private readonly Action onCompletedAction;
            object[] buffer;
            private bool completed;

            public DataRecordEnumerator(IDataRequest request, IDataReader reader, Action onCompletedAction)
            {
                this.reader = reader;
                this.request = request;
                this.onCompletedAction = onCompletedAction;
            }

            public void Dispose()
            {
                // read till the end;
                while(this.MoveNext()){}
            }

            public bool MoveNext()
            {
                if (this.completed || !this.reader.Read())
                {
                    this.completed = true;
                    this.onCompletedAction();
                    return false;
                }

                if (this.buffer == null || this.request.Mode != DataRequestMode.NoBufferReuseMemory)
                {
                    this.buffer = new object[reader.FieldCount];
                }
                reader.GetValues(this.buffer);
                for (int i = 0; i < this.buffer.Length; i++)
                {
                    if (this.buffer[i] is DBNull)
                    {
                        this.buffer[i] = null;
                    }
                }
                return true;
            }



            public void Reset()
            {
                throw new NotImplementedException();
            }

            public object[] Current
            {
                get { return this.buffer; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
