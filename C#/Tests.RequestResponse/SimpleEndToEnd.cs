// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.SqlServer;
using TTRider.FluidSql.RequestResponse;

namespace Tests.DataProvider
{
    [TestClass]
    public class SimpleEndToEnd
    {
        private static string connectionString;

        [ClassInitialize]
        public static void ClassInit(TestContext testcontext)
        {
            connectionString = Utilities.CreateTestDatabase();
        }

        [TestMethod]
        public void ExecuteQueryForOutput()
        {
            var select =
                Sql.Select.From(Sql.Name("Person"), "p")
                    .Top(1)
                    .Assign(Sql.Name("@Name"), Sql.Name("p", "Name"))
                    .Where(Sql.Name("p", "Id").IsEqual(Sql.Parameter.Int("@Id")));
            select.Parameters.Add(Sql.Parameter.NVarChar("@Name").ParameterDirection(ParameterDirection.Output));
            select.ParameterValues.Add(new ParameterValue
            {
                Name = "@Id",
                Value = 1
            });

            var s = Sql.Statements(select);

            var request = DataRequest.Create<SqlServerProvider>(s, connectionString);
            var response = request.GetResponse();

            Assert.AreEqual("Adam", response.Output["@Name"]);
        }


        [TestMethod]
        public void ExecuteQuery()
        {
            var select =
                Sql.Select.From(Sql.Name("Person"), "p")
                    .Top(1)
                    .Output(Sql.Name("p", "Name"))
                    .Where(Sql.Name("p", "Id").IsEqual(Sql.Parameter.Int("@Id")));
            select.Parameters.Add(Sql.Parameter.NVarChar("@Name").ParameterDirection(ParameterDirection.Output));
            select.ParameterValues.Add(new ParameterValue
            {
                Name = "@Id",
                Value = 1
            });

            var s = Sql.Statements(select);

            var request = DataRequest.Create<SqlServerProvider>(s, connectionString);
            var response = request.GetResponse();

            var f = response.Records.First();

            Assert.AreEqual("Adam", f[0]);
        }

        [TestMethod]
        public void ExecuteMultipleRecordsetsQuery()
        {
            var select1 =
                Sql.Select.From(Sql.Name("Person"), "p")
                    .Top(1)
                    .Output(Sql.Name("p", "Name"))
                    .Where(Sql.Name("p", "Id").IsEqual(Sql.Parameter.Int("@Id")));
            select1.Parameters.Add(Sql.Parameter.NVarChar("@Name").ParameterDirection(ParameterDirection.Output));
            select1.ParameterValues.Add("@Id", 1);

            var select2 =
                Sql.Select.From(Sql.Name("Person"), "p")
                    .Top(1)
                    .Output(Sql.Name("p", "Name"), Sql.Name("p", "Dob"))
                    .Where(Sql.Name("p", "Id").IsEqual(Sql.Parameter.Int("@Id")));

            var s = Sql.Statements(select1, select2);

            var request = DataRequest.Create<SqlServerProvider>(s, connectionString);
            var response = request.GetResponse();

            var f = response.Records.First();
            Assert.AreEqual("Adam", f[0]);
            Assert.AreEqual(true, response.HasMoreData);

            var f2 = response.Records.First();
            Assert.AreEqual("Adam", f2[0]);
            Assert.AreEqual(new DateTime(1970, 11, 2), f2[1]);
            Assert.AreEqual(false, response.HasMoreData);
        }
    }
}