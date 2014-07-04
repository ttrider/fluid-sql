// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace TTRider.FluidSql
{
    public static class FluidExtensions
    {
        public static IDataParameterCollection SetValue(this IDataParameterCollection parameterCollection,
            string parameterName, object value)
        {
            if (parameterCollection == null) throw new ArgumentNullException("parameterCollection");
            if (string.IsNullOrWhiteSpace(parameterName)) throw new ArgumentNullException("parameterName");

            if (parameterCollection.Contains(parameterName))
            {
                ((DbParameter)parameterCollection[parameterName]).Value = value;
            }

            return parameterCollection;
        }

        public static Parameter DefaultValue(this Parameter parameter, object defaultValue)
        {
            parameter.DefaultValue = defaultValue;
            return parameter;
        }

        public static Token As(this Token token, string alias)
        {
            token.Alias = alias;
            return token;
        }

        public static SelectStatement Top(this SelectStatement statement, int value, bool percent = false, bool withTies = false)
        {
            statement.Top = new Top(value, percent, withTies);

            return statement;
        }
        public static DeleteStatement Top(this DeleteStatement statement, int value, bool percent = false)
        {
            statement.Top = new Top(value, percent, false);

            return statement;
        }

        public static InsertStatement Top(this InsertStatement statement, int value, bool percent = false)
        {
            statement.Top = new Top(value, percent, false);

            return statement;
        }

        public static SelectStatement Distinct(this SelectStatement statement, bool distinct = true)
        {
            statement.Distinct = distinct;

            return statement;
        }
        public static SelectStatement All(this SelectStatement statement, bool all = true)
        {
            statement.Distinct = !all;

            return statement;
        }
        
        public static SelectStatement Output(this SelectStatement statement, params Token[] columns)
        {
            statement.Output.AddRange(columns);
            return statement;
        }
        public static SelectStatement Output(this SelectStatement statement, IEnumerable<Token> columns)
        {
            if (columns != null)
            {
                statement.Output.AddRange(columns);
            }
            return statement;
        }

        public static T Output<T>(this T statement, params Name[] columns)
            where T : RecordsetStatement
        {
            statement.Output.AddRange(columns);
            return statement;
        }

        public static T Output<T>(this T statement, IEnumerable<Name> columns)
            where T : RecordsetStatement
        {
            if (columns != null)
            {
                statement.Output.AddRange(columns);
            }
            return statement;
        }

        public static SelectStatement Into(this SelectStatement statement, Name target)
        {
            statement.Into = target;

            return statement;
        }

        public static InsertStatement Into(this InsertStatement statement, Name target)
        {
            statement.Into = target;

            return statement;
        }

        public static SelectStatement GroupBy(this SelectStatement statement, params Name[] columns)
        {
            statement.GroupBy.AddRange(columns);
            return statement;
        }
        public static SelectStatement GroupBy(this SelectStatement statement, IEnumerable<Name> columns)
        {
            if (columns != null)
            {
                statement.GroupBy.AddRange(columns);
            }
            return statement;
        }
        public static SelectStatement GroupBy(this SelectStatement statement, params string[] columns)
        {
            statement.GroupBy.AddRange(columns.Select(name => Sql.Name(name)));
            return statement;
        }
        public static SelectStatement GroupBy(this SelectStatement statement, IEnumerable<string> columns)
        {
            if (columns != null)
            {
                statement.GroupBy.AddRange(columns.Select(name => Sql.Name(name)));
            }
            return statement;
        }
        public static SelectStatement OrderBy(this SelectStatement statement, Name column, Direction direction = Direction.Asc)
        {
            statement.OrderBy.Add(Sql.Order(column, direction));
            return statement;
        }
        public static SelectStatement OrderBy(this SelectStatement statement, string column, Direction direction = Direction.Asc)
        {
            statement.OrderBy.Add(Sql.Order(column, direction));
            return statement;
        }
        public static SelectStatement OrderBy(this SelectStatement statement, params Order[] columns)
        {
            statement.OrderBy.AddRange(columns);
            return statement;
        }
        public static SelectStatement OrderBy(this SelectStatement statement, IEnumerable<Order> columns)
        {
            if (columns != null)
            {
                statement.OrderBy.AddRange(columns);
            }
            return statement;
        }
        public static SelectStatement From(this SelectStatement statement, string name, string alias = null)
        {
            statement.From.Add(Sql.Name(name).As(alias));
            return statement;
        }
        public static SelectStatement From(this SelectStatement statement, Token token)
        {
            statement.From.Add(token);
            return statement;
        }
        public static DeleteStatement From(this DeleteStatement statement, string name, string alias = null)
        {
            statement.From = Sql.Name(name).As(alias);
            return statement;
        }
        public static DeleteStatement From(this DeleteStatement statement, Token token)
        {
            statement.From = token;
            return statement;
        }
        public static InsertStatement From(this InsertStatement statement, RecordsetStatement recordset)
        {
            statement.From = recordset;
            return statement;
        }
        public static Union Union(this RecordsetStatement statement, RecordsetStatement with)
        {
            return new Union(statement, with);
        }
        public static Union UnionAll(this RecordsetStatement statement, RecordsetStatement with)
        {
            return new Union(statement, with, true);
        }
        public static Except Except(this RecordsetStatement statement, RecordsetStatement with)
        {
            return new Except(statement, with);
        }
        public static Intersect Intersect(this RecordsetStatement statement, RecordsetStatement with)
        {
            return new Intersect(statement, with);
        }
        public static SelectStatement WrapAsSelect(this RecordsetStatement statement, string alias)
        {
            return new SelectStatement()
                .From(statement.As(alias))
                .Output(statement.Output.Select(
                column=>!string.IsNullOrWhiteSpace(column.Alias)
                    ? column.Alias                                          // we should use alias    
                    :((column is Name)?((Name)column).Parts.Last():null))   // or the LAST PART of the name
                        .Where(name=>!string.IsNullOrWhiteSpace(name))
                        .Select(name=>Sql.Name(alias, name)));
        }

        public static SelectStatement InnerJoin(this SelectStatement statement, Token source, Token on)
        {
            statement.Joins.Add(new Join
            {
                Type = Joins.Inner,
                Source = source,
                On = on
            });
            return statement;
        }
        public static SelectStatement LeftOuterJoin(this SelectStatement statement, Token source, Token on)
        {
            statement.Joins.Add(new Join
            {
                Type = Joins.LeftOuter,
                Source = source,
                On = on
            });
            return statement;
        }
        public static SelectStatement RightOuterJoin(this SelectStatement statement, Token source, Token on)
        {
            statement.Joins.Add(new Join
            {
                Type = Joins.RightOuter,
                Source = source,
                On = on
            });
            return statement;
        }
        public static SelectStatement FullOuterJoin(this SelectStatement statement, Token source, Token on)
        {
            statement.Joins.Add(new Join
            {
                Type = Joins.FullOuter,
                Source = source,
                On = on
            });
            return statement;
        }
        public static SelectStatement CrossJoin(this SelectStatement statement, Token source)
        {
            statement.Joins.Add(new Join
            {
                Type = Joins.Cross,
                Source = source,
            });
            return statement;
        }

        public static SelectStatement Where(this SelectStatement statement, Token condition)
        {
            statement.Where = condition;
            return statement;
        }
        public static DeleteStatement Where(this DeleteStatement statement, Token condition)
        {
            statement.Where = condition;
            return statement;
        }
        public static CreateIndexStatement Where(this CreateIndexStatement statement, Token condition)
        {
            statement.Where = condition;
            return statement;
        }

        public static Token IsEqual(this Token first, Token second)
        {
            return new IsEqualsToken {First = first, Second = second};
        }
        public static Token NotEqual(this Token first, Token second)
        {
            return new NotEqualToken { First = first, Second = second };
        }
        public static Token Less(this Token first, Token second)
        {
            return new LessToken { First = first, Second = second };
        }
        public static Token LessOrEqual(this Token first, Token second)
        {
            return new LessOrEqualToken { First = first, Second = second };
        }
        public static Token Greater(this Token first, Token second)
        {
            return new GreaterToken { First = first, Second = second };
        }
        public static Token GreaterOrEqual(this Token first, Token second)
        {
            return new GreaterOrEqualToken { First = first, Second = second };
        }
        public static Token And(this Token first, Token second)
        {
            return new AndToken { First = first, Second = second };
        }
        public static Token Or(this Token first, Token second)
        {
            return new OrToken { First = first, Second = second };
        }
        public static Token Plus(this Token first, Token second)
        {
            return new PlusToken { First = first, Second = second };
        }
        public static Token Minus(this Token first, Token second)
        {
            return new MinusToken { First = first, Second = second };
        }
        public static Token Divide(this Token first, Token second)
        {
            return new DivideToken { First = first, Second = second };
        }
        public static Token Module(this Token first, Token second)
        {
            return new ModuleToken { First = first, Second = second };
        }
        public static Token Multiply(this Token first, Token second)
        {
            return new MultiplyToken { First = first, Second = second };
        }

        public static Token Group(this Token token)
        {
            return new GroupToken { Token = token };
        }

        public static Token Not(this Token token)
        {
            return new NotToken { Token = token };
        }
        public static Token IsNull(this Token token)
        {
            return new IsNullToken{ Token = token };
        }
        public static Token IsNotNull(this Token token)
        {
            return new IsNotNullToken { Token = token };
        }
        public static Token Between(this Token token, Token first, Token second)
        {
            return new BetweenToken { Token = token, First = first, Second = second };
        }
        public static Token In(this Token token, params Token[] tokens)
        {
            var value = new InToken {Token = token};
            value.Set.AddRange(tokens);
            return value;
        }
        public static Token NotIn(this Token token, params Token[] tokens)
        {
            var value = new NotInToken { Token = token };
            value.Set.AddRange(tokens);
            return value;
        }

        public static Token Contains(this Token first, Token second)
        {
            return new ContainsToken { First = first, Second = second };
        }
        public static Token StartsWith(this Token first, Token second)
        {
            return new StartsWithToken { First = first, Second = second };
        }
        public static Token EndsWith(this Token first, Token second)
        {
            return new EndsWithToken { First = first, Second = second };
        }


        public static IfStatement Then(this IfStatement statement, params IStatement[] statements)
        {
            statement.Then = new StatementsStatement();
            statement.Then.Statements.AddRange(statements);
            return statement;
        }
        public static IfStatement Then(this IfStatement statement, IEnumerable<IStatement> statements)
        {
            statement.Then = new StatementsStatement();
            if (statements != null)
            {
                statement.Then.Statements.AddRange(statements);
            }
            return statement;
        }
        public static IfStatement Else(this IfStatement statement, params IStatement[] statements)
        {
            statement.Else = new StatementsStatement();
            statement.Else.Statements.AddRange(statements);
            return statement;
        }
        public static IfStatement Else(this IfStatement statement, IEnumerable<IStatement> statements)
        {
            statement.Else = new StatementsStatement();
            if (statements!=null)
            statement.Else.Statements.AddRange(statements);
            return statement;
        }

        public static CommentStatement CommentOut(this IStatement statement)
        {
            return new CommentStatement{Content = statement};
        }
        public static CommentToken CommentOut(this Token token)
        {
            return new CommentToken { Content = token };
        }

        public static StringifyStatement Stringify(this IStatement statement)
        {
            return new StringifyStatement { Content = statement };
        }
        public static StringifyToken Stringify(this Token token)
        {
            return new StringifyToken { Content = token };
        }

        public static InsertStatement DefaultValues(this InsertStatement statement, bool useDefaultValues = true)
        {
            statement.DefaultValues = useDefaultValues;
            return statement;
        }
        
        public static InsertStatement Columns(this InsertStatement statement, params Name[] columns)
        {
            statement.Columns.AddRange(columns);
            return statement;
        }
        public static InsertStatement Columns(this InsertStatement statement, IEnumerable<Name> columns)
        {
            if (columns != null)
            {
                statement.Columns.AddRange(columns);
            }
            return statement;
        }
        public static InsertStatement Columns(this InsertStatement statement, params string[] columns)
        {
            statement.Columns.AddRange(columns.Select(name => Sql.Name(name)));
            return statement;
        }
        public static InsertStatement Columns(this InsertStatement statement, IEnumerable<string> columns)
        {
            if (columns != null)
            {
                statement.Columns.AddRange(columns.Select(name => Sql.Name(name)));
            }
            return statement;
        }

        public static InsertStatement Values(this InsertStatement statement, params Token[] values)
        {
            statement.Values.Add(values);
            return statement;
        }
        public static InsertStatement Values(this InsertStatement statement, IEnumerable<Token> values)
        {
            if (values != null)
            {
                statement.Values.Add(values.ToArray());
            }
            return statement;
        }
        public static InsertStatement Values(this InsertStatement statement, params object[] values)
        {
            statement.Values.Add(values.Select(value => (Token)Sql.Scalar(value)).ToArray());
            return statement;
        }
        public static InsertStatement Values(this InsertStatement statement, IEnumerable<object> values)
        {
            if (values != null)
            {
                statement.Values.Add(values.Select(value => (Token)Sql.Scalar(value)).ToArray());
            }
            return statement;
        }




        public static CreateIndexStatement OnColumn(this CreateIndexStatement statement, Name column, Direction direction = Direction.Asc)
        {
            statement.Columns.Add(Sql.Order(column, direction));
            return statement;
        }
        public static CreateIndexStatement OnColumn(this CreateIndexStatement statement, string column, Direction direction = Direction.Asc)
        {
            statement.Columns.Add(Sql.Order(column, direction));
            return statement;
        }
        public static CreateIndexStatement OnColumn(this CreateIndexStatement statement, params Order[] columns)
        {
            statement.Columns.AddRange(columns);
            return statement;
        }
        public static CreateIndexStatement OnColumn(this CreateIndexStatement statement, IEnumerable<Order> columns)
        {
            if (columns != null)
            {
                statement.Columns.AddRange(columns);
            }
            return statement;
        }

        public static CreateIndexStatement Include(this CreateIndexStatement statement, string column)
        {
            statement.Include.Add(Sql.Name(column));
            return statement;
        }
        public static CreateIndexStatement Include(this CreateIndexStatement statement, params Name[] columns)
        {
            statement.Include.AddRange(columns);
            return statement;
        }
        public static CreateIndexStatement Include(this CreateIndexStatement statement, IEnumerable<Name> columns)
        {
            if (columns != null)
            {
                statement.Include.AddRange(columns);
            }
            return statement;
        }

        public static CreateIndexStatement Unique(this CreateIndexStatement statement, bool unique = true)
        {
            statement.Unique = unique;
            return statement;
        }
        public static CreateIndexStatement Clustered(this CreateIndexStatement statement, bool clustered = true)
        {
            statement.Clustered = clustered;
            return statement;
        }
        public static CreateIndexStatement Nonclustered(this CreateIndexStatement statement, bool nonclustered = true)
        {
            statement.Nonclustered = nonclustered;
            return statement;
        }

        public static AlterIndexStatement Rebuild(this AlterIndexStatement statement)
        {
            statement.Rebuild = true;
            return statement;
        }
        public static AlterIndexStatement Disable(this AlterIndexStatement statement)
        {
            statement.Disable = true;
            return statement;
        }
        public static AlterIndexStatement Reorganize(this AlterIndexStatement statement)
        {
            statement.Reorganize = true;
            return statement;
        }


        public static CreateTableStatement Columns(this CreateTableStatement statement, params TableColumn[] columns)
        {
            statement.Columns.AddRange(columns);
            return statement;
        }
        public static CreateTableStatement Columns(this CreateTableStatement statement, IEnumerable<TableColumn> columns)
        {
            if (columns != null)
            {
                statement.Columns.AddRange(columns);
            }
            return statement;
        }


        public static TableColumn Null(this TableColumn column, bool notNull = false)
        {
            column.Null = new bool?(!notNull);
            return column;
        }


        public static TableColumn NotNull(this TableColumn column)
        {
            column.Null = false;
            return column;
        }

        public static TableColumn Identity(this TableColumn column, int seed = 1, int increment = 1)
        {
            column.Identity.On = true;
            column.Identity.Seed = seed;
            column.Identity.Increment = increment;
            return column;
        }

        public static TableColumn Default(this TableColumn column, object value)
        {
            column.DefaultValue = Sql.Scalar(value);
            return column;
        }

        public static TableColumn Default(this TableColumn column, Scalar value)
        {
            column.DefaultValue = value;
            return column;
        }

        public static TableColumn Sparse(this TableColumn column)
        {
            column.Sparse = true;
            return column;
        }

        public static TableColumn RowGuid(this TableColumn column)
        {
            column.RowGuid = true;
            return column;
        }

        #region PrimaryKey

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, string name, params Order[] columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new IndexDefinition();
            }
            statement.PrimaryKey.Name = Sql.Name(name);
            statement.PrimaryKey.Columns.AddRange(columns);
            return statement;
        }
        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, string name, IEnumerable<Order> columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new IndexDefinition();
            }

            statement.PrimaryKey.Name = Sql.Name(name);
            if (columns != null)
            {
                statement.PrimaryKey.Columns.AddRange(columns);
            }
            return statement;
        }
        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, Name name, params Order[] columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new IndexDefinition();
            }

            statement.PrimaryKey.Name = name;
            statement.PrimaryKey.Columns.AddRange(columns);
            return statement;
        }
        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, Name name, IEnumerable<Order> columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new IndexDefinition();
            }

            statement.PrimaryKey.Name = name;
            if (columns != null)
            {
                statement.PrimaryKey.Columns.AddRange(columns);
            }
            return statement;
        }
        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, string name, bool clustered, params Order[] columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new IndexDefinition();
            }

            statement.PrimaryKey.Clustered = clustered;
            statement.PrimaryKey.Name = Sql.Name(name);
            statement.PrimaryKey.Columns.AddRange(columns);
            return statement;
        }
        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, string name, bool clustered, IEnumerable<Order> columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new IndexDefinition();
            }

            statement.PrimaryKey.Clustered = clustered;
            statement.PrimaryKey.Name = Sql.Name(name);
            if (columns != null)
            {
                statement.PrimaryKey.Columns.AddRange(columns);
            }
            return statement;
        }
        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, Name name, bool clustered, params Order[] columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new IndexDefinition();
            }

            statement.PrimaryKey.Clustered = clustered;
            statement.PrimaryKey.Name = name;
            statement.PrimaryKey.Columns.AddRange(columns);
            return statement;
        }
        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, Name name, bool clustered, IEnumerable<Order> columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new IndexDefinition();
            }

            statement.PrimaryKey.Clustered = clustered;
            statement.PrimaryKey.Name = name;
            if (columns != null)
            {
                statement.PrimaryKey.Columns.AddRange(columns);
            }
            return statement;
        }
        #endregion PrimaryKey

        #region IndexOn

        public static CreateTableStatement IndexOn(this CreateTableStatement statement, string name, params Order[] columns)
        {
            var index = new IndexDefinition {Clustered = false, Name = Sql.Name(name)};
            index.Columns.AddRange(columns);
            statement.Indecies.Add(index);
            return statement;
        }
        public static CreateTableStatement IndexOn(this CreateTableStatement statement, string name, IEnumerable<Order> columns)
        {
            var index = new IndexDefinition {Clustered = false, Name = Sql.Name(name)};

            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            statement.Indecies.Add(index);
            return statement;
        }
        public static CreateTableStatement IndexOn(this CreateTableStatement statement, Name name, params Order[] columns)
        {
            var index = new IndexDefinition {Clustered = false, Name = name};

            index.Columns.AddRange(columns);
            statement.Indecies.Add(index);
            return statement;
        }
        public static CreateTableStatement IndexOn(this CreateTableStatement statement, Name name, IEnumerable<Order> columns)
        {
            var index = new IndexDefinition {Clustered = false, Name = name};

            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            statement.Indecies.Add(index);
            return statement;
        }
        public static CreateTableStatement IndexOn(this CreateTableStatement statement, string name, bool clustered, params Order[] columns)
        {
            var index = new IndexDefinition {Clustered = clustered, Name = Sql.Name(name)};

            index.Columns.AddRange(columns);
            statement.Indecies.Add(index);
            return statement;
        }
        public static CreateTableStatement IndexOn(this CreateTableStatement statement, string name, bool clustered, IEnumerable<Order> columns)
        {
            var index = new IndexDefinition {Clustered = clustered, Name = Sql.Name(name)};

            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            statement.Indecies.Add(index);
            return statement;
        }
        public static CreateTableStatement IndexOn(this CreateTableStatement statement, Name name, bool clustered, params Order[] columns)
        {
            var index = new IndexDefinition {Clustered = clustered, Name = name};

            index.Columns.AddRange(columns);
            statement.Indecies.Add(index);
            return statement;
        }
        public static CreateTableStatement IndexOn(this CreateTableStatement statement, Name name, bool clustered, IEnumerable<Order> columns)
        {
            var index = new IndexDefinition {Clustered = clustered, Name = name};

            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            statement.Indecies.Add(index);
            return statement;
        }
        #endregion IndexOn
    }
}

