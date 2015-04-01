// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class Top : Token
    {
        public Top(int value, bool percent, bool withTies)
        {
            if (value < 1) throw new ArgumentException("value");
            this.Value = Sql.Scalar(value);
            this.Percent = percent;
            this.WithTies = withTies;
        }

        public Top(Parameter value, bool percent, bool withTies)
        {
            this.Parameters.Add(value);
            this.Value = value;
            this.Percent = percent;
            this.WithTies = withTies;
        }

        public Token Value { get; set; }
        public bool Percent { get; set; }
        public bool WithTies { get; set; }
    }
}