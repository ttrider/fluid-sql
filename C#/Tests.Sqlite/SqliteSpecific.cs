// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

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