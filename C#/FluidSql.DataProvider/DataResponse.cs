using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace TTRider.FluidSql.DataProvider
{
    public class DataResponse : IDataResponse, IDisposable
    {
        private IDbCommand command;
        private IDbCommand connection;
        private IDataReader reader;
        private IDictionary<string, object> output;
        private int? returnCode;
        private bool completed;
        private DataRecordEnumerable currentEnumerable;

        internal static IDataResponse GetResponse(IDataRequest request, IDbCommand command)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (command == null) throw new ArgumentNullException("command");
            if (command.Connection == null) throw new ArgumentException("command.Connection");

            var response = new DataResponse(request, command);
            response.ProcessRequest();
            return response;
        }

        private DataResponse(IDataRequest request, IDbCommand command)
        {
            this.command = command;
            this.Request = request;
        }

        private void ProcessRequest()
        {
            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
            }

            this.reader = command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        private void EnsureOutputValues()
        {
            if (this.output == null)
            {
                // we need to fetch output data
                while (!this.completed)
                {
                    foreach (var record in this.Records)
                    {
                        var dummy = record;
                    }
                }

                var outputData = new Dictionary<string, object>();
                foreach (var parameter in command.Parameters.OfType<DbParameter>().Where(p => p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output))
                {
                    var value = parameter.Value;
                    if (value == DBNull.Value)
                    {
                        value = null;
                    }
                    outputData[parameter.ParameterName] = value;
                }

                var retCode =
                            command.Parameters.OfType<DbParameter>()
                                .FirstOrDefault(p => p.Direction == ParameterDirection.ReturnValue);
                if (retCode != null)
                {
                    this.returnCode = (int?)retCode.Value;
                }

                this.output = outputData;
            }
        }

        void OnRecordsetProcessed()
        {
            this.currentEnumerable = null;
            if (!this.reader.NextResult())
            {
                this.completed = true;
            }
        }

        public IDataRequest Request { get; private set; }

        public IEnumerable<object[]> Records
        {
            get
            {
                if (completed)
                {
                    return Enumerable.Empty<object[]>();
                }

                return this.currentEnumerable ??
                       (this.currentEnumerable = new DataRecordEnumerable(this, this.OnRecordsetProcessed));
            }
        }

        public IDictionary<string, object> Output
        {
            get
            {
                this.EnsureOutputValues();
                return this.output;
            } 
        }

        public int? ReturnCode
        {
            get
            {
                this.EnsureOutputValues();
                return this.returnCode;
            }
        }

        public bool HasMoreData
        {
            get { return !this.completed; }
        }


        public void Dispose()
        {
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
                reader = null;
            }

            if (command != null)
            {
                command.Dispose();
                command = null;
            }

            if (connection != null)
            {
                connection.Dispose();
                connection = null;
            }
        }



        class DataRecordEnumerable : IEnumerable<object[]>
        {
            private readonly DataResponse response;
            private readonly Action onCompletedAction;

            public DataRecordEnumerable(DataResponse response, Action onCompletedAction)
            {
                this.response = response;
                this.onCompletedAction = onCompletedAction;
            }


            public IEnumerator<object[]> GetEnumerator()
            {
                var enumerator = new DataRecordEnumerator(response, this.onCompletedAction);
                return enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }


        class DataRecordEnumerator : IEnumerator<object[]>
        {
            private readonly DataResponse response;
            private readonly Action onCompletedAction;
            object[] buffer;
            private bool completed;

            public DataRecordEnumerator(DataResponse response, Action onCompletedAction)
            {
                this.response = response;
                this.onCompletedAction = onCompletedAction;
            }

            public void Dispose()
            {
                // read till the end;
                while (this.MoveNext()) { }
            }

            public bool MoveNext()
            {
                if (this.completed || !this.response.reader.Read())
                {
                    this.completed = true;
                    this.onCompletedAction();
                    return false;
                }

                if (this.buffer == null || this.response.Request.Mode != DataRequestMode.NoBufferReuseMemory)
                {
                    this.buffer = new object[this.response.reader.FieldCount];
                }
                this.response.reader.GetValues(this.buffer);
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
