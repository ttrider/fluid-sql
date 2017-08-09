// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TTRider.FluidSql;
using Xunit;

namespace xUnit.FluidSql
{
    public class Merge
    {
        [Fact]
        public void SuperMerge()
        {
            var statement = Sql.Merge.Top(10)
                .Into(Sql.NameAs("foo.target", "target"))
                .Using(Sql.Select.From("foo.source"), "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("source.name").IsEqual("somename"))
                .WhenMatchedThenUpdateSet(Sql.Name("target.a").SetTo(Sql.Name("source.a")),
                    Sql.Name("target.b").SetTo(Sql.Name("source.b")))
                .WhenNotMatchedThenInsert(Sql.Name("source.name").IsEqual("somename"), "name1", "name2")
                .WhenNotMatchedThenInsert(new Name[] { "target.a", "target.b" }, new Name[] { "source.a", "source.b" })
                .WhenNotMatchedBySourceThenDelete()
                ;
            var command = Utilities.GetCommand(statement);
            Assert.NotNull(command);
            Assert.Equal(
                "MERGE TOP ( 10 ) INTO [foo].[target] AS [target] USING ( SELECT * FROM [foo].[source] ) AS [source] ON [target].[id] = [source].[id] WHEN MATCHED AND [source].[name] = [somename] THEN DELETE WHEN MATCHED THEN UPDATE SET [target].[a] = [source].[a], [target].[b] = [source].[b] WHEN NOT MATCHED BY TARGET AND [source].[name] = [somename] THEN INSERT ( [name1], [name2] ) DEFAULT VALUES WHEN NOT MATCHED BY TARGET THEN INSERT ( [target].[a], [target].[b] ) VALUES ( [source].[a], [source].[b] ) WHEN NOT MATCHED BY SOURCE THEN DELETE;",
                command.CommandText);

            var serialized = JsonConvert.SerializeObject(statement, new JsonSerializerSettings()
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                MetadataPropertyHandling = MetadataPropertyHandling.Default,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                TypeNameHandling = TypeNameHandling.All
            });

            File.WriteAllText(@"c:\temp\statement.json",serialized);

            var statement2 = JsonConvert.DeserializeObject<MergeStatement>(serialized);
            //var command2 = Utilities.GetCommand(statement2);
            //Assert.NotNull(command2);
            //Assert.Equal(
            //    "MERGE TOP ( 10 ) INTO [foo].[target] AS [target] USING ( SELECT * FROM [foo].[source] ) AS [source] ON [target].[id] = [source].[id] WHEN MATCHED AND [source].[name] = [somename] THEN DELETE WHEN MATCHED THEN UPDATE SET [target].[a] = [source].[a], [target].[b] = [source].[b] WHEN NOT MATCHED BY TARGET AND [source].[name] = [somename] THEN INSERT ( [name1], [name2] ) DEFAULT VALUES WHEN NOT MATCHED BY TARGET THEN INSERT ( [target].[a], [target].[b] ) VALUES ( [source].[a], [source].[b] ) WHEN NOT MATCHED BY SOURCE THEN DELETE;",
            //    command2.CommandText);

        }

        [Fact]
        public void SuperMerge2()
        {
            var statement = Sql.Merge.Top(10)
                .Into(Sql.NameAs("foo.target", "target"))
                .Using("foo.source", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("source.name").IsEqual("somename"))
                .WhenMatchedThenUpdateSet(Sql.Name("target.a").SetTo(Sql.Name("source.a")),
                    Sql.Name("target.b").SetTo(Sql.Name("source.b")))
                .WhenNotMatchedThenInsert(Sql.Name("source.name").IsEqual("somename"), "name1", "name2")
                .WhenNotMatchedThenInsert(new Name[] { "target.a", "target.b" }, new Name[] { "source.a", "source.b" })
                .WhenNotMatchedBySourceThenDelete()
                ;
            var command = Utilities.GetCommand(statement);
            Assert.NotNull(command);
            Assert.Equal(
                "MERGE TOP ( 10 ) INTO [foo].[target] AS [target] USING [foo].[source] AS [source] ON [target].[id] = [source].[id] WHEN MATCHED AND [source].[name] = [somename] THEN DELETE WHEN MATCHED THEN UPDATE SET [target].[a] = [source].[a], [target].[b] = [source].[b] WHEN NOT MATCHED BY TARGET AND [source].[name] = [somename] THEN INSERT ( [name1], [name2] ) DEFAULT VALUES WHEN NOT MATCHED BY TARGET THEN INSERT ( [target].[a], [target].[b] ) VALUES ( [source].[a], [source].[b] ) WHEN NOT MATCHED BY SOURCE THEN DELETE;",
                command.CommandText);
        }

        [Fact]
        public void SuperMergeWithConditions()
        {
            var statement = Sql.Merge.Top(10)
                .Into(Sql.NameAs("foo.target", "target"))
                .Using("foo.source", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("source.name").IsEqual("somename"))
                .WhenMatchedThenUpdateSet(Sql.Name("target.something").IsNull(),
                    Sql.Name("target.a").SetTo(Sql.Name("source.a")), Sql.Name("target.b").SetTo(Sql.Name("source.b")))
                .WhenNotMatchedThenInsert(Sql.Name("source.name").IsEqual("somename"), "name1", "name2")
                .WhenNotMatchedThenInsert(new Name[] { "target.a", "target.b" }, new Name[] { "source.a", "source.b" })
                .WhenNotMatchedBySourceThenDelete()
                ;
            var command = Utilities.GetCommand(statement);
            Assert.NotNull(command);
            Assert.Equal(
                "MERGE TOP ( 10 ) INTO [foo].[target] AS [target] USING [foo].[source] AS [source] ON [target].[id] = [source].[id] WHEN MATCHED AND [source].[name] = [somename] THEN DELETE WHEN MATCHED AND [target].[something] IS NULL THEN UPDATE SET [target].[a] = [source].[a], [target].[b] = [source].[b] WHEN NOT MATCHED BY TARGET AND [source].[name] = [somename] THEN INSERT ( [name1], [name2] ) DEFAULT VALUES WHEN NOT MATCHED BY TARGET THEN INSERT ( [target].[a], [target].[b] ) VALUES ( [source].[a], [source].[b] ) WHEN NOT MATCHED BY SOURCE THEN DELETE;",
                command.CommandText);
        }

        [Fact]
        public void SuperMergeWithConditions2()
        {
            var statement = Sql.Merge.Top(10)
                .Into(Sql.NameAs("foo.target", "target"))
                .Using("foo.source", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("source.name").IsEqual("somename"))
                .WhenMatchedThenUpdateSet(Sql.Name("target.something").IsNull(),
                    Sql.Name("target.a").SetTo(Sql.Name("source.a")), Sql.Name("target.b").SetTo(Sql.Name("source.b")))
                .WhenNotMatchedThenInsert(Sql.Name("source.name").IsEqual("somename"), "name1", "name2")
                .WhenNotMatchedThenInsert(new Name[] { "target.a", "target.b" }, new Name[] { "source.a", "source.b" })
                .WhenNotMatchedBySourceThenUpdate(Sql.Name("target.something").IsNull(),
                    Sql.Name("target.a").SetTo(Sql.Name("source.a")))
                .WhenNotMatchedBySourceThenUpdate(Sql.Name("target.b").SetTo(Sql.Name("source.b")))
                ;
            var command = Utilities.GetCommand(statement);
            Assert.NotNull(command);
            Assert.Equal(
                "MERGE TOP ( 10 ) INTO [foo].[target] AS [target] USING [foo].[source] AS [source] ON [target].[id] = [source].[id] WHEN MATCHED AND [source].[name] = [somename] THEN DELETE WHEN MATCHED AND [target].[something] IS NULL THEN UPDATE SET [target].[a] = [source].[a], [target].[b] = [source].[b] WHEN NOT MATCHED BY TARGET AND [source].[name] = [somename] THEN INSERT ( [name1], [name2] ) DEFAULT VALUES WHEN NOT MATCHED BY TARGET THEN INSERT ( [target].[a], [target].[b] ) VALUES ( [source].[a], [source].[b] ) WHEN NOT MATCHED BY SOURCE AND [target].[something] IS NULL THEN UPDATE SET [target].[a] = [source].[a] WHEN NOT MATCHED BY SOURCE THEN UPDATE SET [target].[b] = [source].[b];",
                command.CommandText);
        }
    }
}