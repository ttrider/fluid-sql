// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Data;
using System.Diagnostics;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers;

namespace Tests.ProvidersEndToEnd
{
    static class Common
    {
        internal static void ExecuteCommand(IDbCommand command)
        {
            using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                do
                {
                    while (reader.Read())
                    {
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            Trace.Write(reader.GetValue(i));
                            Trace.Write("\t");
                        }
                        Trace.Write("\r\n");
                    }
                } while (reader.NextResult());
            }
        }

        internal static void GenerateAndExecute(IProvider provider, IStatement statement, string connectionString)
        {
            Trace.WriteLine("---------------------------------");
            var script = provider.GenerateStatement(statement);
            Trace.WriteLine(script);

            using (var command = provider.GetCommand(statement, connectionString))
            {
                ExecuteCommand(command);
            }
        }

        internal static IStatement CreateSimpleStatement()
        {
            var notesTable = Sql.Name("notes");
            var usersTable = Sql.Name("users");

            // delete tables, if they already exist
            var predropTables = Sql.Statements(
                Sql.DropTable(notesTable, true),
                Sql.DropTable(usersTable, true)
                );

            // create tables
            var createTableUser = Sql
                .CreateTable(usersTable, true)
                .Columns(
                    TableColumn.Int("id"),
                    TableColumn.VarChar("name", 200))
                .PrimaryKey(true, new Order { Column = "id", Direction = Direction.Asc })
                .IndexOn("IX_name", new Order { Column = "name" });


            var createTableNote = Sql
                .CreateTable(notesTable, true)
                .Columns(
                    TableColumn.Int("id").PrimaryKey(Direction.Asc).AutoIncrement(),
                    TableColumn.Int("user_id"),
                    TableColumn.VarChar("note"),
                    TableColumn.DateTime("timestamp").Default(Sql.Now()))
                .IndexOn("IX_user_id", new Order { Column = "user_id" });

            // populate tables

            var insertUsers = Sql.Insert.Into(usersTable)
                .Columns("id", "name")
                .Values(1, "James")
                .Values(2, "Spock")
                .Values(3, "Leonard")
                .Values(4, "Montgomery")
                .Values(5, "Pavel");

            var insertNotes = Sql.Insert.Into(notesTable)
                .Columns("user_id", "note")
                .Values(1, "James note 1")
                .Values(2, "Spock note 1")
                .Values(3, "Leonard note 1")
                .Values(4, "Montgomery note 1")
                .Values(5, "Pavel note 1")
                .Values(1, "James note 2")
                .Values(2, "Spock note 2")
                .Values(3, "Leonard note 2")
                .Values(4, "Montgomery note 2")
                .Values(5, "Pavel note 2")
                .Values(1, "James note 3")
                .Values(2, "Spock note 3")
                .Values(3, "Leonard note 3")
                .Values(4, "Montgomery note 3")
                .Values(5, "Pavel note 3")
                ;

            // select some values
            var simpleSelect =
                Sql.Select.From(usersTable, "n").Output("n.*").Where(Sql.Name("n", "id").LessOrEqual(Sql.Scalar(3)));

            var joinSelect = Sql.Select.From(notesTable, "nt")
                .InnerJoin(usersTable, "nm", Sql.Name("nt", "user_id").IsEqual("nm.id"))
                .Output("nt.timestamp", Sql.Name("nm", "name"), "nt.note");

            // update values

            var updateValues = Sql.Update(notesTable)
                .Set("note", Sql.Scalar("this is the new value for the note"))
                .Where(Sql.Name("id").IsEqual(Sql.Scalar(4)));


            var deleteValues = Sql.Delete.From(notesTable).Where(Sql.Name("id").IsEqual(Sql.Scalar(2)));


            var dropTables = Sql.Statements(
                Sql.DropTable(notesTable, true),
                Sql.DropTable(usersTable, true)
                );


            var completeScript = Sql.Statements(
                predropTables,
                createTableUser,
                createTableNote,
                insertUsers,
                insertNotes,
                simpleSelect,
                joinSelect,
                updateValues,
                deleteValues,
                dropTables
                );

            return completeScript;
        }
    }
}