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

        public static DeleteStatement Output(this DeleteStatement statement, params Token[] columns)
        {
            statement.Output.AddRange(columns);
            return statement;
        }
        public static DeleteStatement Output(this DeleteStatement statement, IEnumerable<Token> columns)
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
                    ?column.Alias
                    :((column is Name)?((Name)column).FullName:null))
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
    }

}

