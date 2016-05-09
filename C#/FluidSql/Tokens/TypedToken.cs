// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Data;
using TTRider.FluidSql.Providers.SqlServer;

namespace TTRider.FluidSql
{
    public class TypedToken : ExpressionToken, ITyped
    {
        public TypedToken(string name, SqlDbType sqlDbType, byte precision, byte scale)
            : this(name, sqlDbType)
        {
            this.Name = name;
            this.DbType = SqlServerProvider.SqlDbTypeToCommonDbType[sqlDbType];
            this.Precision = precision;
            this.Scale = scale;
        }

        public TypedToken(string name, SqlDbType sqlDbType, int length)
            : this(name, sqlDbType)
        {
            this.Length = length;
        }

        public TypedToken(string name, SqlDbType sqlDbType)
            : this(name)
        {
            this.DbType = SqlServerProvider.SqlDbTypeToCommonDbType[sqlDbType];
        }

        public TypedToken(string name, CommonDbType dbType, byte precision, byte scale)
            : this(name, dbType)
        {
            this.Name = name;
            this.DbType = dbType;
            this.Precision = precision;
            this.Scale = scale;
        }

        public TypedToken(string name, CommonDbType dbType, int length)
            : this(name, dbType)
        {
            this.Length = length;
        }

        public TypedToken(string name, CommonDbType dbType)
            : this(name)
        {
            this.DbType = dbType;
        }

        public TypedToken(string name)
        {
            this.Name = name;
            this.DbType = null;
        }

        public string Name { get; set; }
        public CommonDbType? DbType { get; set; }
        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
        public int? Length { get; set; }
    }
}