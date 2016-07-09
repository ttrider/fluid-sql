using TTRider.FluidSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.MySqlTests
{
    /// <summary>
    /// Summary description for Functions
    /// </summary>
    [TestClass]
    public class FunctionsTest : MySqlProviderTests
    {
        [TestMethod]
        public void GetNow()
        {
            var statement = Sql.Select.Output(Sql.Now(), Sql.GetDate());

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT NOW(), NOW();", command.CommandText);
        }

        [TestMethod]
        public void GetNowUTC()
        {
            var statement = Sql.Select.Output(Sql.Now(true), Sql.GetDate(true));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT UTC_TIMESTAMP(), UTC_TIMESTAMP();", command.CommandText);
        }

        [TestMethod]
        public void GetUuid()
        {
            AssertSql(
                Sql.Select.Output(Sql.UUID(), Sql.NewId()),
                "SELECT UUID(), UUID();"
                );
        }

        [TestMethod]
        public void DatePart()
        {
            AssertSql(Sql.Select.Output(Sql.Year(Sql.Now())), "SELECT YEAR( NOW() );");
            AssertSql(Sql.Select.Output(Sql.Month(Sql.Now())), "SELECT MONTH( NOW() );");
            AssertSql(Sql.Select.Output(Sql.Day(Sql.Now())), "SELECT DAY( NOW() );");
            AssertSql(Sql.Select.Output(Sql.Week(Sql.Now())), "SELECT WEEK( NOW() );");
            AssertSql(Sql.Select.Output(Sql.Hour(Sql.Now())), "SELECT HOUR( NOW() );");
            AssertSql(Sql.Select.Output(Sql.Minute(Sql.Now())), "SELECT MINUTE( NOW() );");
            AssertSql(Sql.Select.Output(Sql.Second(Sql.Now())), "SELECT SECOND( NOW() );");
            AssertSql(Sql.Select.Output(Sql.Millisecond(Sql.Now())), "SELECT MICROSECOND( NOW() ) DIV 1000;");
        }

        [TestMethod]
        public void DateDiff()
        {
            AssertSql(
                Sql.Select.Output(Sql.DurationInYears(Sql.Now(), Sql.Scalar("2012-12-12"))),
                "SELECT TIMESTAMPDIFF( YEAR, NOW(), N'2012-12-12' );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInMonth(Sql.Now(), Sql.Scalar("2012-12-12"))),
                "SELECT TIMESTAMPDIFF( MONTH, NOW(), N'2012-12-12' );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInDays(Sql.Now(), Sql.Scalar("2012-12-12"))),
                "SELECT TIMESTAMPDIFF( DAY, NOW(), N'2012-12-12' );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInWeeks(Sql.Now(), Sql.Scalar("2012-12-12"))),
                "SELECT TIMESTAMPDIFF( WEEK, NOW(), N'2012-12-12' );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInHours(Sql.Now(), Sql.Scalar("2012-12-12"))),
                "SELECT TIMESTAMPDIFF( HOUR, NOW(), N'2012-12-12' );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInMinutes(Sql.Now(), Sql.Scalar("2012-12-12"))),
                "SELECT TIMESTAMPDIFF( MINUTE, NOW(), N'2012-12-12' );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInSeconds(Sql.Now(), Sql.Scalar("2012-12-12"))),
                "SELECT TIMESTAMPDIFF( SECOND, NOW(), N'2012-12-12' );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInMilliseconds(Sql.Now(), Sql.Scalar("2012-12-12"))),
                "SELECT TIMESTAMPDIFF( MICROSECOND, NOW(), N'2012-12-12' ) DIV 1000;"
                );
        }

        [TestMethod]
        public void MakeDate3()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeDate(2015, 1, 1)),
                "SELECT TIMESTAMP( MAKEDATE( 2015, 1 ), MAKETIME( 0, 0, 0 ) );"
                );
        }

        [TestMethod]
        public void MakeDate5()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeDate(2015, 1, 1, 13, 20)),
                "SELECT TIMESTAMP( MAKEDATE( 2015, 1 ), MAKETIME( 13, 20, 0 ) );"
                );
        }

        [TestMethod]
        public void MakeDate6()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeDate(2015, 1, 1, 13, 20, 50)),
                "SELECT TIMESTAMP( MAKEDATE( 2015, 1 ), MAKETIME( 13, 20, 50 ) );"
                );
        }

        [TestMethod]
        public void MakeTime2()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeTime(21, 45)),
                "SELECT MAKETIME ( 21, 45, 0 );"
                );
        }

        [TestMethod]
        public void MakeTime3()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeTime(21, 45, 32)),
                "SELECT MAKETIME ( 21, 45, 32 );"
                );
        }

        [TestMethod]
        public void DateAdd()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeDate(2015, 1, 1, 13, 20, 50).AddDays(1).SubtractWeeks(1)),
                "SELECT ( ( TIMESTAMP( MAKEDATE( 2015, 1 ), MAKETIME( 13, 20, 50 ) ) + INTERVAL 1 DAY ) - INTERVAL 1 WEEK );"
                );
        }
    }
}
