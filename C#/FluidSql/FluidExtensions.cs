// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
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
        static IEnumerable<Name> ToNames(Name name, params Name[] names)
        {
            if (name != null) yield return name;
            foreach (var nm in names)
            {
                yield return nm;
            }
        }
        static IEnumerable<Name> ToNames(string name, params string[] names)
        {
            if (name != null) yield return name;
            foreach (var nm in names)
            {
                yield return nm;
            }
        }
        static IEnumerable<Name> ToNames(IEnumerable<string> names)
        {
            if (names != null)
            {
                foreach (var nm in names)
                {
                    yield return nm;
                }
            }
        }

        static IEnumerable<Order> ToOrders(Order order, params Order[] orders)
        {
            if (order != null) yield return order;
            foreach (var nm in orders)
            {
                yield return nm;
            }
        }
        static IEnumerable<Order> ToOrders(string order, params string[] orders)
        {
            if (order != null) yield return Sql.Order(order);
            foreach (var nm in orders)
            {
                yield return Sql.Order(nm);
            }
        }


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

        public static T As<T>(this T token, string alias)
            where T : IAliasToken
        {
            if (!string.IsNullOrWhiteSpace(alias))
            {
                token.Alias = alias;
            }
            return token;
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

        public static SelectStatement Output(this SelectStatement statement, params ExpressionToken[] columns)
        {
            statement.Output.AddRange(columns);
            return statement;
        }

        public static SelectStatement Output(this SelectStatement statement, IEnumerable<ExpressionToken> columns)
        {
            if (columns != null)
            {
                statement.Output.AddRange(columns);
            }
            return statement;
        }

        public static T Assign<T>(this T statement, Name target, ExpressionToken expression)
            where T : ISetStatement
        {
            statement.Set.Add(new AssignToken { First = target, Second = expression });
            return statement;
        }

        public static T Set<T>(this T statement, Name target, ExpressionToken expression)
            where T : ISetStatement
        {
            statement.Set.Add(new AssignToken { First = target, Second = expression });
            return statement;
        }

        public static AssignToken SetTo(this Name target, ExpressionToken expression)
        {
            return new AssignToken { First = target, Second = expression };
        }


        public static T PlusAssign<T>(this T statement, Name target, ExpressionToken expression)
            where T : ISetStatement
        {
            statement.Set.Add(new PlusToken { First = target, Second = expression, Equal = true });
            return statement;
        }

        public static T MinusAssign<T>(this T statement, Name target, ExpressionToken expression)
            where T : ISetStatement
        {
            statement.Set.Add(new MinusToken { First = target, Second = expression, Equal = true });
            return statement;
        }

        public static T DivideAssign<T>(this T statement, Name target, ExpressionToken expression)
            where T : ISetStatement
        {
            statement.Set.Add(new DivideToken { First = target, Second = expression, Equal = true });
            return statement;
        }

        public static T BitwiseAndAssign<T>(this T statement, Name target, ExpressionToken expression)
            where T : ISetStatement
        {
            statement.Set.Add(new BitwiseAndToken { First = target, Second = expression, Equal = true });
            return statement;
        }

        public static T BitwiseOrAssign<T>(this T statement, Name target, ExpressionToken expression)
            where T : ISetStatement
        {
            statement.Set.Add(new BitwiseOrToken { First = target, Second = expression, Equal = true });
            return statement;
        }

        public static T BitwiseXorAssign<T>(this T statement, Name target, ExpressionToken expression)
            where T : ISetStatement
        {
            statement.Set.Add(new BitwiseXorToken { First = target, Second = expression, Equal = true });
            return statement;
        }

        public static T ModuloAssign<T>(this T statement, Name target, ExpressionToken expression)
            where T : ISetStatement
        {
            statement.Set.Add(new ModuloToken { First = target, Second = expression, Equal = true });
            return statement;
        }

        public static T MultiplyAssign<T>(this T statement, Name target, ExpressionToken expression)
            where T : ISetStatement
        {
            statement.Set.Add(new MultiplyToken { First = target, Second = expression, Equal = true });
            return statement;
        }


        public static SelectStatement BitwiseNotAssign(this SelectStatement statement, Name target, ExpressionToken expression)
        {
            statement.Set.Add(new BitwiseNotToken { First = target, Second = expression, Equal = true });
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
        public static T Output<T>(this T statement, IEnumerable<string> columns)
            where T : RecordsetStatement
        {
            if (columns != null)
            {
                statement.Output.AddRange(columns.Select(s => Sql.Name(s)));
            }
            return statement;
        }

        public static T OutputInto<T>(this T statement, Name target, params Name[] columns)
            where T : RecordsetStatement
        {
            statement.Output.AddRange(columns);
            statement.OutputInto = target;
            return statement;
        }

        public static T OutputInto<T>(this T statement, Name target, IEnumerable<Name> columns)
            where T : RecordsetStatement
        {
            if (columns != null)
            {
                statement.Output.AddRange(columns);
            }
            statement.OutputInto = target;
            return statement;
        }
        public static T OutputInto<T>(this T statement, Name target, IEnumerable<string> columns)
            where T : RecordsetStatement
        {
            if (columns != null)
            {
                statement.Output.AddRange(columns.Select(s => Sql.Name(s)));
            }
            statement.OutputInto = target;
            return statement;
        }

        public static T Into<T>(this T statement, Name target)
            where T : IIntoStatement
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

        public static T OrderBy<T>(this T statement, Name column,
            Direction direction = Direction.Asc)
            where T : IOrderByStatement
        {
            statement.OrderBy.Add(Sql.Order(column, direction));
            return statement;
        }

        public static T OrderBy<T>(this T statement, string column,
            Direction direction = Direction.Asc)
        where T : IOrderByStatement
        {
            statement.OrderBy.Add(Sql.Order(column, direction));
            return statement;
        }

        public static T OrderBy<T>(this T statement, params Order[] columns)
        where T : IOrderByStatement
        {
            statement.OrderBy.AddRange(columns);
            return statement;
        }

        public static T OrderBy<T>(this T statement, IEnumerable<Order> columns)
        where T : IOrderByStatement
        {
            if (columns != null)
            {
                statement.OrderBy.AddRange(columns);
            }
            return statement;
        }

        public static SelectStatement From(this SelectStatement statement, string name, string alias = null)
        {
            statement.From.Add(new RecordsetSourceToken { Source = Sql.Name(name), Alias = alias });
            return statement;
        }

        public static SelectStatement From(this SelectStatement statement, RecordsetSourceToken token, string alias = null)
        {
            statement.From.Add(token.As(alias));
            return statement;
        }

        public static DeleteStatement From(this DeleteStatement statement, Token token)
        {
            statement.RecordsetSource = new RecordsetSourceToken { Source = token };
            return statement;
        }
        public static MergeStatement Using(this MergeStatement statement, RecordsetStatement token, string alias = null)
        {
            statement.Using = token.As(alias);
            return statement;
        }
        public static MergeStatement Using(this MergeStatement statement, Name token, string alias = null)
        {
            statement.Using = token.As(alias);
            return statement;
        }
        public static MergeStatement On(this MergeStatement statement, Token token)
        {
            statement.On = token;
            return statement;
        }


        public static MergeStatement WhenMatchedThenDelete(this MergeStatement statement, Token andCondition = null)
        {
            statement.WhenMatched.Add(new WhenMatchedTokenThenDeleteToken
            {
                AndCondition = andCondition
            });
            return statement;
        }
        public static MergeStatement WhenMatchedThenUpdateSet(this MergeStatement statement, IEnumerable<AssignToken> set)
        {
            var wm = new WhenMatchedTokenThenUpdateSetToken();

            if (set != null)
            {
                foreach (var columnValue in set)
                {
                    wm.Set.Add(columnValue);
                }
            }

            statement.WhenMatched.Add(wm);
            return statement;
        }
        public static MergeStatement WhenMatchedThenUpdateSet(this MergeStatement statement, params AssignToken[] set)
        {
            return WhenMatchedThenUpdateSet(statement, (IEnumerable<AssignToken>)set);
        }
        public static MergeStatement WhenMatchedThenUpdateSet(this MergeStatement statement, Token andCondition, IEnumerable<AssignToken> set)
        {
            var wm = new WhenMatchedTokenThenUpdateSetToken
            {
                AndCondition = andCondition
            };
            if (set != null)
            {
                foreach (var columnValue in set)
                {
                    wm.Set.Add(columnValue);
                }
            }

            statement.WhenMatched.Add(wm);
            return statement;
        }
        public static MergeStatement WhenMatchedThenUpdateSet(this MergeStatement statement, Token andCondition, params AssignToken[] set)
        {
            return WhenMatchedThenUpdateSet(statement, andCondition, (IEnumerable<AssignToken>)set);
        }
        public static MergeStatement WhenNotMatchedBySourceThenDelete(this MergeStatement statement, Token andCondition = null)
        {
            statement.WhenNotMatchedBySource.Add(new WhenMatchedTokenThenDeleteToken
            {
                AndCondition = andCondition
            });
            return statement;
        }
        public static MergeStatement WhenNotMatchedBySourceThenUpdate(this MergeStatement statement, Token andCondition, IEnumerable<AssignToken> set)
        {
            var wm = new WhenMatchedTokenThenUpdateSetToken
            {
                AndCondition = andCondition
            };
            if (set != null)
            {
                foreach (var columnValue in set)
                {
                    wm.Set.Add(columnValue);
                }
            }
            statement.WhenNotMatchedBySource.Add(wm);
            return statement;
        }
        public static MergeStatement WhenNotMatchedBySourceThenUpdate(this MergeStatement statement, Token andCondition, params AssignToken[] set)
        {
            return WhenNotMatchedBySourceThenUpdate(statement, andCondition, (IEnumerable<AssignToken>)set);
        }
        public static MergeStatement WhenNotMatchedBySourceThenUpdate(this MergeStatement statement, params AssignToken[] set)
        {
            return WhenNotMatchedBySourceThenUpdate(statement, null, (IEnumerable<AssignToken>)set);
        }
        public static MergeStatement WhenNotMatchedBySourceThenUpdate(this MergeStatement statement, IEnumerable<AssignToken> set)
        {
            return WhenNotMatchedBySourceThenUpdate(statement, null, set);
        }

        public static MergeStatement WhenNotMatchedThenInsert(this MergeStatement statement, IEnumerable<Name> columns)
        {
            var wm = new WhenNotMatchedTokenThenInsertToken();

            if (columns != null)
            {
                foreach (var column in columns)
                {
                    wm.Columns.Add(column);
                }
            }
            statement.WhenNotMatched.Add(wm);
            return statement;
        }
        public static MergeStatement WhenNotMatchedThenInsert(this MergeStatement statement, Token andCondition, IEnumerable<Name> columns)
        {
            var wm = new WhenNotMatchedTokenThenInsertToken
            {
                AndCondition = andCondition
            };
            if (columns != null)
            {
                foreach (var column in columns)
                {
                    wm.Columns.Add(column);
                }
            }
            statement.WhenNotMatched.Add(wm);
            return statement;
        }
        public static MergeStatement WhenNotMatchedThenInsert(this MergeStatement statement, IEnumerable<Name> columns, IEnumerable<Name> values)
        {
            var wm = new WhenNotMatchedTokenThenInsertToken();

            if (columns != null)
            {
                foreach (var column in columns)
                {
                    wm.Columns.Add(column);
                }
            }

            if (values != null)
            {
                foreach (var value in values)
                {
                    wm.Values.Add(value);
                }
            }
            statement.WhenNotMatched.Add(wm);
            return statement;
        }
        public static MergeStatement WhenNotMatchedThenInsert(this MergeStatement statement, Token andCondition, IEnumerable<Name> columns, IEnumerable<Name> values)
        {
            var wm = new WhenNotMatchedTokenThenInsertToken
            {
                AndCondition = andCondition
            };
            if (columns != null)
            {
                foreach (var column in columns)
                {
                    wm.Columns.Add(column);
                }
            }
            if (values != null)
            {
                foreach (var value in values)
                {
                    wm.Values.Add(value);
                }
            }

            statement.WhenNotMatched.Add(wm);
            return statement;
        }
        public static MergeStatement WhenNotMatchedThenInsert(this MergeStatement statement, params Name[] columns)
        {
            var wm = new WhenNotMatchedTokenThenInsertToken();

            if (columns != null)
            {
                foreach (var column in columns)
                {
                    wm.Columns.Add(column);
                }
            }
            statement.WhenNotMatched.Add(wm);
            return statement;
        }
        public static MergeStatement WhenNotMatchedThenInsert(this MergeStatement statement, Token andCondition, params Name[] columns)
        {
            var wm = new WhenNotMatchedTokenThenInsertToken
            {
                AndCondition = andCondition
            };
            if (columns != null)
            {
                foreach (var column in columns)
                {
                    wm.Columns.Add(column);
                }
            }
            statement.WhenNotMatched.Add(wm);
            return statement;
        }

        public static InsertStatement From(this InsertStatement statement, RecordsetStatement recordset)
        {
            statement.From = recordset;
            return statement;
        }

        public static UnionStatement Union(this RecordsetStatement statement, RecordsetStatement with)
        {
            return new UnionStatement(statement, with);
        }

        public static UnionStatement UnionAll(this RecordsetStatement statement, RecordsetStatement with)
        {
            return new UnionStatement(statement, with, true);
        }

        public static ExceptStatement Except(this RecordsetStatement statement, RecordsetStatement with)
        {
            return new ExceptStatement(statement, with);
        }

        public static IntersectStatement Intersect(this RecordsetStatement statement, RecordsetStatement with)
        {
            return new IntersectStatement(statement, with);
        }

        public static SelectStatement WrapAsSelect(this RecordsetStatement statement, string alias)
        {
            return new SelectStatement()
                .From(statement, alias)
                .Output(statement.Output.Select(
                    column => !string.IsNullOrWhiteSpace(column.Alias)
                        ? column.Alias // we should use alias    
                        : ((column is Name) ? ((Name)column).LastPart : null)) // or the LAST PART of the name
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Select(name => Sql.Name(alias, name)));
        }

        #region INNER JOIN

        public static T Join<T>(this T statement, Joins join, RecordsetSourceToken source, ExpressionToken on)
            where T : IJoinStatement
        {
            statement.Joins.Add(new Join
            {
                Type = join,
                Source = source,
                On = on
            });
            return statement;
        }


        public static T InnerJoin<T>(this T statement, RecordsetSourceToken source, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.Inner, source, on);
        }
        public static T InnerJoin<T>(this T statement, RecordsetSourceToken source, string alias, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.Inner, source.As(alias), on);
        }
        public static T InnerJoin<T>(this T statement, string source, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.Inner, Sql.Name(source), on);
        }
        public static T InnerJoin<T>(this T statement, string source, string alias, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.Inner, Sql.Name(source).As(alias), on);
        }

        public static T LeftOuterJoin<T>(this T statement, RecordsetSourceToken source, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.LeftOuter, source, on);
        }
        public static T LeftOuterJoin<T>(this T statement, RecordsetSourceToken source, string alias, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.LeftOuter, source.As(alias), on);
        }
        public static T LeftOuterJoin<T>(this T statement, string source, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.LeftOuter, Sql.Name(source), on);
        }
        public static T LeftOuterJoin<T>(this T statement, string source, string alias, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.LeftOuter, Sql.Name(source).As(alias), on);
        }

        public static T RightOuterJoin<T>(this T statement, RecordsetSourceToken source, ExpressionToken on)
                   where T : IJoinStatement
        {
            return Join(statement, Joins.RightOuter, source, on);
        }
        public static T RightOuterJoin<T>(this T statement, RecordsetSourceToken source, string alias, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.RightOuter, source.As(alias), on);
        }
        public static T RightOuterJoin<T>(this T statement, string source, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.RightOuter, Sql.Name(source), on);
        }
        public static T RightOuterJoin<T>(this T statement, string source, string alias, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.RightOuter, Sql.Name(source).As(alias), on);
        }


        public static T FullOuterJoin<T>(this T statement, RecordsetSourceToken source, ExpressionToken on)
                           where T : IJoinStatement
        {
            return Join(statement, Joins.FullOuter, source, on);
        }
        public static T FullOuterJoin<T>(this T statement, RecordsetSourceToken source, string alias, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.FullOuter, source.As(alias), on);
        }
        public static T FullOuterJoin<T>(this T statement, string source, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.FullOuter, Sql.Name(source), on);
        }
        public static T FullOuterJoin<T>(this T statement, string source, string alias, ExpressionToken on)
            where T : IJoinStatement
        {
            return Join(statement, Joins.FullOuter, Sql.Name(source).As(alias), on);
        }



        public static T CrossJoin<T>(this T statement, RecordsetSourceToken source)
                           where T : IJoinStatement
        {
            return Join(statement, Joins.Cross, source, null);
        }
        public static T CrossJoin<T>(this T statement, RecordsetSourceToken source, string alias)
            where T : IJoinStatement
        {
            return Join(statement, Joins.Cross, source.As(alias), null);
        }
        public static T CrossJoin<T>(this T statement, string source)
            where T : IJoinStatement
        {
            return Join(statement, Joins.Cross, Sql.Name(source), null);
        }
        public static T CrossJoin<T>(this T statement, string source, string alias)
            where T : IJoinStatement
        {
            return Join(statement, Joins.Cross, Sql.Name(source).As(alias), null);
        }
        #endregion INNER JOIN

        public static SelectStatement Having(this SelectStatement statement, Token condition)
        {
            statement.Having = condition;
            return statement;
        }

        public static T Where<T>(this T statement, Token condition)
            where T : IWhereStatement
        {
            statement.Where = condition;
            return statement;
        }


        public static ExpressionToken IsEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new IsEqualsToken { First = first, Second = second };
        }

        public static ExpressionToken NotEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new NotEqualToken { First = first, Second = second };
        }

        public static ExpressionToken Less(this ExpressionToken first, ExpressionToken second)
        {
            return new LessToken { First = first, Second = second };
        }

        public static ExpressionToken NotLess(this ExpressionToken first, ExpressionToken second)
        {
            return new NotLessToken { First = first, Second = second };
        }

        public static ExpressionToken LessOrEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new LessOrEqualToken { First = first, Second = second };
        }

        public static ExpressionToken Greater(this ExpressionToken first, ExpressionToken second)
        {
            return new GreaterToken { First = first, Second = second };
        }

        public static ExpressionToken NotGreater(this ExpressionToken first, ExpressionToken second)
        {
            return new NotGreaterToken { First = first, Second = second };
        }

        public static ExpressionToken GreaterOrEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new GreaterOrEqualToken { First = first, Second = second };
        }

        public static ExpressionToken And(this ExpressionToken first, ExpressionToken second)
        {
            return new AndToken { First = first, Second = second };
        }

        public static ExpressionToken Or(this ExpressionToken first, ExpressionToken second)
        {
            return new OrToken { First = first, Second = second };
        }

        public static ExpressionToken PlusEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new PlusToken { First = first, Second = second, Equal = true };
        }

        public static ExpressionToken MinusEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new MinusToken { First = first, Second = second, Equal = true };
        }

        public static ExpressionToken DivideEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new DivideToken { First = first, Second = second, Equal = true };
        }

        public static ExpressionToken BitwiseAndEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new BitwiseAndToken { First = first, Second = second, Equal = true };
        }

        public static ExpressionToken BitwiseOrEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new BitwiseOrToken { First = first, Second = second, Equal = true };
        }

        public static ExpressionToken BitwiseXorEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new BitwiseXorToken { First = first, Second = second, Equal = true };
        }

        public static ExpressionToken BitwiseNotEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new BitwiseNotToken { First = first, Second = second, Equal = true };
        }

        public static ExpressionToken ModuloEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new ModuloToken { First = first, Second = second, Equal = true };
        }

        public static ExpressionToken MultiplyEqual(this ExpressionToken first, ExpressionToken second)
        {
            return new MultiplyToken { First = first, Second = second, Equal = true };
        }

        public static ExpressionToken Plus(this ExpressionToken first, ExpressionToken second)
        {
            return new PlusToken { First = first, Second = second };
        }

        public static ExpressionToken Minus(this ExpressionToken first, ExpressionToken second)
        {
            return new MinusToken { First = first, Second = second };
        }

        public static ExpressionToken Divide(this ExpressionToken first, ExpressionToken second)
        {
            return new DivideToken { First = first, Second = second };
        }

        public static ExpressionToken BitwiseAnd(this ExpressionToken first, ExpressionToken second)
        {
            return new BitwiseAndToken { First = first, Second = second };
        }

        public static ExpressionToken BitwiseOr(this ExpressionToken first, ExpressionToken second)
        {
            return new BitwiseOrToken { First = first, Second = second };
        }

        public static ExpressionToken BitwiseXor(this ExpressionToken first, ExpressionToken second)
        {
            return new BitwiseXorToken { First = first, Second = second };
        }

        public static ExpressionToken BitwiseNot(this ExpressionToken first, ExpressionToken second)
        {
            return new BitwiseNotToken { First = first, Second = second };
        }

        public static ExpressionToken Modulo(this ExpressionToken first, ExpressionToken second)
        {
            return new ModuloToken { First = first, Second = second };
        }

        public static MultiplyToken Multiply(this ExpressionToken first, ExpressionToken second)
        {
            return new MultiplyToken { First = first, Second = second };
        }


        public static ExpressionToken IsEqual(this ExpressionToken first, string second)
        {
            return new IsEqualsToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken NotEqual(this ExpressionToken first, string second)
        {
            return new NotEqualToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken Less(this ExpressionToken first, string second)
        {
            return new LessToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken NotLess(this ExpressionToken first, string second)
        {
            return new NotLessToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken LessOrEqual(this ExpressionToken first, string second)
        {
            return new LessOrEqualToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken Greater(this ExpressionToken first, string second)
        {
            return new GreaterToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken NotGreater(this ExpressionToken first, string second)
        {
            return new NotGreaterToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken GreaterOrEqual(this ExpressionToken first, string second)
        {
            return new GreaterOrEqualToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken And(this ExpressionToken first, string second)
        {
            return new AndToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken Or(this ExpressionToken first, string second)
        {
            return new OrToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken PlusEqual(this ExpressionToken first, string second)
        {
            return new PlusToken { First = first, Second = Sql.Name(second), Equal = true };
        }

        public static ExpressionToken MinusEqual(this ExpressionToken first, string second)
        {
            return new MinusToken { First = first, Second = Sql.Name(second), Equal = true };
        }

        public static ExpressionToken DivideEqual(this ExpressionToken first, string second)
        {
            return new DivideToken { First = first, Second = Sql.Name(second), Equal = true };
        }

        public static ExpressionToken BitwiseAndEqual(this ExpressionToken first, string second)
        {
            return new BitwiseAndToken { First = first, Second = Sql.Name(second), Equal = true };
        }

        public static ExpressionToken BitwiseOrEqual(this ExpressionToken first, string second)
        {
            return new BitwiseOrToken { First = first, Second = Sql.Name(second), Equal = true };
        }

        public static ExpressionToken BitwiseXorEqual(this ExpressionToken first, string second)
        {
            return new BitwiseXorToken { First = first, Second = Sql.Name(second), Equal = true };
        }

        public static ExpressionToken BitwiseNotEqual(this ExpressionToken first, string second)
        {
            return new BitwiseNotToken { First = first, Second = Sql.Name(second), Equal = true };
        }

        public static ExpressionToken ModuloEqual(this ExpressionToken first, string second)
        {
            return new ModuloToken { First = first, Second = Sql.Name(second), Equal = true };
        }

        public static ExpressionToken MultiplyEqual(this ExpressionToken first, string second)
        {
            return new MultiplyToken { First = first, Second = Sql.Name(second), Equal = true };
        }

        public static ExpressionToken Plus(this ExpressionToken first, string second)
        {
            return new PlusToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken Minus(this ExpressionToken first, string second)
        {
            return new MinusToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken Divide(this ExpressionToken first, string second)
        {
            return new DivideToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken BitwiseAnd(this ExpressionToken first, string second)
        {
            return new BitwiseAndToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken BitwiseOr(this ExpressionToken first, string second)
        {
            return new BitwiseOrToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken BitwiseXor(this ExpressionToken first, string second)
        {
            return new BitwiseXorToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken BitwiseNot(this ExpressionToken first, string second)
        {
            return new BitwiseNotToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken Modulo(this ExpressionToken first, string second)
        {
            return new ModuloToken { First = first, Second = Sql.Name(second) };
        }

        public static ExpressionToken Multiply(this ExpressionToken first, string second)
        {
            return new MultiplyToken { First = first, Second = Sql.Name(second) };
        }

        public static GroupToken Group(this Token token)
        {
            return new GroupToken { Token = token };
        }

        public static ExpressionToken Not(this ExpressionToken token)
        {
            return new NotToken { Token = token };
        }

        public static ExpressionToken IsNull(this ExpressionToken token)
        {
            return new IsNullToken { Token = token };
        }

        public static ExpressionToken IsNotNull(this ExpressionToken token)
        {
            return new IsNotNullToken { Token = token };
        }

        public static BetweenToken Between(this ExpressionToken token, ExpressionToken first, ExpressionToken second)
        {
            return new BetweenToken { Token = token, First = first, Second = second };
        }

        public static ExpressionToken In(this ExpressionToken token, params ExpressionToken[] tokens)
        {
            var value = new InToken { Token = token };
            value.Set.AddRange(tokens);
            return value;
        }

        public static ExpressionToken NotIn(this ExpressionToken token, params ExpressionToken[] tokens)
        {
            var value = new NotInToken { Token = token };
            value.Set.AddRange(tokens);
            return value;
        }

        public static ExpressionToken In(this ExpressionToken token, IEnumerable<ExpressionToken> tokens)
        {
            var value = new InToken { Token = token };
            if (tokens != null)
            {
                value.Set.AddRange(tokens);
            }
            return value;
        }

        public static ExpressionToken NotIn(this ExpressionToken token, IEnumerable<ExpressionToken> tokens)
        {
            var value = new NotInToken { Token = token };
            if (tokens != null)
            {
                value.Set.AddRange(tokens);
            }
            return value;
        }

        public static ExpressionToken Contains(this ExpressionToken first, ExpressionToken second)
        {
            return new ContainsToken { First = first, Second = second };
        }

        public static ExpressionToken StartsWith(this ExpressionToken first, ExpressionToken second)
        {
            return new StartsWithToken { First = first, Second = second };
        }

        public static ExpressionToken EndsWith(this ExpressionToken first, ExpressionToken second)
        {
            return new EndsWithToken { First = first, Second = second };
        }

        public static ExpressionToken Like(this ExpressionToken first, ExpressionToken second)
        {
            return new LikeToken { First = first, Second = second };
        }


        public static IfStatement Then(this IfStatement statement, params IStatement[] statements)
        {
            return Then(statement, (IEnumerable<IStatement>)statements);
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
            return Else(statement, (IEnumerable<IStatement>)statements);
        }

        public static IfStatement Else(this IfStatement statement, IEnumerable<IStatement> statements)
        {
            statement.Else = new StatementsStatement();
            if (statements != null)
                statement.Else.Statements.AddRange(statements);
            return statement;
        }

        public static CommentStatement CommentOut(this IStatement statement)
        {
            return new CommentStatement { Content = statement };
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

        public static InsertStatement Columns(this InsertStatement statement, IEnumerable<Name> columns)
        {
            if (columns != null)
            {
                statement.Columns.AddRange(columns);
            }
            return statement;
        }

        public static InsertStatement Columns(this InsertStatement statement, Name column, params Name[] columns)
        {
            return Columns(statement, ToNames(column, columns));
        }

        public static InsertStatement Columns(this InsertStatement statement, string column, params string[] columns)
        {
            return Columns(statement, ToNames(column, columns));
        }

        public static InsertStatement Columns(this InsertStatement statement, IEnumerable<string> columns)
        {
            return Columns(statement, ToNames(columns));
        }

        public static InsertStatement Values(this InsertStatement statement, IEnumerable<Token> values)
        {
            if (values != null)
            {
                statement.Values.Add(values.ToArray());
            }
            return statement;
        }
        public static InsertStatement Values(this InsertStatement statement, params Token[] values)
        {
            return Values(statement, (IEnumerable<Token>)values);
        }

        public static InsertStatement Values(this InsertStatement statement, params object[] values)
        {
            return Values(statement, (values ?? Enumerable.Empty<object>()).Select(Sql.Scalar));
        }

        public static InsertStatement Values(this InsertStatement statement, IEnumerable<object> values)
        {
            return Values(statement, (values ?? Enumerable.Empty<object>()).Select(Sql.Scalar));
        }
        public static InsertStatement OrReplace(this InsertStatement statement)
        {
            statement.Conflict = OnConflict.Replace;
            return statement;
        }
        public static InsertStatement OrRollback(this InsertStatement statement)
        {
            statement.Conflict = OnConflict.Rollback;
            return statement;
        }
        public static InsertStatement OrAbort(this InsertStatement statement)
        {
            statement.Conflict = OnConflict.Abort;
            return statement;
        }
        public static InsertStatement OrFail(this InsertStatement statement)
        {
            statement.Conflict = OnConflict.Fail;
            return statement;
        }
        public static InsertStatement OrIgnore(this InsertStatement statement)
        {
            statement.Conflict = OnConflict.Ignore;
            return statement;
        }

        public static UpdateStatement OrReplace(this UpdateStatement statement)
        {
            statement.Conflict = OnConflict.Replace;
            return statement;
        }
        public static UpdateStatement OrRollback(this UpdateStatement statement)
        {
            statement.Conflict = OnConflict.Rollback;
            return statement;
        }
        public static UpdateStatement OrAbort(this UpdateStatement statement)
        {
            statement.Conflict = OnConflict.Abort;
            return statement;
        }
        public static UpdateStatement OrFail(this UpdateStatement statement)
        {
            statement.Conflict = OnConflict.Fail;
            return statement;
        }
        public static UpdateStatement OrIgnore(this UpdateStatement statement)
        {
            statement.Conflict = OnConflict.Ignore;
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


        public static CreateIndexStatement OnColumn(this CreateIndexStatement statement, Name column,
            Direction direction = Direction.Asc)
        {
            statement.Columns.Add(Sql.Order(column, direction));
            return statement;
        }

        public static CreateIndexStatement OnColumn(this CreateIndexStatement statement, string column,
            Direction direction = Direction.Asc)
        {
            statement.Columns.Add(Sql.Order(column, direction));
            return statement;
        }

        public static CreateIndexStatement OnColumn(this CreateIndexStatement statement, Order column, params Order[] columns)
        {
            return OnColumn(statement, ToOrders(column, columns));
        }
        public static CreateIndexStatement OnColumn(this CreateIndexStatement statement, string column, params string[] columns)
        {
            return OnColumn(statement, ToOrders(column, columns));
        }

        public static CreateIndexStatement Include(this CreateIndexStatement statement, IEnumerable<Name> columns)
        {
            if (columns != null)
            {
                statement.Include.AddRange(columns);
            }
            return statement;
        }

        public static CreateIndexStatement Include(this CreateIndexStatement statement, string column, params string[] columns)
        {
            return Include(statement, ToNames(column, columns));
        }

        public static CreateIndexStatement Include(this CreateIndexStatement statement, Name column, params Name[] columns)
        {
            return Include(statement, ToNames(column, columns));
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
            column.Null = !notNull;
            return column;
        }
        public static TableColumn Null(this TableColumn column, OnConflict onConflict, bool notNull = false)
        {
            column.Null = !notNull;
            column.NullConflict = onConflict;
            return column;
        }

        public static TableColumn NotNull(this TableColumn column)
        {
            column.Null = false;
            return column;
        }
        public static TableColumn NotNull(this TableColumn column, OnConflict onConflict)
        {
            column.Null = false;
            column.NullConflict = onConflict;
            return column;
        }

        public static TableColumn Identity(this TableColumn column, int seed = 1, int increment = 1)
        {
            column.Identity.On = true;
            column.Identity.Seed = seed;
            column.Identity.Increment = increment;
            return column;
        }
        public static TableColumn AutoIncrement(this TableColumn column)
        {
            column.Identity.On = true;
            column.Identity.Seed = 1;
            column.Identity.Increment = 1;
            return column;
        }
        public static TableColumn PrimaryKey(this TableColumn column, Direction direction = Direction.Asc)
        {
            column.PrimaryKeyDirection = direction;
            return column;
        }
        public static TableColumn PrimaryKey(this TableColumn column, OnConflict onConflict, Direction direction = Direction.Asc)
        {
            column.PrimaryKeyDirection = direction;
            column.PrimaryKeyConflict = onConflict;
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
        public static TableColumn Default(this TableColumn column, Function value)
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

        public static TryCatchStatement Catch(this TryCatchStatement statement, IStatement catchStatement)
        {
            statement.CatchStatement = catchStatement;
            return statement;
        }

        public static WhileStatement Do(this WhileStatement statement, params IStatement[] statements)
        {
            statement.Do = new StatementsStatement();
            statement.Do.Statements.AddRange(statements);
            return statement;
        }

        public static WhileStatement Do(this WhileStatement statement, IEnumerable<IStatement> statements)
        {
            statement.Do = new StatementsStatement();
            if (statements != null)
            {
                statement.Do.Statements.AddRange(statements);
            }
            return statement;
        }


        #region PrimaryKey

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, Name name,
            params Order[] columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Name = name;
            statement.PrimaryKey.Columns.AddRange(columns);
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, Name name,
            IEnumerable<Order> columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Name = name;
            if (columns != null)
            {
                statement.PrimaryKey.Columns.AddRange(columns);
            }
            return statement;
        }



        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, Name name, bool clustered,
            params Order[] columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Clustered = clustered;
            statement.PrimaryKey.Name = name;
            statement.PrimaryKey.Columns.AddRange(columns);
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, Name name, bool clustered,
            IEnumerable<Order> columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Clustered = clustered;
            statement.PrimaryKey.Name = name;
            if (columns != null)
            {
                statement.PrimaryKey.Columns.AddRange(columns);
            }
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, params Order[] columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }
            statement.PrimaryKey.Name = Sql.Name("PK_" + statement.Name);
            statement.PrimaryKey.Columns.AddRange(columns);
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, IEnumerable<Order> columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Name = Sql.Name("PK_" + statement.Name);
            if (columns != null)
            {
                statement.PrimaryKey.Columns.AddRange(columns);
            }
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, bool clustered,
            params Order[] columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Clustered = clustered;
            statement.PrimaryKey.Name = Sql.Name("PK_" + statement.Name);
            statement.PrimaryKey.Columns.AddRange(columns);
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, bool clustered,
            IEnumerable<Order> columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Clustered = clustered;
            statement.PrimaryKey.Name = Sql.Name("PK_" + statement.Name);
            if (columns != null)
            {
                statement.PrimaryKey.Columns.AddRange(columns);
            }
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, Name name, OnConflict onConflict,
            params Order[] columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Name = name;
            statement.PrimaryKey.Conflict = onConflict;
            statement.PrimaryKey.Columns.AddRange(columns);
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, Name name, OnConflict onConflict,
            IEnumerable<Order> columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Name = name;
            statement.PrimaryKey.Conflict = onConflict;
            if (columns != null)
            {
                statement.PrimaryKey.Columns.AddRange(columns);
            }
            return statement;
        }



        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, Name name, bool clustered, OnConflict onConflict,
            params Order[] columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Clustered = clustered;
            statement.PrimaryKey.Conflict = onConflict;
            statement.PrimaryKey.Name = name;
            statement.PrimaryKey.Columns.AddRange(columns);
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, Name name, bool clustered,
             OnConflict onConflict, IEnumerable<Order> columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Clustered = clustered;
            statement.PrimaryKey.Conflict = onConflict;
            statement.PrimaryKey.Name = name;
            if (columns != null)
            {
                statement.PrimaryKey.Columns.AddRange(columns);
            }
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, OnConflict onConflict, params Order[] columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }
            statement.PrimaryKey.Name = Sql.Name("PK_" + statement.Name);
            statement.PrimaryKey.Conflict = onConflict;
            statement.PrimaryKey.Columns.AddRange(columns);
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, OnConflict onConflict, IEnumerable<Order> columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Name = Sql.Name("PK_" + statement.Name);
            statement.PrimaryKey.Conflict = onConflict;
            if (columns != null)
            {
                statement.PrimaryKey.Columns.AddRange(columns);
            }
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, bool clustered,
           OnConflict onConflict, params Order[] columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Clustered = clustered;
            statement.PrimaryKey.Conflict = onConflict;
            statement.PrimaryKey.Name = Sql.Name("PK_" + statement.Name);
            statement.PrimaryKey.Columns.AddRange(columns);
            return statement;
        }

        public static CreateTableStatement PrimaryKey(this CreateTableStatement statement, bool clustered, OnConflict onConflict,
            IEnumerable<Order> columns)
        {
            if (statement.PrimaryKey == null)
            {
                statement.PrimaryKey = new ConstrainDefinition();
            }

            statement.PrimaryKey.Clustered = clustered;
            statement.PrimaryKey.Conflict = onConflict;
            statement.PrimaryKey.Name = Sql.Name("PK_" + statement.Name);
            if (columns != null)
            {
                statement.PrimaryKey.Columns.AddRange(columns);
            }
            return statement;
        }
        #endregion PrimaryKey

        #region UniqueConstrainOn

        public static CreateTableStatement UniqueConstrainOn(this CreateTableStatement statement, string name,
            params Order[] columns)
        {
            var index = new ConstrainDefinition { Clustered = false, Name = Sql.Name(name) };
            index.Columns.AddRange(columns);
            statement.UniqueConstrains.Add(index);
            return statement;
        }

        public static CreateTableStatement UniqueConstrainOn(this CreateTableStatement statement, string name,
            IEnumerable<Order> columns)
        {
            var index = new ConstrainDefinition { Clustered = false, Name = Sql.Name(name) };

            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            statement.UniqueConstrains.Add(index);
            return statement;
        }

        public static CreateTableStatement UniqueConstrainOn(this CreateTableStatement statement, Name name,
            params Order[] columns)
        {
            var index = new ConstrainDefinition { Clustered = false, Name = name };

            index.Columns.AddRange(columns);
            statement.UniqueConstrains.Add(index);
            return statement;
        }

        public static CreateTableStatement UniqueConstrainOn(this CreateTableStatement statement, Name name,
            IEnumerable<Order> columns)
        {
            var index = new ConstrainDefinition { Clustered = false, Name = name };

            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            statement.UniqueConstrains.Add(index);
            return statement;
        }

        public static CreateTableStatement UniqueConstrainOn(this CreateTableStatement statement, string name,
            bool clustered, params Order[] columns)
        {
            var index = new ConstrainDefinition { Clustered = clustered, Name = Sql.Name(name) };

            index.Columns.AddRange(columns);
            statement.UniqueConstrains.Add(index);
            return statement;
        }

        public static CreateTableStatement UniqueConstrainOn(this CreateTableStatement statement, string name,
            bool clustered, IEnumerable<Order> columns)
        {
            var index = new ConstrainDefinition { Clustered = clustered, Name = Sql.Name(name) };

            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            statement.UniqueConstrains.Add(index);
            return statement;
        }

        public static CreateTableStatement UniqueConstrainOn(this CreateTableStatement statement, Name name,
            bool clustered, params Order[] columns)
        {
            var index = new ConstrainDefinition { Clustered = clustered, Name = name };

            index.Columns.AddRange(columns);
            statement.UniqueConstrains.Add(index);
            return statement;
        }

        public static CreateTableStatement UniqueConstrainOn(this CreateTableStatement statement, Name name,
            bool clustered, IEnumerable<Order> columns)
        {
            var index = new ConstrainDefinition { Clustered = clustered, Name = name };

            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            statement.UniqueConstrains.Add(index);
            return statement;
        }

        public static CreateTableStatement UniqueConstrainOn(this CreateTableStatement statement, params Order[] columns)
        {
            var index = new ConstrainDefinition { Clustered = false, Name = Sql.Name("UC_" + statement.Name) };
            index.Columns.AddRange(columns);
            statement.UniqueConstrains.Add(index);
            return statement;
        }

        public static CreateTableStatement UniqueConstrainOn(this CreateTableStatement statement,
            IEnumerable<Order> columns)
        {
            var index = new ConstrainDefinition { Clustered = false, Name = Sql.Name("UC_" + statement.Name) };

            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            statement.UniqueConstrains.Add(index);
            return statement;
        }

        public static CreateTableStatement UniqueConstrainOn(this CreateTableStatement statement, bool clustered,
            params Order[] columns)
        {
            var index = new ConstrainDefinition { Clustered = clustered, Name = Sql.Name("UC_" + statement.Name) };

            index.Columns.AddRange(columns);
            statement.UniqueConstrains.Add(index);
            return statement;
        }

        public static CreateTableStatement UniqueConstrainOn(this CreateTableStatement statement, bool clustered,
            IEnumerable<Order> columns)
        {
            var index = new ConstrainDefinition { Clustered = clustered, Name = Sql.Name("UC_" + statement.Name) };

            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            statement.UniqueConstrains.Add(index);
            return statement;
        }

        #endregion UniqueConstrainOn

        #region IndexOn

        public static CreateTableStatement IndexOn(this CreateTableStatement statement, string name,
            params Order[] columns)
        {
            var index = new CreateIndexStatement
            {
                Clustered = false,
                Name = Sql.Name(name),
                On = statement.Name,
                Unique = false
            };
            index.Columns.AddRange(columns);
            statement.Indicies.Add(index);
            return statement;
        }

        public static CreateTableStatement IndexOn(this CreateTableStatement statement, string name,
            IEnumerable<Order> columns)
        {
            var index = new CreateIndexStatement
            {
                Clustered = false,
                Name = Sql.Name(name),
                On = statement.Name,
                Unique = false
            };
            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            statement.Indicies.Add(index);
            return statement;
        }

        public static CreateTableStatement IndexOn(this CreateTableStatement statement, Name name,
            params Order[] columns)
        {
            var index = new CreateIndexStatement { Clustered = false, Name = name, On = statement.Name, Unique = false };
            index.Columns.AddRange(columns);
            statement.Indicies.Add(index);
            return statement;
        }

        public static CreateTableStatement IndexOn(this CreateTableStatement statement, Name name,
            IEnumerable<Order> columns)
        {
            var index = new CreateIndexStatement { Clustered = false, Name = name, On = statement.Name, Unique = false };
            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            statement.Indicies.Add(index);
            return statement;
        }

        public static CreateTableStatement IndexOn(this CreateTableStatement statement, string name,
            IEnumerable<Order> columns, IEnumerable<string> includeColumns)
        {
            var index = new CreateIndexStatement
            {
                Clustered = false,
                Name = Sql.Name(name),
                On = statement.Name,
                Unique = false
            };
            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            if (includeColumns != null)
            {
                index.Include.AddRange(includeColumns.Select(ic => Sql.Name(ic)));
            }
            statement.Indicies.Add(index);
            return statement;
        }

        public static CreateTableStatement IndexOn(this CreateTableStatement statement, Name name,
            IEnumerable<Order> columns, IEnumerable<string> includeColumns)
        {
            var index = new CreateIndexStatement { Clustered = false, Name = name, On = statement.Name, Unique = false };
            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            if (includeColumns != null)
            {
                index.Include.AddRange(includeColumns.Select(ic => Sql.Name(ic)));
            }
            statement.Indicies.Add(index);
            return statement;
        }

        public static CreateTableStatement IndexOn(this CreateTableStatement statement, string name,
            IEnumerable<Order> columns, IEnumerable<Name> includeColumns)
        {
            var index = new CreateIndexStatement
            {
                Clustered = false,
                Name = Sql.Name(name),
                On = statement.Name,
                Unique = false
            };
            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            if (includeColumns != null)
            {
                index.Include.AddRange(includeColumns);
            }
            statement.Indicies.Add(index);
            return statement;
        }

        public static CreateTableStatement IndexOn(this CreateTableStatement statement, Name name,
            IEnumerable<Order> columns, IEnumerable<Name> includeColumns)
        {
            var index = new CreateIndexStatement { Clustered = false, Name = name, On = statement.Name, Unique = false };
            if (columns != null)
            {
                index.Columns.AddRange(columns);
            }
            if (includeColumns != null)
            {
                index.Include.AddRange(includeColumns);
            }
            statement.Indicies.Add(index);
            return statement;
        }

        #endregion IndexOn

        #region Top

        public static SelectStatement Top(this SelectStatement statement, int value, bool percent, bool withTies = false)
        {
            statement.Top = new Top(value, percent, withTies);

            return statement;
        }
        public static SelectStatement Top(this SelectStatement statement, Parameter value, bool percent, bool withTies = false)
        {
            statement.Top = new Top(value, percent, withTies);

            return statement;
        }

        public static T Top<T>(this T statement, int value, bool percent = false)
            where T : ITopStatement
        {
            statement.Top = new Top(value, percent, false);

            return statement;
        }
        public static T Top<T>(this T statement, Parameter value, bool percent = false)
            where T : ITopStatement
        {
            statement.Top = new Top(value, percent, false);

            return statement;
        }

        public static T Limit<T>(this T statement, int value)
            where T : ITopStatement
        {
            statement.Top = new Top(value, false, false);

            return statement;
        }

        public static T Offset<T>(this T statement, int value)
            where T : IOffsetStatement
        {
            statement.Offset = new Scalar { Value = value };
            return statement;
        }
        public static T FetchNext<T>(this T statement, int value)
            where T : ITopStatement
        {
            statement.Top = new Top(value, false, false);

            return statement;
        }
        #endregion Top

        #region CTE

        public static CTEDefinition As(this CTEDeclaration cte, SelectStatement definition)
        {
            return new CTEDefinition
            {
                Declaration = cte,
                Definition = definition
            };
        }
        public static CTEDeclaration Recursive(this CTEDeclaration cte, bool recursive = true)
        {
            cte.Recursive = recursive;
            return cte;
        }

        public static CTEDeclaration With(this CTEDefinition previousCommonTableExpression, string name, params string[] columnNames)
        {
            var cte = new CTEDeclaration
            {
                Name = name,
                PreviousCommonTableExpression = previousCommonTableExpression
            };
            if (columnNames != null)
            {
                cte.Columns.AddRange(columnNames.Select(n => Sql.Name(n)));
            }
            return cte;
        }
        public static CTEDeclaration With(this CTEDefinition previousCommonTableExpression, string name, IEnumerable<string> columnNames)
        {
            var cte = new CTEDeclaration
            {
                Name = name,
                PreviousCommonTableExpression = previousCommonTableExpression
            };
            if (columnNames != null)
            {
                cte.Columns.AddRange(columnNames.Select(n => Sql.Name(n)));
            }
            return cte;
        }

        static IEnumerable<CTEDefinition> GetCteDefinitions(CTEDefinition definition)
        {
            if (definition != null)
            {
                if (definition.Declaration != null && definition.Declaration.PreviousCommonTableExpression != null)
                {
                    foreach (var prevDefinition in GetCteDefinitions(definition.Declaration.PreviousCommonTableExpression))
                    {
                        yield return prevDefinition;
                    }
                }
                yield return definition;
            }
        }

        public static SelectStatement Select(this CTEDefinition cte)
        {
            var statement = new SelectStatement();
            statement.CommonTableExpressions.AddRange(GetCteDefinitions(cte));
            return statement;
        }

        public static DeleteStatement Delete(this CTEDefinition cte)
        {
            var statement = new DeleteStatement();
            statement.CommonTableExpressions.AddRange(GetCteDefinitions(cte));
            return statement;
        }

        public static InsertStatement Insert(this CTEDefinition cte)
        {
            var statement = new InsertStatement();
            statement.CommonTableExpressions.AddRange(GetCteDefinitions(cte));
            return statement;
        }

        public static MergeStatement Merge(this CTEDefinition cte)
        {
            var statement = new MergeStatement();
            statement.CommonTableExpressions.AddRange(GetCteDefinitions(cte));
            return statement;
        }
        public static UpdateStatement Update(this CTEDefinition cte, string target)
        {
            var statement = new UpdateStatement
            {
                Target = Sql.Name(target)
            };
            statement.CommonTableExpressions.AddRange(GetCteDefinitions(cte));
            return statement;
        }

        public static UpdateStatement Update(this CTEDefinition cte, Name target)
        {
            var statement = new UpdateStatement
            {
                Target = target
            };
            statement.CommonTableExpressions.AddRange(GetCteDefinitions(cte));
            return statement;
        }
        #endregion CTE


        #region Snippet

        public static Snippet Dialect(this Snippet snippet, string value, string dialectName, params string[] additionalDialects)
        {
            return snippet;
        }
        public static Snippet Dialect(this Snippet snippet, string value, IEnumerable<string> dialects)
        {
            return snippet;
        }

        public static SnippetStatement Dialect(this SnippetStatement snippetStatement, string value, string dialectName, params string[] additionalDialects)
        {
            return snippetStatement;
        }
        public static SnippetStatement Dialect(this SnippetStatement snippetStatement, string value, IEnumerable<string> dialects)
        {



            return snippetStatement;
        }

        #endregion Snippet
    }
}