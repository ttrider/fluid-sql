// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class ParameterSet : ICollection<Parameter>
    {
        private readonly Dictionary<string, Parameter> items =
            new Dictionary<string, Parameter>(StringComparer.CurrentCultureIgnoreCase);

        public void Add(Parameter item)
        {
            if (item == null) return;

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
            }
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

        public int Count
        {
            get { return this.items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

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
    }
}