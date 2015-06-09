using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using log4net;

namespace TTRider.FluidSql.DataProvider
{
    public class DataResponse : IDataResponse, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DataResponse));

        private IDbCommand command;
        private IDbCommand connection;
        private IDataReader reader;
        private IDictionary<string, object> output;
        private int? returnCode;
        private bool completed;
        private bool disposed;
        private DataRecordEnumerable currentEnumerable;

        internal static IDataResponse GetResponse(IDataRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (request.Command == null) throw new ArgumentException("request.Command");
            if (request.Command.Connection == null) throw new ArgumentException("request.Command.Connection");

            var response = new DataResponse(request);
            response.ProcessRequest();
            return response;
        }

        private DataResponse(IDataRequest request)
        {
            this.Request = request;
            this.command = request.Command;
            this.sessionHash = (request.Command.CommandText+string.Join("-",request.PrerequisiteCommands)+DateTime.Now.ToString("s")).GetHashCode().ToString("x8");
        }

        ~DataResponse()
        {
            Dispose(false);
        }

        private readonly string sessionHash;

        private void ProcessRequest()
        {
            if (this.Request.Command.Connection.State != ConnectionState.Open)
            {
                this.Request.Command.Connection.Open();
            }

            // process Prerequisites
            foreach (var prerequisiteCommand in this.Request.PrerequisiteCommands)
            {
                Log.DebugFormat("{0}-PREREQUISITE_COMMAND: {1}",this.sessionHash,prerequisiteCommand.GetCommandSummary());
                prerequisiteCommand.ExecuteNonQuery();
            }

            Log.DebugFormat("{0}-COMMAND: {1}", this.sessionHash, command.GetCommandSummary());
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
                EnsureOutputValues();
                Log.DebugFormat("{0}-COMMAND COMPLETED", this.sessionHash);
                if (Completed != null)
                {
                    Completed(this, EventArgs.Empty);
                }
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
            GC.SuppressFinalize(this);
            Dispose(true);
        }
        void Dispose(bool disposing)
        {
            if (disposed) return;

            if (reader != null)
            {
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
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
            this.disposed = true;
        }

        public event EventHandler Completed;

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
