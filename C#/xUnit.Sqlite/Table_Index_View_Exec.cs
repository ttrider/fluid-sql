// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.IO;
using System.Reflection;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;
using Xunit;

namespace xUnit.Sqlite
{
    public partial class Table_Index_View
    {
        private const string ConnectionStringFormat = @"Data Source={0};Version=3;New=True;Compress=True;";
        static string ConnectionString;

        public static void Init()
        {
            ConnectionString = string.Format(ConnectionStringFormat,
                Path.Combine(Assembly.GetEntryAssembly().CodeBase, "CreateTable.db"));
        }

        public static void Execute(IStatement statement)
        {
            Provider.GetCommand(statement, ConnectionString).ExecuteNonQuery();
        }

        [Fact]
        public void CreateTableExec()
        {
            //Execute(this.CreateTable());
        }

        [Fact]
        public void CreateTableConflictsExec()
        {
            //Execute(this.CreateTableConflicts());
        }

        [Fact]
        public void CreateTableWithIndexExec()
        {
            //Execute(this.CreateTableWithIndex());
        }

        [Fact]
        public void CreateTableWithIndexIfNotExistsExec()
        {
            //Execute(CreateTableWithIndexIfNotExists());
        }
    }
}