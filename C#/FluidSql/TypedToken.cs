using System.Data;

namespace TTRider.FluidSql
{
    public class TypedToken : Token
    {
        public TypedToken(string name, SqlDbType sqlDbType, byte precision, byte scale)
            : this(name, sqlDbType)
        {
            this.Name = name;
            this.DbType = sqlDbType;
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
            this.DbType = sqlDbType;
        }

        public TypedToken(string name)
        {
            this.Name = name;
            this.DbType = null;
        }

        public string Name { get; set; }
        public SqlDbType? DbType { get; set; }
        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
        public int? Length { get; set; }
    }
}