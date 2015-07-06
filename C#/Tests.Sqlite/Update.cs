using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;

namespace Tests.Sqlite
{
    [TestClass]
    public class Update
    {
        public static SqliteProvider Provider = new SqliteProvider();

        [TestMethod]
        public void UpdateDefault()
        {
            var statement = Sql.Update("foo.bar").Set(Sql.Name("a"),Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("UPDATE \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b';", text);
        }

        [TestMethod]
        public void UpdateOrFail()
        {
            var statement = Sql.Update("foo.bar").OrFail()
                .Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("UPDATE OR FAIL \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b';", text);
        }

        [TestMethod]
        public void UpdateOrAbort()
        {
            var statement = Sql.Update("foo.bar").OrAbort()
                .Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("UPDATE OR ABORT \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b';", text);
        }

        [TestMethod]
        public void UpdateOrIgnore()
        {
            var statement = Sql.Update("foo.bar").OrIgnore()
                .Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("UPDATE OR IGNORE \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b';", text);
        }

        [TestMethod]
        public void UpdateOrReplace()
        {
            var statement = Sql.Update("foo.bar").OrReplace()
                .Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("UPDATE OR REPLACE \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b';", text);
        }

        [TestMethod]
        public void UpdateOrRollback()
        {
            var statement = Sql.Update("foo.bar").OrRollback()
                .Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("UPDATE OR ROLLBACK \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b';", text);
        }

        [TestMethod]
        public void UpdateWhere()
        {
            var statement = Sql.Update("foo.bar")
                .Set(Sql.Name("a"), Sql.Scalar(1))
                .Set(Sql.Name("c"), Sql.Scalar("1"))
                .Where(Sql.Name("z").IsEqual(Sql.Scalar("b")))
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);

            Assert.AreEqual("UPDATE \"foo\".\"bar\" SET \"a\" = 1, \"c\" = '1' WHERE \"z\" = 'b';", text);
        }
    }
}








