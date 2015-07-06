using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Sqlite
{
    [TestClass]
    public class SqliteSpecific
    {
        [TestMethod]
        public void Vacuum()
        {
            SqliteAssert.AreEqual(TTRider.FluidSql.Sqlite.Vacuum, "VACUUM;");
        }
    }
}
