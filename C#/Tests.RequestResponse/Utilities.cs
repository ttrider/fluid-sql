// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System;
using System.Data.SqlClient;
using System.IO;

namespace Tests.DataProvider
{
    static class Utilities
    {
        public static string CreateTestDatabase()
        {
            var name = "D" + Guid.NewGuid().ToString("N");

            var target = Path.GetFullPath(@".\" + name + ".mdf");

            var cs = @"Data Source=(LocalDB)\v11.0;Integrated Security=True;Connect Timeout=30;Initial Catalog=";
            cs += name;

            if (!File.Exists(target))
            {
                var connection = new SqlConnection(@"server=(LocalDB)\v11.0");
                using (connection)
                {
                    connection.Open();

                    var sql =
                        string.Format(
                            @"CREATE DATABASE [{1}] ON PRIMARY (NAME={1}_data, FILENAME = '{0}{1}_data.mdf') LOG ON (NAME={1}_log, FILENAME = '{0}{1}_log.ldf')",
                            Path.GetFullPath(@".\"), name);
                    new SqlCommand(sql, connection).ExecuteNonQuery();

                    new SqlCommand(string.Format(@"USE [{0}]", name), connection).ExecuteNonQuery();

                    new SqlCommand(
                        @"CREATE TABLE Person ( Id INT NOT NULL PRIMARY KEY, Name NVARCHAR(255) NOT NULL, DOB DATETIME NULL);",
                        connection).ExecuteNonQuery();

                    new SqlCommand(@"INSERT INTO Person ( Id, Name, DOB) VALUES 
                    (1,'Adam','1970-11-2'),
                    (2,'John','1971-10-6'),
                    (3,'James','1972-9-8'),
                    (4,'Vlad','1973-8-10'),
                    (5,'Paul','1974-7-12'),
                    (6,'Ringo','1975-6-16'),
                    (7,'George','1977-5-20'),
                    (8,'Alexei','1978-4-22'),
                    (9,'Bruce','1979-3-25');", connection).ExecuteNonQuery();

                    new SqlCommand(
                        @"CREATE PROCEDURE GetPersonById (@Id INT) AS BEGIN SELECT Id, Name, Dob FROM Person WHERE Id = @Id; RETURN 5; END;",
                        connection).ExecuteNonQuery();
                    new SqlCommand(
                        @"CREATE PROCEDURE OutputPersonById (@Id INT, @Name NVARCHAR(255) OUTPUT, @Dob DATETIME OUTPUT) AS BEGIN SELECT @Name = Name, @dob = Dob FROM Person WHERE Id = @Id; RETURN 5; END;",
                        connection).ExecuteNonQuery();

                    connection.Close();
                }
            }

            return cs;
        }
    }
}