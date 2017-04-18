// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using Xunit;

namespace xUnit.Redshift
{
    public class FunctionsTest : RedshiftSqlProviderTests
    {
        
       [Fact]
        public void Case()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.Case.When(Sql.Scalar("a").IsEqual(Sql.Scalar("b")), Sql.Scalar("a"))
                    .When(Sql.Scalar("a").NotEqual(Sql.Scalar("b")), Sql.Scalar("b"))
                    .Else(Sql.Scalar("c"))),
                "SELECT CASE WHEN 'a' = 'b' THEN 'a' WHEN 'a' <> 'b' THEN 'b' ELSE 'c' END;"
                );
        }
    }
}
