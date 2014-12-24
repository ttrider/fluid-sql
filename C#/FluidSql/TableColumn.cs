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

        public Token DefaultValue { get; set; }

        public bool Sparse { get; set; }

        public bool? Null { get; set; }

        public bool RowGuid { get; set; }

        public IdentityOptions Identity { get; private set; }

        //public static TableColumn Any(string name) { return new TableColumn(name); }
        //public static TableColumn Any(string name, object defaultValue) { return new TableColumn(name) { DefaultValue = defaultValue }; }
        public static TableColumn BigInt(string name)
        {
            return new TableColumn(name, SqlDbType.BigInt);
        }

        public static TableColumn Binary(string name, int length)
        {
            return new TableColumn(name, SqlDbType.Binary, length);
        }

        public static TableColumn Bit(string name)
        {
            return new TableColumn(name, SqlDbType.Bit);
        }

        public static TableColumn Char(string name, int length)
        {
            return new TableColumn(name, SqlDbType.Char, length);
        }

        public static TableColumn DateTime(string name)
        {
            return new TableColumn(name, SqlDbType.DateTime);
        }

        public static TableColumn Decimal(string name, int length, byte precision = 18, byte scale = 0)
        {
            return new TableColumn(name, SqlDbType.Decimal, precision, scale);
        }

        public static TableColumn Float(string name)
        {
            return new TableColumn(name, SqlDbType.Float);
        }

        public static TableColumn Image(string name)
        {
            return new TableColumn(name, SqlDbType.Image);
        }

        public static TableColumn Int(string name)
        {
            return new TableColumn(name, SqlDbType.Int);
        }

        public static TableColumn Money(string name)
        {
            return new TableColumn(name, SqlDbType.Money);
        }

        public static TableColumn NChar(string name, int length)
        {
            return new TableColumn(name, SqlDbType.NChar, length);
        }

        public static TableColumn NText(string name)
        {
            return new TableColumn(name, SqlDbType.NText);
        }

        public static TableColumn NVarChar(string name, int length = -1)
        {
            return new TableColumn(name, SqlDbType.NVarChar, length);
        }

        public static TableColumn String(string name)
        {
            return new TableColumn(name, SqlDbType.NVarChar, -1);
        }

        public static TableColumn Real(string name)
        {
            return new TableColumn(name, SqlDbType.Real);
        }

        public static TableColumn UniqueIdentifier(string name)
        {
            return new TableColumn(name, SqlDbType.UniqueIdentifier);
        }

        public static TableColumn SmallDateTime(string name)
        {
            return new TableColumn(name, SqlDbType.SmallDateTime);
        }

        public static TableColumn SmallInt(string name)
        {
            return new TableColumn(name, SqlDbType.SmallInt);
        }

        public static TableColumn SmallMoney(string name)
        {
            return new TableColumn(name, SqlDbType.SmallMoney);
        }

        public static TableColumn Text(string name)
        {
            return new TableColumn(name, SqlDbType.Text);
        }

        public static TableColumn Timestamp(string name)
        {
            return new TableColumn(name, SqlDbType.Timestamp);
        }

        public static TableColumn TinyInt(string name)
        {
            return new TableColumn(name, SqlDbType.TinyInt);
        }

        public static TableColumn VarBinary(string name, int length = -1)
        {
            return new TableColumn(name, SqlDbType.VarBinary, length);
        }

        public static TableColumn VarChar(string name, int length = -1)
        {
            return new TableColumn(name, SqlDbType.VarChar, length);
        }

        public static TableColumn Variant(string name)
        {
            return new TableColumn(name, SqlDbType.Variant);
        }

        public static TableColumn Xml(string name)
        {
            return new TableColumn(name, SqlDbType.Xml);
        }

        public static TableColumn Udt(string name)
        {
            return new TableColumn(name, SqlDbType.Udt);
        }

        public static TableColumn Structured(string name)
        {
            return new TableColumn(name, SqlDbType.Structured);
        }

        public static TableColumn Date(string name)
        {
            return new TableColumn(name, SqlDbType.Date);
        }

        public static TableColumn Time(string name, int length = 7)
        {
            return new TableColumn(name, SqlDbType.Time, length);
        }

        public static TableColumn DateTime2(string name, int length = 7)
        {
            return new TableColumn(name, SqlDbType.DateTime2, length);
        } //0..7

        public static TableColumn DateTimeOffset(string name, int length = 7)
        {
            return new TableColumn(name, SqlDbType.DateTimeOffset, length);
        } //0..7
    }
}