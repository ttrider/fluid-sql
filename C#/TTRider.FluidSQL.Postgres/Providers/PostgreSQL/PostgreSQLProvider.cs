// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql.Providers.PostgreSql
{
    public class PostgreSqlProvider : Postgres.Core.ProviderCore
    {
        protected override VisitorState Compile(IStatement statement)
        {
            return new PostgreSqlVisitor().Compile(statement);
        }
    }
}