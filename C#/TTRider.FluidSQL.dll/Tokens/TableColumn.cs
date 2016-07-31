// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Data;

namespace TTRider.FluidSql
{
    public class TableColumn : TypedToken
    {
        public TableColumn(string name, SqlDbType sqlDbType, byte precision, byte scale)
            : base(name, sqlDbType, precision, scale)
        {
            this.Identity = new IdentityOptions();
        }

        public TableColumn(string name, SqlDbType sqlDbType, int length)
            : base(name, sqlDbType, length)
        {
            this.Identity = new IdentityOptions();
        }

        public TableColumn(string name, SqlDbType sqlDbType)
            : base(name, sqlDbType)
        {
            this.Identity = new IdentityOptions();
        }

        public TableColumn(string name, CommonDbType dbType, byte precision, byte scale)
            : base(name, dbType, precision, scale)
        {
            this.Identity = new IdentityOptions();
        }

        public TableColumn(string name, CommonDbType dbType, int length)
            : base(name, dbType, length)
        {
            this.Identity = new IdentityOptions();
        }

        public TableColumn(string name, CommonDbType dbType)
            : base(name, dbType)
        {
            this.Identity = new IdentityOptions();
        }


        public Token DefaultValue { get; set; }

        public bool Sparse { get; set; }

        public bool? Null { get; set; }
        public OnConflict? NullConflict { get; set; }

        public bool RowGuid { get; set; }

        public IdentityOptions Identity { get; private set; }

        public Direction? PrimaryKeyDirection { get; set; }
        public OnConflict? PrimaryKeyConflict { get; set; }

        //public static TableColumn Any(string name) { return new TableColumn(name); }
        //public static TableColumn Any(string name, object defaultValue) { return new TableColumn(name) { DefaultValue = defaultValue }; }
        public static TableColumn BigInt(string name)
        {
            return new TableColumn(name, CommonDbType.BigInt);
        }

        public static TableColumn Binary(string name, int length)
        {
            return new TableColumn(name, CommonDbType.Binary, length);
        }

        public static TableColumn Bit(string name)
        {
            return new TableColumn(name, CommonDbType.Bit);
        }

        public static TableColumn Char(string name, int length)
        {
            return new TableColumn(name, CommonDbType.Char, length);
        }

        public static TableColumn DateTime(string name)
        {
            return new TableColumn(name, CommonDbType.DateTime);
        }

        public static TableColumn Decimal(string name, int length, byte precision = 18, byte scale = 0)
        {
            return new TableColumn(name, CommonDbType.Decimal, precision, scale);
        }

        public static TableColumn Float(string name)
        {
            return new TableColumn(name, CommonDbType.Float);
        }

        public static TableColumn Image(string name)
        {
            return new TableColumn(name, CommonDbType.Image);
        }

        public static TableColumn Int(string name)
        {
            return new TableColumn(name, CommonDbType.Int);
        }

        public static TableColumn Money(string name)
        {
            return new TableColumn(name, CommonDbType.Money);
        }

        public static TableColumn NChar(string name, int length)
        {
            return new TableColumn(name, CommonDbType.NChar, length);
        }

        public static TableColumn NText(string name)
        {
            return new TableColumn(name, CommonDbType.NText);
        }

        public static TableColumn NVarChar(string name, int length = -1)
        {
            return new TableColumn(name, CommonDbType.NVarChar, length);
        }

        public static TableColumn String(string name)
        {
            return new TableColumn(name, CommonDbType.NVarChar, -1);
        }

        public static TableColumn Real(string name)
        {
            return new TableColumn(name, CommonDbType.Real);
        }

        public static TableColumn UniqueIdentifier(string name)
        {
            return new TableColumn(name, CommonDbType.UniqueIdentifier);
        }

        public static TableColumn SmallDateTime(string name)
        {
            return new TableColumn(name, CommonDbType.SmallDateTime);
        }

        public static TableColumn SmallInt(string name)
        {
            return new TableColumn(name, CommonDbType.SmallInt);
        }

        public static TableColumn SmallMoney(string name)
        {
            return new TableColumn(name, CommonDbType.SmallMoney);
        }

        public static TableColumn Text(string name)
        {
            return new TableColumn(name, CommonDbType.Text);
        }

        public static TableColumn Timestamp(string name)
        {
            return new TableColumn(name, CommonDbType.Timestamp);
        }

        public static TableColumn TinyInt(string name)
        {
            return new TableColumn(name, CommonDbType.TinyInt);
        }

        public static TableColumn VarBinary(string name, int length = -1)
        {
            return new TableColumn(name, CommonDbType.VarBinary, length);
        }

        public static TableColumn VarChar(string name, int length = -1)
        {
            return new TableColumn(name, CommonDbType.VarChar, length);
        }

        public static TableColumn Variant(string name)
        {
            return new TableColumn(name, CommonDbType.Variant);
        }

        public static TableColumn Xml(string name)
        {
            return new TableColumn(name, CommonDbType.Xml);
        }

        public static TableColumn Date(string name)
        {
            return new TableColumn(name, CommonDbType.Date);
        }

        public static TableColumn Time(string name, int length = 7)
        {
            return new TableColumn(name, CommonDbType.Time, length);
        }

        public static TableColumn DateTime2(string name, int length = 7)
        {
            return new TableColumn(name, CommonDbType.DateTime2, length);
        } //0..7

        public static TableColumn DateTimeOffset(string name, int length = 7)
        {
            return new TableColumn(name, CommonDbType.DateTimeOffset, length);
        } //0..7
    }
}