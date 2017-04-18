// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace Tests.Sqlite
{
    public partial class Table_Index_View
    {
        private const string ConnectionStringFormat = @"Data Source={0};Version=3;New=True;Compress=True;";
        static string ConnectionString;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            ConnectionString = string.Format(ConnectionStringFormat,
                Path.Combine(context.TestResultsDirectory, "CreateTable.db"));
        }

        public static void Execute(IStatement statement)
        {
            Provider.GetCommand(statement, ConnectionString).ExecuteNonQuery();
        }

        [TestMethod]
        public void CreateTableExec()
        {
            Execute(this.CreateTable());
        }

        [TestMethod]
        public void CreateTableConflictsExec()
        {
            Execute(this.CreateTableConflicts());
        }

        [TestMethod]
        public void CreateTableWithIndexExec()
        {
            Execute(this.CreateTableWithIndex());
        }

        [TestMethod]
        public void CreateTableWithIndexIfNotExistsExec()
        {
            Execute(CreateTableWithIndexIfNotExists());
        }
    }
}