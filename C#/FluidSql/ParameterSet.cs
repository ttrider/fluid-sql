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
    public class ParameterSet : ICollection<Parameter>
    {
        private readonly Dictionary<string, Parameter> items =
            new Dictionary<string, Parameter>(StringComparer.CurrentCultureIgnoreCase);

        public void Add(Parameter item)
        {
            this.Add(item, item.Direction);
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public bool Contains(Parameter item)
        {
            if (item == null) return false;
            return this.items.ContainsKey(item.Name);
        }

        public void CopyTo(Parameter[] array, int arrayIndex)
        {
            this.items.Values.CopyTo(array, arrayIndex);
        }

        public int Count => this.items.Count;

        public bool IsReadOnly => false;

        public bool Remove(Parameter item)
        {
            if (item == null) return false;
            this.items.Remove(item.Name);
            return true;
        }

        public IEnumerator<Parameter> GetEnumerator()
        {
            return this.items.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.items.Values.GetEnumerator();
        }

        public void Add(Parameter item, ParameterDirection direction)
        {
            if (item == null) return;

            item.Direction = direction;

            Parameter value;
            if (!this.items.TryGetValue(item.Name, out value))
            {
                this.items[item.Name] = item;
            }
            else
            {
                if (!value.DbType.HasValue)
                {
                    value.DbType = item.DbType;
                }
                if (!value.Precision.HasValue)
                {
                    value.Precision = item.Precision;
                }
                if (!value.Scale.HasValue)
                {
                    value.Scale = item.Scale;
                }
                if (!value.Length.HasValue)
                {
                    value.Length = item.Length;
                }
                value.Direction = direction;
            }
        }

        #region helpers

        public Parameter AddBigInt(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.BigInt(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddBinary(string name, int length, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Binary(name, length);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddBit(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Bit(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddChar(string name, int length, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Char(name, length);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddDateTime(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.DateTime(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddDecimal(string name, byte precision = 18, byte scale = 0,
            ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Decimal(name, precision, scale);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddFloat(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Float(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddImage(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Image(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddInt(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Int(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddMoney(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Money(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddNChar(string name, int length, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.NChar(name, length);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddNText(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.NText(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddNVarChar(string name, int length = -1,
            ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.NVarChar(name, length);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddString(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.String(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddReal(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Real(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddUniqueIdentifier(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.UniqueIdentifier(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddSmallDateTime(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.SmallDateTime(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddSmallInt(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.SmallInt(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddSmallMoney(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.SmallMoney(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddText(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Text(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddTimestamp(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Timestamp(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddTinyInt(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.TinyInt(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddVarBinary(string name, int length = -1,
            ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.VarBinary(name, length);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddVarChar(string name, int length = -1,
            ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.VarChar(name, length);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddDate(string name, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Date(name);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddTime(string name, int length = 7, ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.Time(name, length);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddDateTime2(string name, int length = 7,
            ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.DateTime2(name, length);
            this.Add(p, direction);
            return p;
        }

        public Parameter AddDateTimeOffset(string name, int length = 7,
            ParameterDirection direction = ParameterDirection.Input)
        {
            var p = Sql.Parameter.DateTimeOffset(name, length);
            this.Add(p, direction);
            return p;
        }

        #endregion helpers
    }
}