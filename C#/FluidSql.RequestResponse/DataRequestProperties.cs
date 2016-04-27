// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TTRider.Data.RequestResponse;
using TTRider.FluidSql.Providers;

namespace TTRider.FluidSql.RequestResponse
{
    public class DataRequestProperties
    {
        public DataRequestProperties()
        {
            this.PrerequisiteStatements = new List<IStatement>();
        }

        public IProvider QueryProvider { get; set; }

        public string ConnectionString { get; set; }

        public DbRequestMode Mode { get; set; }

        public IStatement Statement { get; set; }

        public IList<IStatement> PrerequisiteStatements { get; }


        internal DataRequestProperties Clone()
        {
            var clone = new DataRequestProperties
            {
                ConnectionString = this.ConnectionString,
                Mode = this.Mode,
                QueryProvider = this.QueryProvider,
                Statement = this.Statement
            };
            foreach (var ps in this.PrerequisiteStatements)
            {
                clone.PrerequisiteStatements.Add(ps);
            }
            return clone;
        }

        internal IDbCommand GetCommand()
        {
            return this.QueryProvider.GetCommand(this.Statement, this.ConnectionString);
        }

        internal Task<IDbCommand> GetCommandAsync()
        {
            return this.QueryProvider.GetCommandAsync(this.Statement, this.ConnectionString);
        }

        internal IEnumerable<IDbCommand> GetPrerequiseteCommands()
        {
            return this.PrerequisiteStatements.Select(ps => this.QueryProvider.GetCommand(ps));
        }
    }
}