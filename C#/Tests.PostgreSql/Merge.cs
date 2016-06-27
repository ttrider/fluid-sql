using TTRider.FluidSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.PostgreSql
{
    /// <summary>
    /// Summary description for Merge
    /// </summary>
    [TestClass]
    public class Merge : PostgreSqlProviderTests
    {
        [TestMethod]
        public void MergeWhenMatchUpdate()
        {
            var statement = Sql.Merge
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenUpdateSet(Sql.Name("id").SetTo(Sql.Name("source.id")), Sql.Name("column_value").SetTo(Sql.Name("source.column_value")));
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nCREATE TEMPORARY TABLE \"tmp_0\" AS ( WITH \"updated\" AS ( UPDATE \"target_tbl\" AS \"target\" SET \"id\" = \"source\".\"id\", \"column_value\" = \"source\".\"column_value\" FROM \"source_tbl\" AS \"source\" WHERE \"target\".\"id\" = \"source\".\"id\" RETURNING \"target\".\"id\" ) SELECT \"id\" FROM \"updated\" );\r\nDROP TABLE IF EXISTS \"tmp_0\";\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void MergeWhenMatchUpdateWithoutAlias()
        {
            var statement = Sql.Merge
                .Into(Sql.Name("target_tbl"))
                .Using("source_tbl")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenUpdateSet(Sql.Name("id").SetTo(Sql.Name("source.id")), Sql.Name("column_value").SetTo(Sql.Name("source.column_value")));
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nCREATE TEMPORARY TABLE \"tmp_0\" AS ( WITH \"updated\" AS ( UPDATE \"target_tbl\" AS \"alias_target_tbl\" SET \"id\" = \"alias_source_tbl\".\"id\", \"column_value\" = \"alias_source_tbl\".\"column_value\" FROM \"source_tbl\" AS \"alias_source_tbl\" WHERE \"alias_target_tbl\".\"id\" = \"alias_source_tbl\".\"id\" RETURNING \"alias_target_tbl\".\"id\" ) SELECT \"id\" FROM \"updated\" );\r\nDROP TABLE IF EXISTS \"tmp_0\";\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void MergeWhenMatchDelete()
        {
            var statement = Sql.Merge
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete();
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nDELETE FROM \"target_tbl\" AS \"target\" WHERE \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"source_tbl\" ) );\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void MergeWhenMatchDeleteWithoutAlias()
        {
            var statement = Sql.Merge
                .Into(Sql.Name("target_tbl"))
                .Using("source_tbl")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete();
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nDELETE FROM \"target_tbl\" AS \"alias_target_tbl\" WHERE \"alias_target_tbl\".\"id\" IN ( ( SELECT \"id\" FROM \"source_tbl\" ) );\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void MergeTopWhenMatchDelete()
        {
            var statement = Sql.Merge
                .Into(Sql.NameAs("target_tbl", "target"))
                .Top(2)
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete();
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nCREATE TEMPORARY TABLE IF NOT EXISTS \"top_alias\" AS ( SELECT \"id\" FROM \"target_tbl\" LIMIT 2 );\r\nDELETE FROM \"target_tbl\" AS \"target\" WHERE \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"source_tbl\" ) ) AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) );\r\nDROP TABLE IF EXISTS \"top_alias\";\r\nEND;\r\n$do$;", command.CommandText);
        }
        
        [TestMethod]
        public void MergeTopWhenMatchDeleteWithCondition()
        {
            var statement = Sql.Merge
                .Into(Sql.NameAs("target_tbl", "target"))
                .Top(2)
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("column_value").IsEqual(Sql.Scalar("text1")));
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nCREATE TEMPORARY TABLE IF NOT EXISTS \"top_alias\" AS ( SELECT \"id\" FROM \"target_tbl\" LIMIT 2 );\r\nDELETE FROM \"target_tbl\" AS \"target\" WHERE \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"source_tbl\" ) ) AND \"target\".\"column_value\" = 'text1' AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) );\r\nDROP TABLE IF EXISTS \"top_alias\";\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void MergeTopWhenMatchDeleteWithConditionWithoutAlias()
        {
            var statement = Sql.Merge
                .Into(Sql.Name("target_tbl"))
                .Top(2)
                .Using("source_tbl")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("column_value").IsEqual(Sql.Scalar("text1")));
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nCREATE TEMPORARY TABLE IF NOT EXISTS \"top_alias\" AS ( SELECT \"id\" FROM \"target_tbl\" LIMIT 2 );\r\nDELETE FROM \"target_tbl\" AS \"alias_target_tbl\" WHERE \"alias_target_tbl\".\"id\" IN ( ( SELECT \"id\" FROM \"source_tbl\" ) ) AND \"alias_target_tbl\".\"column_value\" = 'text1' AND \"alias_target_tbl\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) );\r\nDROP TABLE IF EXISTS \"top_alias\";\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void MergeWhenMatchDeleteCondition()
        {
            var statement = Sql.Merge
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("column_value").IsEqual(Sql.Scalar("222")));
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nDELETE FROM \"target_tbl\" AS \"target\" WHERE \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"source_tbl\" ) ) AND \"target\".\"column_value\" = '222';\r\nEND;\r\n$do$;", command.CommandText);
        }
        
        [TestMethod]
        public void MergeWhenNotMatchBySourceDelete()
        {
            var statement = Sql.Merge
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenNotMatchedBySourceThenDelete();
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nDELETE FROM \"target_tbl\" AS \"target\" WHERE \"target\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"source_tbl\" ) );\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void MergeWhenNotMatchBySourceDeleteWithoutAlias()
        {
            var statement = Sql.Merge
                .Into(Sql.Name("target_tbl"))
                .Using("source_tbl")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenNotMatchedBySourceThenDelete();
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nDELETE FROM \"target_tbl\" AS \"alias_target_tbl\" WHERE \"alias_target_tbl\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"source_tbl\" ) );\r\nEND;\r\n$do$;", command.CommandText);
        }

        /* [TestMethod]
         public void MergeWhenNotMatchedThenInsert()
         {
             var statement = Sql.Merge
                 .Into(Sql.NameAs("target_tbl", "target"))
                 .Using(Sql.Select.From("source_tbl"), "source")
                 .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                 .WhenNotMatchedThenInsert(Sql.Name("source.column_value").IsEqual(Sql.Scalar("text4")));
             var command = Provider.GetCommand(statement);
             Assert.IsNotNull(command);
             Assert.AreEqual("WITH \"source\" AS ( SELECT * FROM \"source_tbl\" ), \"tmp_wnmit_0\" AS ( INSERT INTO \"target_tbl\" SELECT * FROM \"source\" WHERE \"source\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"target_tbl\" ) ) AND \"source\".\"column_value\" = 'text4' ) SELECT 1;", command.CommandText);
         }*/

        [TestMethod]
        public void MergeWhenMatchesDefaultValue()
        {
            var statement = Sql.Merge
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenNotMatchedThenInsert(Sql.Name("source.column_value").IsEqual(Sql.Scalar("text4")), "id", "column_value");
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nWITH \"source\" AS ( SELECT * FROM \"source_tbl\" AS \"source\" WHERE \"source\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"target_tbl\" ) ) AND \"source\".\"column_value\" = 'text4' ) INSERT INTO \"target_tbl\" ( \"id\" ) SELECT \"id\" FROM \"source\";\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void MergeWhenMatchesDefaultValueWithoutAlias()
        {
            var statement = Sql.Merge
                .Into(Sql.Name("target_tbl"))
                .Using("source_tbl")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("id")))
                .WhenNotMatchedThenInsert(Sql.Name("column_value").IsEqual(Sql.Scalar("text4")), "id", "column_value");
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nWITH \"alias_source_tbl\" AS ( SELECT * FROM \"source_tbl\" AS \"alias_source_tbl\" WHERE \"alias_source_tbl\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"target_tbl\" ) ) AND \"alias_source_tbl\".\"column_value\" = 'text4' ) INSERT INTO \"target_tbl\" ( \"id\" ) SELECT \"id\" FROM \"alias_source_tbl\";\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void MergeWhenMatchesValue()
        {
            var statement = Sql.Merge
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenNotMatchedThenInsert(new Name[] { "id", "column_value" }, new Name[] { "source.id", "source.column_value" });

            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nWITH \"source\" AS ( SELECT * FROM \"source_tbl\" AS \"source\" WHERE \"source\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"target_tbl\" ) ) ) INSERT INTO \"target_tbl\" ( \"id\", \"column_value\" ) SELECT \"id\", \"column_value\" FROM \"source\";\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void MergeWhenMatchesValueWithoutAlias()
        {
            var statement = Sql.Merge
                .Into(Sql.Name("target_tbl"))
                .Using("source_tbl")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenNotMatchedThenInsert(new Name[] { "id", "column_value" }, new Name[] { "source.id", "source.column_value" });

            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nWITH \"alias_source_tbl\" AS ( SELECT * FROM \"source_tbl\" AS \"alias_source_tbl\" WHERE \"alias_source_tbl\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"target_tbl\" ) ) ) INSERT INTO \"target_tbl\" ( \"id\", \"column_value\" ) SELECT \"id\", \"column_value\" FROM \"alias_source_tbl\";\r\nEND;\r\n$do$;", command.CommandText);
        }

        #region Unit tests from FluidSql
        [TestMethod]
        public void SuperMerge()
        {
            var statement = Sql.Merge
                .Into(Sql.NameAs("target_tbl", "target"))
                .Top(5)
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("column_value").IsEqual(Sql.Scalar("text1")))
                .WhenMatchedThenUpdateSet(Sql.Name("id").SetTo(Sql.Name("source.id")), Sql.Name("column_value").SetTo(Sql.Name("source.column_value")))
                .WhenNotMatchedThenInsert(Sql.Name("source.column_value").IsEqual(Sql.Scalar("text4")), "id","column_value")
                .WhenNotMatchedBySourceThenDelete()
                ;
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nCREATE TEMPORARY TABLE IF NOT EXISTS \"top_alias\" AS ( SELECT \"id\" FROM \"target_tbl\" LIMIT 5 );\r\nCREATE TEMPORARY TABLE \"tmp_0\" AS ( WITH \"updated\" AS ( UPDATE \"target_tbl\" AS \"target\" SET \"id\" = \"source\".\"id\", \"column_value\" = \"source\".\"column_value\" FROM \"source_tbl\" AS \"source\" WHERE \"target\".\"id\" = \"source\".\"id\" AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) ) RETURNING \"target\".\"id\" ) SELECT \"id\" FROM \"updated\" );\r\nDELETE FROM \"target_tbl\" AS \"target\" WHERE \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"source_tbl\" ) ) AND \"target\".\"column_value\" = 'text1' AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) );\r\nDELETE FROM \"target_tbl\" AS \"target\" WHERE \"target\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"source_tbl\" ) ) AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) );\r\nWITH \"source\" AS ( SELECT * FROM \"source_tbl\" AS \"source\" WHERE \"source\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"target_tbl\" ) ) AND \"source\".\"column_value\" = 'text4' ) INSERT INTO \"target_tbl\" ( \"id\" ) SELECT \"id\" FROM \"source\";\r\nDROP TABLE IF EXISTS \"tmp_0\";\r\nDROP TABLE IF EXISTS \"top_alias\";\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void SuperMerge2()
        {
            var statement = Sql.Merge.Top(10)
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("source.column_value").IsEqual(Sql.Scalar("somename")))
                .WhenMatchedThenUpdateSet(Sql.Name("target.id").SetTo(Sql.Name("source.id")), Sql.Name("target.column_value").SetTo(Sql.Name("source.column_value")))
                .WhenNotMatchedThenInsert(Sql.Name("source.column_value").IsEqual(Sql.Scalar("somename")), "name1", "name2")
                .WhenNotMatchedThenInsert(new Name[] { "target.id", "target.column_value" }, new Name[] { "source.id", "source.column_value" })
                .WhenNotMatchedBySourceThenDelete()
                ;
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nCREATE TEMPORARY TABLE IF NOT EXISTS \"top_alias\" AS ( SELECT \"id\" FROM \"target_tbl\" LIMIT 10 );\r\nCREATE TEMPORARY TABLE \"tmp_0\" AS ( WITH \"updated\" AS ( UPDATE \"target_tbl\" AS \"target\" SET \"id\" = \"source\".\"id\", \"column_value\" = \"source\".\"column_value\" FROM \"source_tbl\" AS \"source\" WHERE \"target\".\"id\" = \"source\".\"id\" AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) ) RETURNING \"target\".\"id\" ) SELECT \"id\" FROM \"updated\" );\r\nDELETE FROM \"target_tbl\" AS \"target\" WHERE \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"source_tbl\" WHERE \"source_tbl\".\"column_value\" = 'somename' ) ) AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) );\r\nDELETE FROM \"target_tbl\" AS \"target\" WHERE \"target\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"source_tbl\" ) ) AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) );\r\nWITH \"source\" AS ( SELECT * FROM \"source_tbl\" AS \"source\" WHERE \"source\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"target_tbl\" ) ) AND \"source\".\"column_value\" = 'somename' ) INSERT INTO \"target_tbl\" ( \"id\" ) SELECT \"id\" FROM \"source\";\r\nWITH \"source\" AS ( SELECT * FROM \"source_tbl\" AS \"source\" WHERE \"source\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"target_tbl\" ) ) ) INSERT INTO \"target_tbl\" ( \"id\", \"column_value\" ) SELECT \"id\", \"column_value\" FROM \"source\";\r\nDROP TABLE IF EXISTS \"tmp_0\";\r\nDROP TABLE IF EXISTS \"top_alias\";\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void SuperMergeWithConditions()
        {
            var statement = Sql.Merge.Top(10)
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("source.column_value").IsEqual(Sql.Scalar("somename")))
                .WhenMatchedThenUpdateSet(Sql.Name("target.column_value").IsNull(), Sql.Name("target.id").SetTo(Sql.Name("source.id")), Sql.Name("target.column_value").SetTo(Sql.Name("source.column_value")))
                .WhenNotMatchedThenInsert(Sql.Name("source.column_value").IsEqual(Sql.Scalar("somename")), "name1", "name2")
                .WhenNotMatchedBySourceThenDelete()
                ;
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nCREATE TEMPORARY TABLE IF NOT EXISTS \"top_alias\" AS ( SELECT \"id\" FROM \"target_tbl\" LIMIT 10 );\r\nCREATE TEMPORARY TABLE \"tmp_0\" AS ( WITH \"updated\" AS ( UPDATE \"target_tbl\" AS \"target\" SET \"id\" = \"source\".\"id\", \"column_value\" = \"source\".\"column_value\" FROM \"source_tbl\" AS \"source\" WHERE \"target\".\"id\" = \"source\".\"id\" AND \"target\".\"column_value\" IS NULL AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) ) RETURNING \"target\".\"id\" ) SELECT \"id\" FROM \"updated\" );\r\nDELETE FROM \"target_tbl\" AS \"target\" WHERE \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"source_tbl\" WHERE \"source_tbl\".\"column_value\" = 'somename' ) ) AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) );\r\nDELETE FROM \"target_tbl\" AS \"target\" WHERE \"target\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"source_tbl\" ) ) AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) );\r\nWITH \"source\" AS ( SELECT * FROM \"source_tbl\" AS \"source\" WHERE \"source\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"target_tbl\" ) ) AND \"source\".\"column_value\" = 'somename' ) INSERT INTO \"target_tbl\" ( \"id\" ) SELECT \"id\" FROM \"source\";\r\nDROP TABLE IF EXISTS \"tmp_0\";\r\nDROP TABLE IF EXISTS \"top_alias\";\r\nEND;\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void SuperMergeWithConditions2()
        {
            var statement = Sql.Merge.Top(10)
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("source.column_value").IsEqual(Sql.Scalar("somename")))
                .WhenMatchedThenUpdateSet(Sql.Name("target.column_value").IsNull(), Sql.Name("target.id").SetTo(Sql.Name("source.id")), Sql.Name("target.column_value").SetTo(Sql.Name("source.column_value")))
                .WhenNotMatchedThenInsert(Sql.Name("source.column_value").IsEqual(Sql.Scalar("somename")), "name1", "name2")
                ;
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN\r\nCREATE TEMPORARY TABLE IF NOT EXISTS \"top_alias\" AS ( SELECT \"id\" FROM \"target_tbl\" LIMIT 10 );\r\nCREATE TEMPORARY TABLE \"tmp_0\" AS ( WITH \"updated\" AS ( UPDATE \"target_tbl\" AS \"target\" SET \"id\" = \"source\".\"id\", \"column_value\" = \"source\".\"column_value\" FROM \"source_tbl\" AS \"source\" WHERE \"target\".\"id\" = \"source\".\"id\" AND \"target\".\"column_value\" IS NULL AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) ) RETURNING \"target\".\"id\" ) SELECT \"id\" FROM \"updated\" );\r\nDELETE FROM \"target_tbl\" AS \"target\" WHERE \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"source_tbl\" WHERE \"source_tbl\".\"column_value\" = 'somename' ) ) AND \"target\".\"id\" IN ( ( SELECT \"id\" FROM \"top_alias\" ) );\r\nWITH \"source\" AS ( SELECT * FROM \"source_tbl\" AS \"source\" WHERE \"source\".\"id\" NOT IN ( ( SELECT \"id\" FROM \"target_tbl\" ) ) AND \"source\".\"column_value\" = 'somename' ) INSERT INTO \"target_tbl\" ( \"id\" ) SELECT \"id\" FROM \"source\";\r\nDROP TABLE IF EXISTS \"tmp_0\";\r\nDROP TABLE IF EXISTS \"top_alias\";\r\nEND;\r\n$do$;", command.CommandText);
        }
        #endregion
    }
}
