using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace Tests.MySqlTests
{
    public partial class TableTests
    {
        /*
         * create database fluidsqltests;
         *  create user fluidsqluser;
         *  grant all on fluidsqltests.* to 'fluidsqluser'@'localhost' identified by 'fluidsqlpwd';
         *  grant select, insert, delete, update on fluidsqltests.* to 'fluidsqluser'@'localhost' identified by 'fluidsqlpwd';
         */

        [TestMethod]
        public void EndToEnd00()
        {
            var create = CreateTable00();



            var drop = DropTable00();


            var statements = Sql.Statements(

                create,

                drop
                );


            var cmd = provider.GetCommand(statements, @"Server=localhost; Database=fluidsqltests; Uid=fluidsqluser; Pwd=fluidsqlpwd;");

            cmd.ExecuteNonQuery();

        }
    }
}
