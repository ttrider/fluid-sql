using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace FluidSqlTests
{
    [TestClass]
    public class Update
    {
        [TestMethod]
        public void UpdateDefault()
        {
            var statement = Sql.Update("foo.bar").Set(Sql.Name("a"),Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("UPDATE [foo].[bar] SET [a] = 1 WHERE [z] = N'b';", command.CommandText);
        }

        [TestMethod]
        public void UpdateOutput()
        {
            var statement = Sql.Update("foo.bar").Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b"))).Output(Sql.Name("inserted","a"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("UPDATE [foo].[bar] SET [a] = 1 OUTPUT [inserted].[a] WHERE [z] = N'b';", command.CommandText);
        }

        [TestMethod]
        public void UpdateOutputInto()
        {
            var statement = Sql.Update("foo.bar").Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b"))).OutputInto("@tempt", Sql.Name("inserted", "a"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("UPDATE [foo].[bar] SET [a] = 1 OUTPUT [inserted].[a] INTO @tempt WHERE [z] = N'b';", command.CommandText);
        }

        [TestMethod]
        public void UpdateJoinOutputInto()
        {
            var statement = Sql.Update("foo.bar")
                .Set(Sql.Name("a"), Sql.Scalar(1))
                .Set(Sql.Name("c"), Sql.Scalar("1"))
                .Where(Sql.Name("z").IsEqual(Sql.Scalar("b")))
                .OutputInto("@tempt", Sql.Name("inserted", "a"))
                .InnerJoin(Sql.Name("b"), Sql.Name("a","aa").IsEqual("b.bb"))
                ;

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("UPDATE [foo].[bar] SET [a] = 1, [c] = N'1' OUTPUT [inserted].[a] INTO @tempt INNER JOIN [b] ON [a].[aa] = [b].[bb] WHERE [z] = N'b';", command.CommandText);
        }
    }
}








