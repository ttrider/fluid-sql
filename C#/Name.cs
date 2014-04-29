// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace TTRider.FluidSql
{
    public class Name : Token
    {
        public List<string> Parts { get; private set; }

        public string FullName
        {
            get
            {
                return string.Join(".",
                    this.Parts
                        .SkipWhile(string.IsNullOrWhiteSpace)
                        .Select(item => string.IsNullOrWhiteSpace(item)||(string.Equals(item, "*")) ? item : "[" + item + "]"));
            }
            set
            {
                this.Parts.Clear();
                this.Parts.AddRange(Sql.GetParts(value));
            }
        }

        public Name()
        {
            this.Parts = new List<string>();
        }
        public Name(string name)
        {
            this.Parts = new List<string>(Sql.GetParts(name));
        }
    }
}