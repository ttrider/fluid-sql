using TTRider.FluidSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.MySqlTests
{
    /// <summary>
    /// Summary description for Merge
    /// </summary>
    [TestClass]
    public class Merge : MySqlProviderTests
    {
        [TestMethod]
        public void MergeCreateTopTemporaryTable()
        {
            var statement = Sql.Merge
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .Top(5);
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE IF NOT EXISTS `top_alias` AS ( SELECT `id` FROM `target_tbl` LIMIT 5 );\r\nDROP TEMPORARY TABLE IF EXISTS `top_alias`;\r\nCOMMIT;", command.CommandText);
        }

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
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE `tmp_0` AS ( SELECT `target`.`id` FROM `target_tbl` AS `target` INNER JOIN `source_tbl` AS `source` ON `target`.`id` = `source`.`id` WHERE `target`.`id` = `source`.`id` );\r\nUPDATE `target_tbl` AS `target`, `source_tbl` AS `source` SET `target`.`id` = `source`.`id`, `target`.`column_value` = `source`.`column_value` WHERE `target`.`id` = `source`.`id`;\r\nDROP TEMPORARY TABLE IF EXISTS `tmp_0`;\r\nCOMMIT;", command.CommandText);
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
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE `tmp_0` AS ( SELECT `alias_target_tbl`.`id` FROM `target_tbl` AS `alias_target_tbl` INNER JOIN `source_tbl` AS `alias_source_tbl` ON `alias_target_tbl`.`id` = `alias_source_tbl`.`id` WHERE `alias_target_tbl`.`id` = `alias_source_tbl`.`id` );\r\nUPDATE `target_tbl` AS `alias_target_tbl`, `source_tbl` AS `alias_source_tbl` SET `alias_target_tbl`.`id` = `alias_source_tbl`.`id`, `alias_target_tbl`.`column_value` = `alias_source_tbl`.`column_value` WHERE `alias_target_tbl`.`id` = `alias_source_tbl`.`id`;\r\nDROP TEMPORARY TABLE IF EXISTS `tmp_0`;\r\nCOMMIT;", command.CommandText);
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
            Assert.AreEqual("START TRANSACTION;\r\nDELETE `target` FROM `target_tbl` AS `target` WHERE `target`.`id` IN ( ( SELECT `id` FROM `source_tbl` ) );\r\nCOMMIT;", command.CommandText);
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
            Assert.AreEqual("START TRANSACTION;\r\nDELETE `alias_target_tbl` FROM `target_tbl` AS `alias_target_tbl` WHERE `alias_target_tbl`.`id` IN ( ( SELECT `id` FROM `source_tbl` ) );\r\nCOMMIT;", command.CommandText);
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
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE IF NOT EXISTS `top_alias` AS ( SELECT `id` FROM `target_tbl` LIMIT 2 );\r\nDELETE `target` FROM `target_tbl` AS `target` WHERE `target`.`id` IN ( ( SELECT `id` FROM `source_tbl` ) ) AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nDROP TEMPORARY TABLE IF EXISTS `top_alias`;\r\nCOMMIT;", command.CommandText);
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
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE IF NOT EXISTS `top_alias` AS ( SELECT `id` FROM `target_tbl` LIMIT 2 );\r\nDELETE `target` FROM `target_tbl` AS `target` WHERE `target`.`id` IN ( ( SELECT `id` FROM `source_tbl` ) ) AND `target`.`column_value` = N'text1' AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nDROP TEMPORARY TABLE IF EXISTS `top_alias`;\r\nCOMMIT;", command.CommandText);
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
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE IF NOT EXISTS `top_alias` AS ( SELECT `id` FROM `target_tbl` LIMIT 2 );\r\nDELETE `alias_target_tbl` FROM `target_tbl` AS `alias_target_tbl` WHERE `alias_target_tbl`.`id` IN ( ( SELECT `id` FROM `source_tbl` ) ) AND `alias_target_tbl`.`column_value` = N'text1' AND `alias_target_tbl`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nDROP TEMPORARY TABLE IF EXISTS `top_alias`;\r\nCOMMIT;", command.CommandText);
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
            Assert.AreEqual("START TRANSACTION;\r\nDELETE `target` FROM `target_tbl` AS `target` WHERE `target`.`id` IN ( ( SELECT `id` FROM `source_tbl` ) ) AND `target`.`column_value` = N'222';\r\nCOMMIT;", command.CommandText);
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
            Assert.AreEqual("START TRANSACTION;\r\nDELETE `target` FROM `target_tbl` AS `target` WHERE `target`.`id` NOT IN ( ( SELECT `id` FROM `source_tbl` ) );\r\nCOMMIT;", command.CommandText);
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
            Assert.AreEqual("START TRANSACTION;\r\nDELETE `alias_target_tbl` FROM `target_tbl` AS `alias_target_tbl` WHERE `alias_target_tbl`.`id` NOT IN ( ( SELECT `id` FROM `source_tbl` ) );\r\nCOMMIT;", command.CommandText);
        }

        [TestMethod]
        public void MergeWhenNotMatchesDefaultValue()
        {
            var statement = Sql.Merge
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenNotMatchedThenInsert(Sql.Name("source.column_value").IsEqual(Sql.Scalar("text4")), "id", "column_value");
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE `tmp_source` AS ( SELECT * FROM `source_tbl` AS `source` WHERE `source`.`id` NOT IN ( ( SELECT `id` FROM `target_tbl` ) ) AND `source`.`column_value` = N'text4' );\r\nINSERT INTO `target_tbl` ( `id` ) SELECT `id` FROM `tmp_source`;\r\nDROP TABLE IF EXISTS `tmp_source`;\r\nCOMMIT;", command.CommandText);
        }

        [TestMethod]
        public void MergeWhenNotMatchesDefaultValueWithoutAlias()
        {
            var statement = Sql.Merge
                .Into(Sql.Name("target_tbl"))
                .Using("source_tbl")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("id")))
                .WhenNotMatchedThenInsert(Sql.Name("column_value").IsEqual(Sql.Scalar("text41")), "id", "column_value");
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE `tmp_alias_source_tbl` AS ( SELECT * FROM `source_tbl` AS `alias_source_tbl` WHERE `alias_source_tbl`.`id` NOT IN ( ( SELECT `id` FROM `target_tbl` ) ) AND `alias_source_tbl`.`column_value` = N'text41' );\r\nINSERT INTO `target_tbl` ( `id` ) SELECT `id` FROM `tmp_alias_source_tbl`;\r\nDROP TABLE IF EXISTS `tmp_alias_source_tbl`;\r\nCOMMIT;", command.CommandText);
        }

        [TestMethod]
        public void MergeWhenNotMatchesValue()
        {
            var statement = Sql.Merge
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenNotMatchedThenInsert(new Name[] { "id", "column_value" }, new Name[] { "source.id", "source.column_value" });

            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE `tmp_source` AS ( SELECT * FROM `source_tbl` AS `source` WHERE `source`.`id` NOT IN ( ( SELECT `id` FROM `target_tbl` ) ) );\r\nINSERT INTO `target_tbl` ( `id`, `column_value` ) SELECT `id`, `column_value` FROM `tmp_source`;\r\nDROP TABLE IF EXISTS `tmp_source`;\r\nCOMMIT;", command.CommandText);
        }

        [TestMethod]
        public void MergeWhenNotMatchesValueWithoutAlias()
        {
            var statement = Sql.Merge
                .Into(Sql.Name("target_tbl"))
                .Using("source_tbl")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenNotMatchedThenInsert(new Name[] { "id", "column_value" }, new Name[] { "source.id", "source.column_value" });

            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE `tmp_alias_source_tbl` AS ( SELECT * FROM `source_tbl` AS `alias_source_tbl` WHERE `alias_source_tbl`.`id` NOT IN ( ( SELECT `id` FROM `target_tbl` ) ) );\r\nINSERT INTO `target_tbl` ( `id`, `column_value` ) SELECT `id`, `column_value` FROM `tmp_alias_source_tbl`;\r\nDROP TABLE IF EXISTS `tmp_alias_source_tbl`;\r\nCOMMIT;", command.CommandText);
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
                .WhenMatchedThenDelete(Sql.Name("column_value").IsEqual(Sql.Scalar("name1")))
                .WhenMatchedThenUpdateSet(Sql.Name("id").SetTo(Sql.Name("source.id")), Sql.Name("column_value").SetTo(Sql.Name("source.column_value")))
                .WhenNotMatchedThenInsert(Sql.Name("source.column_value").IsEqual(Sql.Scalar("name41")), "id", "column_value")
                .WhenNotMatchedBySourceThenDelete()
                ;
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE IF NOT EXISTS `top_alias` AS ( SELECT `id` FROM `target_tbl` LIMIT 5 );\r\nCREATE TEMPORARY TABLE `tmp_0` AS ( SELECT `target`.`id` FROM `target_tbl` AS `target` INNER JOIN `source_tbl` AS `source` ON `target`.`id` = `source`.`id` WHERE `target`.`id` = `source`.`id` AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) ) );\r\nUPDATE `target_tbl` AS `target`, `source_tbl` AS `source` SET `target`.`id` = `source`.`id`, `target`.`column_value` = `source`.`column_value` WHERE `target`.`id` = `source`.`id` AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nDELETE `target` FROM `target_tbl` AS `target` WHERE `target`.`id` IN ( ( SELECT `id` FROM `source_tbl` ) ) AND `target`.`column_value` = N'name1' AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nDELETE `target` FROM `target_tbl` AS `target` WHERE `target`.`id` NOT IN ( ( SELECT `id` FROM `source_tbl` ) ) AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nCREATE TEMPORARY TABLE `tmp_source` AS ( SELECT * FROM `source_tbl` AS `source` WHERE `source`.`id` NOT IN ( ( SELECT `id` FROM `target_tbl` ) ) AND `source`.`column_value` = N'name41' );\r\nINSERT INTO `target_tbl` ( `id` ) SELECT `id` FROM `tmp_source`;\r\nDROP TABLE IF EXISTS `tmp_source`;\r\nDROP TEMPORARY TABLE IF EXISTS `tmp_0`;\r\nDROP TEMPORARY TABLE IF EXISTS `top_alias`;\r\nCOMMIT;", command.CommandText);
        }

        [TestMethod]
        public void SuperMerge2()
        {
            var statement = Sql.Merge.Top(10)
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("source.column_value").IsEqual(Sql.Scalar("name31")))
                .WhenMatchedThenUpdateSet(Sql.Name("target.id").SetTo(Sql.Name("source.id")), Sql.Name("target.column_value").SetTo(Sql.Name("source.column_value")))
                .WhenNotMatchedThenInsert(Sql.Name("source.column_value").IsEqual(Sql.Scalar("name51")), "name1", "name2")
                .WhenNotMatchedThenInsert(new Name[] { "target.id", "target.column_value" }, new Name[] { "source.id", "source.column_value" })
                .WhenNotMatchedBySourceThenDelete()
                ;
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE IF NOT EXISTS `top_alias` AS ( SELECT `id` FROM `target_tbl` LIMIT 10 );\r\nCREATE TEMPORARY TABLE `tmp_0` AS ( SELECT `target`.`id` FROM `target_tbl` AS `target` INNER JOIN `source_tbl` AS `source` ON `target`.`id` = `source`.`id` WHERE `target`.`id` = `source`.`id` AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) ) );\r\nUPDATE `target_tbl` AS `target`, `source_tbl` AS `source` SET `target`.`id` = `source`.`id`, `target`.`column_value` = `source`.`column_value` WHERE `target`.`id` = `source`.`id` AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nDELETE `target` FROM `target_tbl` AS `target` WHERE `target`.`id` IN ( ( SELECT `id` FROM `source_tbl` WHERE `source_tbl`.`column_value` = N'name31' ) ) AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nDELETE `target` FROM `target_tbl` AS `target` WHERE `target`.`id` NOT IN ( ( SELECT `id` FROM `source_tbl` ) ) AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nCREATE TEMPORARY TABLE `tmp_source` AS ( SELECT * FROM `source_tbl` AS `source` WHERE `source`.`id` NOT IN ( ( SELECT `id` FROM `target_tbl` ) ) AND `source`.`column_value` = N'name51' );\r\nINSERT INTO `target_tbl` ( `id` ) SELECT `id` FROM `tmp_source`;\r\nDROP TABLE IF EXISTS `tmp_source`;\r\nCREATE TEMPORARY TABLE `tmp_source` AS ( SELECT * FROM `source_tbl` AS `source` WHERE `source`.`id` NOT IN ( ( SELECT `id` FROM `target_tbl` ) ) );\r\nINSERT INTO `target_tbl` ( `id`, `column_value` ) SELECT `id`, `column_value` FROM `tmp_source`;\r\nDROP TABLE IF EXISTS `tmp_source`;\r\nDROP TEMPORARY TABLE IF EXISTS `tmp_0`;\r\nDROP TEMPORARY TABLE IF EXISTS `top_alias`;\r\nCOMMIT;", command.CommandText);
        }

        [TestMethod]
        public void SuperMergeWithConditions()
        {
            var statement = Sql.Merge.Top(10)
                .Into(Sql.NameAs("target_tbl", "target"))
                .Using("source_tbl", "source")
                .On(Sql.Name("target.id").IsEqual(Sql.Name("source.id")))
                .WhenMatchedThenDelete(Sql.Name("source.column_value").IsEqual(Sql.Scalar("name31")))
                .WhenMatchedThenUpdateSet(Sql.Name("target.column_value").IsNull(), Sql.Name("target.id").SetTo(Sql.Name("source.id")), Sql.Name("target.column_value").SetTo(Sql.Name("source.column_value")))
                .WhenNotMatchedThenInsert(Sql.Name("source.column_value").IsEqual(Sql.Scalar("name51")), "name1", "name2")
                .WhenNotMatchedBySourceThenDelete()
                ;
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE IF NOT EXISTS `top_alias` AS ( SELECT `id` FROM `target_tbl` LIMIT 10 );\r\nCREATE TEMPORARY TABLE `tmp_0` AS ( SELECT `target`.`id` FROM `target_tbl` AS `target` INNER JOIN `source_tbl` AS `source` ON `target`.`id` = `source`.`id` WHERE `target`.`id` = `source`.`id` AND `target`.`column_value` IS NULL AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) ) );\r\nUPDATE `target_tbl` AS `target`, `source_tbl` AS `source` SET `target`.`id` = `source`.`id`, `target`.`column_value` = `source`.`column_value` WHERE `target`.`id` = `source`.`id` AND `target`.`column_value` IS NULL AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nDELETE `target` FROM `target_tbl` AS `target` WHERE `target`.`id` IN ( ( SELECT `id` FROM `source_tbl` WHERE `source_tbl`.`column_value` = N'name31' ) ) AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nDELETE `target` FROM `target_tbl` AS `target` WHERE `target`.`id` NOT IN ( ( SELECT `id` FROM `source_tbl` ) ) AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nCREATE TEMPORARY TABLE `tmp_source` AS ( SELECT * FROM `source_tbl` AS `source` WHERE `source`.`id` NOT IN ( ( SELECT `id` FROM `target_tbl` ) ) AND `source`.`column_value` = N'name51' );\r\nINSERT INTO `target_tbl` ( `id` ) SELECT `id` FROM `tmp_source`;\r\nDROP TABLE IF EXISTS `tmp_source`;\r\nDROP TEMPORARY TABLE IF EXISTS `tmp_0`;\r\nDROP TEMPORARY TABLE IF EXISTS `top_alias`;\r\nCOMMIT;", command.CommandText);
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
            Assert.AreEqual("START TRANSACTION;\r\nCREATE TEMPORARY TABLE IF NOT EXISTS `top_alias` AS ( SELECT `id` FROM `target_tbl` LIMIT 10 );\r\nCREATE TEMPORARY TABLE `tmp_0` AS ( SELECT `target`.`id` FROM `target_tbl` AS `target` INNER JOIN `source_tbl` AS `source` ON `target`.`id` = `source`.`id` WHERE `target`.`id` = `source`.`id` AND `target`.`column_value` IS NULL AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) ) );\r\nUPDATE `target_tbl` AS `target`, `source_tbl` AS `source` SET `target`.`id` = `source`.`id`, `target`.`column_value` = `source`.`column_value` WHERE `target`.`id` = `source`.`id` AND `target`.`column_value` IS NULL AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nDELETE `target` FROM `target_tbl` AS `target` WHERE `target`.`id` IN ( ( SELECT `id` FROM `source_tbl` WHERE `source_tbl`.`column_value` = N'somename' ) ) AND `target`.`id` IN ( ( SELECT `id` FROM `top_alias` ) );\r\nCREATE TEMPORARY TABLE `tmp_source` AS ( SELECT * FROM `source_tbl` AS `source` WHERE `source`.`id` NOT IN ( ( SELECT `id` FROM `target_tbl` ) ) AND `source`.`column_value` = N'somename' );\r\nINSERT INTO `target_tbl` ( `id` ) SELECT `id` FROM `tmp_source`;\r\nDROP TABLE IF EXISTS `tmp_source`;\r\nDROP TEMPORARY TABLE IF EXISTS `tmp_0`;\r\nDROP TEMPORARY TABLE IF EXISTS `top_alias`;\r\nCOMMIT;", command.CommandText);
        }
        #endregion
    }
}
