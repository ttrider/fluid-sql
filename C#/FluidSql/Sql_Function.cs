// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public partial class Sql
    {
        public static Function Coalesce(params Token[] arguments)
        {
            var f = new Function
            {
                Name = "COALESCE"
            };
            f.Arguments.AddRange(arguments);
            return f;
        }

        public static Function Coalesce(IEnumerable<Token> arguments)
        {
            var f = new Function
            {
                Name = "COALESCE"
            };
            if (arguments != null)
            {
                f.Arguments.AddRange(arguments);
            }
            return f;
        }

        public static Function NullIf(Token argument1, Token argument2)
        {
            var f = new Function
            {
                Name = "NULLIF"
            };
            f.Arguments.Add(argument1);
            f.Arguments.Add(argument2);
            return f;
        }

        public static Function Function(string name, params Token[] arguments)
        {
            var f = new Function
            {
                Name = name
            };
            f.Arguments.AddRange(arguments);
            return f;
        }

        public static Function Function(string name, IEnumerable<Token> arguments)
        {
            var f = new Function
            {
                Name = name
            };
            if (arguments != null)
            {
                f.Arguments.AddRange(arguments);
            }
            return f;
        }
    }
}
