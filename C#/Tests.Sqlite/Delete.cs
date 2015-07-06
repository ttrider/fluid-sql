using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;

namespace Tests.Sqlite
{
    [TestClass]
    public class Delete
    {
        public static SqliteProvider Provider = new SqliteProvider();

        [TestMethod]
        public void Delete1()
        {
            var statement = Sql.Delete.Top(1).From(Sql.Name("foo.bar"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"foo\".\"bar\" LIMIT 1;", text);
        }

        [TestMethod]
        public void Delete1P()
        {
            var statement = Sql.Delete.From(Sql.Name("foo.bar")).Limit(1).Offset(10).OrderBy(Sql.Name("id"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"foo\".\"bar\" ORDER BY \"id\" ASC LIMIT 1 OFFSET 10;", text);
        }

        [TestMethod]
        public void DeleteWhere()
        {
            var statement = Sql.Delete.From(Sql.Name("foo.bar")).Where(Sql.Name("f").IsEqual(Sql.Name("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"foo\".\"bar\" WHERE \"f\" = \"b\";", text);
        }
    
    }
}
