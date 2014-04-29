// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

using System;
using System.Data;

namespace TTRider.FluidSql
{
    public class Parameter : Token, IEquatable<Parameter>
    {
        public string Name { get; set; }
        public SqlDbType? DbType { get; set; }
        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
        public int? Length { get; set; }

        public Parameter(string name, SqlDbType sqlDbType, byte precision, byte scale)
            : this(name, sqlDbType)
        {
            this.Name = name;
            this.DbType = sqlDbType;
            this.Precision = precision;
            this.Scale = scale;
        }
        public Parameter(string name, SqlDbType sqlDbType, int length)
            : this(name, sqlDbType)
        {
            this.Length = length;
        }
        public Parameter(string name, SqlDbType sqlDbType)
            : this(name)
        {
            this.DbType = sqlDbType;
        }
        public Parameter(string name)
        {
            this.Name = name;
            this.DbType = null;
            this.Parameters.Add(this);
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as Parameter);
        }

        public override int GetHashCode()
        {
            return string.IsNullOrWhiteSpace(this.Name) ? 0 : this.Name.GetHashCode();
        }

        public bool Equals(Parameter obj)
        {
            if (obj == null) return false;

            return string.Equals(this.Name, obj.Name, StringComparison.CurrentCultureIgnoreCase);
        }


        public static Parameter Any(string name) { return new Parameter(name); }
        public static Parameter BigInt(string name) { return new Parameter(name, SqlDbType.BigInt); }
        public static Parameter Binary(string name, int length) { return new Parameter(name, SqlDbType.Binary, length); }
        public static Parameter Bit(string name) { return new Parameter(name, SqlDbType.Bit); }
        public static Parameter Char(string name, int length) { return new Parameter(name, SqlDbType.Char, length); }
        public static Parameter DateTime(string name) { return new Parameter(name, SqlDbType.DateTime); }
        public static Parameter Decimal(string name, int length, byte precision = 18, byte scale = 0) { return new Parameter(name, SqlDbType.Decimal, precision, scale); }
        public static Parameter Float(string name) { return new Parameter(name, SqlDbType.Float); }
        public static Parameter Image(string name) { return new Parameter(name, SqlDbType.Image); }
        public static Parameter Int(string name) { return new Parameter(name, SqlDbType.Int); }
        public static Parameter Money(string name) { return new Parameter(name, SqlDbType.Money); }
        public static Parameter NChar(string name, int length) { return new Parameter(name, SqlDbType.NChar, length); }
        public static Parameter NText(string name) { return new Parameter(name, SqlDbType.NText); }
        public static Parameter NVarChar(string name, int length = -1) { return new Parameter(name, SqlDbType.NVarChar, length); }
        public static Parameter String(string name) { return new Parameter(name, SqlDbType.NVarChar, -1); }
        public static Parameter Real(string name) { return new Parameter(name, SqlDbType.Real); }
        public static Parameter UniqueIdentifier(string name) { return new Parameter(name, SqlDbType.UniqueIdentifier); }
        public static Parameter SmallDateTime(string name) { return new Parameter(name, SqlDbType.SmallDateTime); }
        public static Parameter SmallInt(string name) { return new Parameter(name, SqlDbType.SmallInt); }
        public static Parameter SmallMoney(string name) { return new Parameter(name, SqlDbType.SmallMoney); }
        public static Parameter Text(string name) { return new Parameter(name, SqlDbType.Text); }
        public static Parameter Timestamp(string name) { return new Parameter(name, SqlDbType.Timestamp); }
        public static Parameter TinyInt(string name) { return new Parameter(name, SqlDbType.TinyInt); }
        public static Parameter VarBinary(string name, int length = -1) { return new Parameter(name, SqlDbType.VarBinary, length); }
        public static Parameter VarChar(string name, int length = -1) { return new Parameter(name, SqlDbType.VarChar, length); }
        public static Parameter Variant(string name) { return new Parameter(name, SqlDbType.Variant); }
        public static Parameter Xml(string name) { return new Parameter(name, SqlDbType.Xml); }
        public static Parameter Udt(string name) { return new Parameter(name, SqlDbType.Udt); }
        public static Parameter Structured(string name) { return new Parameter(name, SqlDbType.Structured); }
        public static Parameter Date(string name) { return new Parameter(name, SqlDbType.Date); }
        public static Parameter Time(string name, int length = 7) { return new Parameter(name, SqlDbType.Time, length); }
        public static Parameter DateTime2(string name, int length = 7) { return new Parameter(name, SqlDbType.DateTime2, length); } //0..7
        public static Parameter DateTimeOffset(string name, int length = 7) { return new Parameter(name, SqlDbType.DateTimeOffset, length); }//0..7
    }
}
