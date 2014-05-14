// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TTRider.FluidSql
{
    public class Sql
    {
        private static readonly Regex ParseName = new Regex(@"(\[(?<name>[^\]]*)]\.?)|((?<name>[^\.]*)\.?)",
            RegexOptions.Compiled);

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
            get { return new SelectStatement(); }
        }

        #region Transaction

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
        #endregion Transaction

        public static StatementsStatement Statements(params IStatement[] statements)
        {
            var statement = new StatementsStatement();
            statement.Statements.AddRange(statements);
            return statement;
        }

        public static StatementsStatement Statements(IEnumerable<IStatement> statements)
        {
            var statement = new StatementsStatement();
            if (statements != null)
            {
                statement.Statements.AddRange(statements);
            }
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

        public static DeleteStatement Delete
        {
            get { return new DeleteStatement(); }
        }


        public static DropTableStatement DropTable(Name name, bool checkExists = false)
        {
            return new DropTableStatement { Name = name, CheckExists = checkExists };
        }

        public static CreateTableStatement CreateTable(Name name, bool checkIfNotExists = false)
        {
            return new CreateTableStatement
            {
                Name = name,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static CommentToken Comment(string comment)
        {
            return new CommentToken{ Content = Snippet(comment)};
        }
        public static CommentStatement Comment(IStatement statement)
        {
            return new CommentStatement { Content = statement };
        }

        public static InsertStatement Insert
        {
            get
            {
                return new InsertStatement();
            }
        }

    #endregion Statements
        public static Name Star(string source = null)
        {
            var name = new Name();

            if (!String.IsNullOrWhiteSpace(source))
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

        public static Snippet Snippet(string value, params Parameter[] parameters)
        {
            var val = new Snippet { Value = value };

            foreach (var p  in parameters)
            {
                val.Parameters.Add(p);
            }
            return val;
        }
        public static Snippet Snippet(string value, IEnumerable<Parameter> parameters)
        {
            var val = new Snippet { Value = value };

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    val.Parameters.Add(p);
                }
            }
            return val;
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

        public static CreateIndexStatement CreateIndex(Name name, Name on = null)
        {
            return new CreateIndexStatement
            {
                Name = name, 
                On = on
            };
        }

        public static AlterIndexStatement AlterIndex(Name name, Name on = null)
        {
            return new AlterIndexStatement
            {
                Name = name,
                On = on
            };
        }

        public static DropIndexStatement DropIndex(Name name, Name on = null)
        {
            return new DropIndexStatement
            {
                Name = name,
                On = on
            };
        }

        public static CreateIndexStatement CreateIndex(string name, string on)
        {
            return new CreateIndexStatement
            {
                Name = Sql.Name(name),
                On = Sql.Name(on)
            };
        }

        public static AlterIndexStatement AlterIndex(string name, string on)
        {
            return new AlterIndexStatement
            {
                Name = Sql.Name(name),
                On = Sql.Name(on)
            };
        }
        public static AlterIndexStatement AlterIndexAll(string on)
        {
            return new AlterIndexStatement
            {
                Name = null,
                On = Sql.Name(on)
            };
        }

        public static DropIndexStatement DropIndex(string name, string on)
        {
            return new DropIndexStatement
            {
                Name = Sql.Name(name),
                On = Sql.Name(on)
            };
        }

    
    }
}
