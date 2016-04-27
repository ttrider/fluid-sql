// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;

namespace TTRider.FluidSql
{
    public class Parameter : TypedToken, IEquatable<Parameter>
    {
        public Parameter(string name, SqlDbType sqlDbType, byte precision, byte scale)
            : base(name, sqlDbType, precision, scale)
        {
            this.Parameters.Add(this);
        }

        public Parameter(string name, SqlDbType sqlDbType, int length)
            : base(name, sqlDbType, length)
        {
            this.Parameters.Add(this);
        }

        public Parameter(string name, SqlDbType sqlDbType)
            : base(name, sqlDbType)
        {
            this.Parameters.Add(this);
        }

        public Parameter(string name, CommonDbType dbType, byte precision, byte scale)
            : base(name, dbType, precision, scale)
        {
            this.Parameters.Add(this);
        }

        public Parameter(string name, CommonDbType dbType, int length)
            : base(name, dbType, length)
        {
            this.Parameters.Add(this);
        }

        public Parameter(string name, CommonDbType dbType)
            : base(name, dbType)
        {
            this.Parameters.Add(this);
        }

        public Parameter(string name)
            : base(name)
        {
            this.Parameters.Add(this);
        }

        public object DefaultValue { get; set; }

        public ParameterDirection Direction { get; set; }

        /// <summary>
        ///     indicate that procedure's default value needs to be used
        /// </summary>
        public bool UseDefault { get; set; }

        public bool ReadOnly { get; set; }

        public bool Equals(Parameter obj)
        {
            if (obj == null) return false;

            return string.Equals(this.Name, obj.Name, StringComparison.CurrentCultureIgnoreCase);
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as Parameter);
        }

        public override int GetHashCode()
        {
            return string.IsNullOrWhiteSpace(this.Name) ? 0 : this.Name.GetHashCode();
        }


        public static Parameter Any(string name)
        {
            return new Parameter(name);
        }

        public static Parameter Any(string name, object defaultValue)
        {
            return new Parameter(name) { DefaultValue = defaultValue };
        }

        public static Parameter BigInt(string name)
        {
            return new Parameter(name, CommonDbType.BigInt);
        }

        public static Parameter Binary(string name, int length)
        {
            return new Parameter(name, CommonDbType.Binary, length);
        }

        public static Parameter Bit(string name)
        {
            return new Parameter(name, CommonDbType.Bit);
        }

        public static Parameter Char(string name, int length)
        {
            return new Parameter(name, CommonDbType.Char, length);
        }

        public static Parameter DateTime(string name)
        {
            return new Parameter(name, CommonDbType.DateTime);
        }

        public static Parameter Decimal(string name, int length, byte precision = 18, byte scale = 0)
        {
            return new Parameter(name, CommonDbType.Decimal, precision, scale);
        }

        public static Parameter Float(string name)
        {
            return new Parameter(name, CommonDbType.Float);
        }

        public static Parameter Image(string name)
        {
            return new Parameter(name, CommonDbType.Image);
        }

        public static Parameter Int(string name)
        {
            return new Parameter(name, CommonDbType.Int);
        }

        public static Parameter Money(string name)
        {
            return new Parameter(name, CommonDbType.Money);
        }

        public static Parameter NChar(string name, int length)
        {
            return new Parameter(name, CommonDbType.NChar, length);
        }

        public static Parameter NText(string name)
        {
            return new Parameter(name, CommonDbType.NText);
        }

        public static Parameter NVarChar(string name, int length = -1)
        {
            return new Parameter(name, CommonDbType.NVarChar, length);
        }

        public static Parameter String(string name)
        {
            return new Parameter(name, CommonDbType.NVarChar, -1);
        }

        public static Parameter Real(string name)
        {
            return new Parameter(name, CommonDbType.Real);
        }

        public static Parameter UniqueIdentifier(string name)
        {
            return new Parameter(name, CommonDbType.UniqueIdentifier);
        }

        public static Parameter SmallDateTime(string name)
        {
            return new Parameter(name, CommonDbType.SmallDateTime);
        }

        public static Parameter SmallInt(string name)
        {
            return new Parameter(name, CommonDbType.SmallInt);
        }

        public static Parameter SmallMoney(string name)
        {
            return new Parameter(name, CommonDbType.SmallMoney);
        }

        public static Parameter Text(string name)
        {
            return new Parameter(name, CommonDbType.Text);
        }

        public static Parameter Timestamp(string name)
        {
            return new Parameter(name, CommonDbType.Timestamp);
        }

        public static Parameter TinyInt(string name)
        {
            return new Parameter(name, CommonDbType.TinyInt);
        }

        public static Parameter VarBinary(string name, int length = -1)
        {
            return new Parameter(name, CommonDbType.VarBinary, length);
        }

        public static Parameter VarChar(string name, int length = -1)
        {
            return new Parameter(name, CommonDbType.VarChar, length);
        }

        public static Parameter Variant(string name)
        {
            return new Parameter(name, CommonDbType.Variant);
        }

        public static Parameter Xml(string name)
        {
            return new Parameter(name, CommonDbType.Xml);
        }

        public static Parameter Date(string name)
        {
            return new Parameter(name, CommonDbType.Date);
        }

        public static Parameter Time(string name, int length = 7)
        {
            return new Parameter(name, CommonDbType.Time, length);
        }

        public static Parameter DateTime2(string name, int length = 7)
        {
            return new Parameter(name, CommonDbType.DateTime2, length);
        } //0..7

        public static Parameter DateTimeOffset(string name, int length = 7)
        {
            return new Parameter(name, CommonDbType.DateTimeOffset, length);
        } //0..7

        public Parameter Clone()
        {
            var copy = (Parameter) MemberwiseClone();
            copy.Parameters.Clear();
            copy.ParameterValues.Clear();
            return copy;
        }
    }

    public class ParameterEqualityComparer : EqualityComparer<Parameter>
    {
        public override bool Equals(Parameter x, Parameter y)
        {
            return String.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode(Parameter obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }

    public class ParameterFactory
    {
        public Parameter Any(string name)
        {
            return new Parameter(name);
        }

        public Parameter Any(string name, object defaultValue)
        {
            return new Parameter(name) { DefaultValue = defaultValue };
        }

        public Parameter BigInt(string name)
        {
            return new Parameter(name, CommonDbType.BigInt);
        }

        public Parameter Binary(string name, int length)
        {
            return new Parameter(name, CommonDbType.Binary, length);
        }

        public Parameter Bit(string name)
        {
            return new Parameter(name, CommonDbType.Bit);
        }

        public Parameter Char(string name, int length)
        {
            return new Parameter(name, CommonDbType.Char, length);
        }

        public Parameter DateTime(string name)
        {
            return new Parameter(name, CommonDbType.DateTime);
        }

        public Parameter Decimal(string name, int length, byte precision = 18, byte scale = 0)
        {
            return new Parameter(name, CommonDbType.Decimal, precision, scale);
        }

        public Parameter Float(string name)
        {
            return new Parameter(name, CommonDbType.Float);
        }

        public Parameter Image(string name)
        {
            return new Parameter(name, CommonDbType.Image);
        }

        public Parameter Int(string name)
        {
            return new Parameter(name, CommonDbType.Int);
        }

        public Parameter Money(string name)
        {
            return new Parameter(name, CommonDbType.Money);
        }

        public Parameter NChar(string name, int length)
        {
            return new Parameter(name, CommonDbType.NChar, length);
        }

        public Parameter NText(string name)
        {
            return new Parameter(name, CommonDbType.NText);
        }

        public Parameter NVarChar(string name, int length = -1)
        {
            return new Parameter(name, CommonDbType.NVarChar, length);
        }

        public Parameter String(string name)
        {
            return new Parameter(name, CommonDbType.NVarChar, -1);
        }

        public Parameter Real(string name)
        {
            return new Parameter(name, CommonDbType.Real);
        }

        public Parameter UniqueIdentifier(string name)
        {
            return new Parameter(name, CommonDbType.UniqueIdentifier);
        }

        public Parameter SmallDateTime(string name)
        {
            return new Parameter(name, CommonDbType.SmallDateTime);
        }

        public Parameter SmallInt(string name)
        {
            return new Parameter(name, CommonDbType.SmallInt);
        }

        public Parameter SmallMoney(string name)
        {
            return new Parameter(name, CommonDbType.SmallMoney);
        }

        public Parameter Text(string name)
        {
            return new Parameter(name, CommonDbType.Text);
        }

        public Parameter Timestamp(string name)
        {
            return new Parameter(name, CommonDbType.Timestamp);
        }

        public Parameter TinyInt(string name)
        {
            return new Parameter(name, CommonDbType.TinyInt);
        }

        public Parameter VarBinary(string name, int length = -1)
        {
            return new Parameter(name, CommonDbType.VarBinary, length);
        }

        public Parameter VarChar(string name, int length = -1)
        {
            return new Parameter(name, CommonDbType.VarChar, length);
        }

        public Parameter Variant(string name)
        {
            return new Parameter(name, CommonDbType.Variant);
        }

        public Parameter Xml(string name)
        {
            return new Parameter(name, CommonDbType.Xml);
        }

        public Parameter Date(string name)
        {
            return new Parameter(name, CommonDbType.Date);
        }

        public Parameter Time(string name, int length = 7)
        {
            return new Parameter(name, CommonDbType.Time, length);
        }

        public Parameter DateTime2(string name, int length = 7)
        {
            return new Parameter(name, CommonDbType.DateTime2, length);
        } //0..7

        public Parameter DateTimeOffset(string name, int length = 7)
        {
            return new Parameter(name, CommonDbType.DateTimeOffset, length);
        } //0..7
    }

    public partial class Sql
    {
        private static readonly ParameterFactory ParameterFactory = new ParameterFactory();

        public static ParameterFactory Parameter
        {
            get { return ParameterFactory; }
        }
    }
}