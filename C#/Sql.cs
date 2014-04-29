// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TTRider.FluidSql
{
    public class Sql
    {
        static readonly Regex ParseName = new Regex(@"(\[(?<name>[^\]]*)]\.?)|((?<name>[^\.]*)\.?)", RegexOptions.Compiled);

        internal static IEnumerable<string> GetParts(string name)
        {
            var match = ParseName.Match(name);
            while (match.Success)
            {
                if (match.Length > 0)
                {
                    yield return match.Groups["name"].Value;
                }
                match = match.NextMatch();
            }
        }

        #region Statements

        public static SelectStatement Select
        {
            get
            {
                return new SelectStatement();
            }
        }


        public static BeginTransactionStatement BeginTransaction(Name name = null, string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description
            };
        }
        public static CommitTransactionStatement CommitTransaction(Name name = null)
        {
            return new CommitTransactionStatement
            {
                Name = name
            };

        }
        public static RollbackTransactionStatement RollbackTransaction(Name name = null)
        {
            return new RollbackTransactionStatement
            {
                Name = name
            };
        }
        public static SaveTransactionStatement SaveTransaction(Name name = null)
        {
            return new SaveTransactionStatement
            {
                Name = name
            };
        }

        public static BeginTransactionStatement BeginTransaction(Parameter parameter, string description = null)
        {
            return new BeginTransactionStatement
            {
                Parameter = parameter,
                Description = description
            };
        }
        public static CommitTransactionStatement CommitTransaction(Parameter parameter)
        {
            return new CommitTransactionStatement
            {
                Parameter = parameter
            };

        }
        public static RollbackTransactionStatement RollbackTransaction(Parameter parameter)
        {
            return new RollbackTransactionStatement
            {
                Parameter = parameter
            };
        }
        public static SaveTransactionStatement SaveTransaction(Parameter parameter)
        {
            return new SaveTransactionStatement
            {
                Parameter = parameter
            };
        }

        public static StatementsStatement Statements(params IStatement[] statements)
        {
            var statement = new StatementsStatement();
            statement.Statements.AddRange(statements);
            return statement;
        }

        public static DeclareStatement Declare(Parameter variable, Token initializer = null)
        {
            return new DeclareStatement
            {
                Variable = variable,
                Initializer = initializer
            };
        }

        public static IfStatement If(Token condition)
        {
            return new IfStatement
            {
                Condition = condition
            };
        }

        #endregion Statements
        public static Name Star(string source = null)
        {
            var name = new Name();

            if (!string.IsNullOrWhiteSpace(source))
            {
                name.Parts.Add(source);
            }
            name.Parts.Add("*");
            return name;
        }

        public static Name Name(params string[] names)
        {
            var mpn = new Name();

            foreach (var name in names)
            {
                mpn.Parts.AddRange(GetParts(name));
            }

            return mpn;
        }

        public static Snippet Snippet(string value)
        {
            return new Snippet { Value = value };
        }
        public static Scalar Scalar(object value)
        {
            return new Scalar { Value = value };
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

        public static Token Group(Token value)
        {
            return new GroupToken { Token = value };
        }


        public static Order Order(Name column, Direction direction = Direction.Asc)
        {
            return new Order { Column = column, Direction = direction };
        }
        public static Order Order(string column, Direction direction = Direction.Asc)
        {
            return new Order { Column = new Name(column), Direction = direction };
        }
    }
}
