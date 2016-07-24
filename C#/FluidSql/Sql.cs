// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TTRider.FluidSql
{
    public partial class Sql
    {
        public static CaseToken Case => new CaseToken();

        public static Name Star([CanBeNull] string source = null)
        {
            return !String.IsNullOrWhiteSpace(source) ? new Name(source, "*") : new Name("*");
        }

        public static Name Default([CanBeNull] string source = null)
        {
            const string defaultToken = "DEFAULT";
            var token =
                !String.IsNullOrWhiteSpace(source) ? new Name(source, defaultToken)
                : new Name(defaultToken);
            return token.NoQuotes(defaultToken);
        }

        public static Name Name(params string[] names)
        {
            return new Name(names);
        }

        public static Name NameAs([NotNull] string name, [NotNull] string alias)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(alias));
            return new Name(name) { Alias = alias };
        }

        public static Name NameAs([NotNull] string part1, [NotNull] string part2, [NotNull] string alias)
        {
            if (string.IsNullOrWhiteSpace(part1))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(part1));
            if (string.IsNullOrWhiteSpace(part2))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(part2));
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(alias));
            return new Name(part1, part2) { Alias = alias };
        }

        public static Name NameAs([NotNull] string part1, [NotNull] string part2, [NotNull] string part3,
            [NotNull] string alias)
        {
            if (string.IsNullOrWhiteSpace(part1))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(part1));
            if (string.IsNullOrWhiteSpace(part2))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(part2));
            if (string.IsNullOrWhiteSpace(part3))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(part3));
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(alias));
            return new Name(part1, part2, part3) { Alias = alias };
        }

        public static Name NameAs([NotNull] string part1, [NotNull] string part2, [NotNull] string part3,
            [NotNull] string part4, [NotNull] string alias)
        {
            if (string.IsNullOrWhiteSpace(part1))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(part1));
            if (string.IsNullOrWhiteSpace(part2))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(part2));
            if (string.IsNullOrWhiteSpace(part3))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(part3));
            if (string.IsNullOrWhiteSpace(part4))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(part4));
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(alias));
            return new Name(part1, part2, part3, part4) { Alias = alias };
        }

        public static Snippet Snippet([NotNull] string value, params Parameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            var val = new Snippet { Value = value };

            foreach (var p in parameters)
            {
                val.Parameters.Add(p);
            }
            return val;
        }

        public static Snippet Template([NotNull] string value, [NotNull] Token argument, params Token[] arguments)
        {
            if (argument == null) throw new ArgumentNullException(nameof(argument));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            return Template(value, Enumerable.Repeat(argument, 1).Union(arguments));
        }

        public static Snippet Snippet([NotNull] string value, IEnumerable<Parameter> parameters)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
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

        public static Snippet Template([NotNull] string value, IEnumerable<Token> arguments)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            var val = new Snippet { Value = value };
            if (arguments != null)
            {
                foreach (var argument in arguments)
                {
                    val.Arguments.Add(argument);
                    if (argument is Parameter)
                    {
                        val.Parameters.Add((Parameter)argument);
                    }
                }
            }
            return val;
        }

        public static SnippetStatement SnippetStatement([CanBeNull] string value,
            [CanBeNull] IEnumerable<Parameter> parameters)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var snippetStatement = new SnippetStatement
            {
                Value = value
            };
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                    snippetStatement.Parameters.Add(parameter);
            }
            return snippetStatement;
        }

        public static SnippetStatement TemplateStatement([CanBeNull] string value,
            [CanBeNull] IEnumerable<Token> arguments)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            var snippetStatement = new SnippetStatement
            {
                Value = value
            };
            if (arguments != null)
            {
                foreach (var argument in arguments)
                {
                    snippetStatement.Arguments.Add(argument);
                    if (argument is Parameter)
                    {
                        snippetStatement.Parameters.Add((Parameter)argument);
                    }
                }
            }

            return snippetStatement;
        }

        public static SnippetStatement SnippetStatement([NotNull] string value,
            [CanBeNull] params Parameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            var snippetStatement = new SnippetStatement
            {
                Value = value
            };
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    snippetStatement.Parameters.Add(parameter);
                }
            }
            return snippetStatement;
        }

        public static SnippetStatement TemplateStatement([NotNull] string value, [NotNull] Token argument,
            [CanBeNull] params Token[] arguments)
        {
            if (argument == null) throw new ArgumentNullException(nameof(argument));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

            var args = Enumerable.Repeat(argument, 1);
            if (arguments != null)
            {
                args = args.Union(arguments);
            }
            return TemplateStatement(value, args);
        }

        public static Scalar Scalar(object value)
        {
            if (value is Scalar)
            {
                return (Scalar) value;
            }
            return new Scalar { Value = value };
        }

        public static UnaryMinusToken Minus([NotNull] Token value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new UnaryMinusToken { Token = value };
        }

        public static GroupToken Group([NotNull] Token value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new GroupToken { Token = value };
        }

        public static ExpressionToken Exists([NotNull] ExpressionToken value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new ExistsToken { Token = value };
        }

        public static ExpressionToken NotExists([NotNull] ExpressionToken value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new ExistsToken { Token = value }.Not();
        }

        public static ExpressionToken Not([NotNull] ExpressionToken value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return value.Not();
        }

        public static ExpressionToken All([NotNull] SelectStatement subQuery)
        {
            if (subQuery == null) throw new ArgumentNullException(nameof(subQuery));
            return new AllToken { Token = subQuery };
        }

        public static ExpressionToken Any([NotNull] SelectStatement subQuery)
        {
            if (subQuery == null) throw new ArgumentNullException(nameof(subQuery));
            return new AnyToken { Token = subQuery };
        }

        public static ExpressionToken Some([NotNull] SelectStatement subQuery)
        {
            if (subQuery == null) throw new ArgumentNullException(nameof(subQuery));
            return new AnyToken { Token = subQuery };
        }


        public static Order Order([NotNull] Name column, Direction direction = Direction.Asc)
        {
            if (column == null) throw new ArgumentNullException(nameof(column));
            return new Order { Column = column, Direction = direction };
        }

        public static Order Order([NotNull] string column, Direction direction = Direction.Asc)
        {
            if (string.IsNullOrWhiteSpace(column))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(column));
            return new Order { Column = new Name(column), Direction = direction };
        }

        public static CreateIndexStatement CreateIndex([NotNull] Name name, [CanBeNull] Name on = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new CreateIndexStatement
            {
                Name = name,
                On = on
            };
        }

        public static AlterIndexStatement AlterIndex([NotNull] Name name, [CanBeNull] Name on = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new AlterIndexStatement
            {
                Name = name,
                On = on
            };
        }

        public static DropIndexStatement DropIndex([NotNull] Name name, [CanBeNull] Name on = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new DropIndexStatement
            {
                Name = name,
                On = on
            };
        }

        public static DropIndexStatement DropIndex([NotNull] Name name, [NotNull] Name on, bool checkExists)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (@on == null) throw new ArgumentNullException(nameof(@on));
            return new DropIndexStatement
            {
                Name = name,
                On = on,
                CheckExists = checkExists
            };
        }

        public static DropIndexStatement DropIndex([NotNull] Name name, bool checkExists)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new DropIndexStatement
            {
                Name = name,
                CheckExists = checkExists
            };
        }

        public static CreateIndexStatement CreateIndex([NotNull] string name, string on)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            return new CreateIndexStatement
            {
                Name = Name(name),
                On = Name(on)
            };
        }

        public static AlterIndexStatement AlterIndexAll([NotNull] Name on)
        {
            if (@on == null) throw new ArgumentNullException(nameof(@on));
            return new AlterIndexStatement
            {
                Name = null,
                On = on
            };
        }

        public static AlterIndexStatement Reindex([NotNull] Name on)
        {
            if (@on == null) throw new ArgumentNullException(nameof(@on));
            return new AlterIndexStatement
            {
                Name = null,
                On = on
            }.Rebuild();
        }

        public static AlterIndexStatement Reindex([NotNull] Name name, [NotNull] Name on)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (@on == null) throw new ArgumentNullException(nameof(@on));
            return new AlterIndexStatement
            {
                Name = name,
                On = on
            }.Rebuild();
        }


        public static CTEDeclaration With([NotNull] string name, [CanBeNull] params string[] columnNames)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            var cte = new CTEDeclaration
            {
                Name = name
            };
            if (columnNames != null)
            {
                cte.Columns.AddRange(columnNames.Select(n => Name(n)));
            }
            return cte;
        }

        public static CTEDeclaration With([NotNull] string name, [CanBeNull] IEnumerable<string> columnNames)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            var cte = new CTEDeclaration
            {
                Name = name
            };
            if (columnNames != null)
            {
                cte.Columns.AddRange(columnNames.Select(n => Name(n)));
            }
            return cte;
        }

        #region Set

        public static SetStatement Assign([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new AssignToken { First = target, Second = expression }
            };
        }

        public static SetStatement PlusAssign([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new PlusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MinusAssign([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new MinusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement DivideAssign([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new DivideToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseAndAssign([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new BitwiseAndToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseOrAssign([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new BitwiseOrToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseXorAssign([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new BitwiseXorToken { First = target, Second = expression, Equal = true }
            };
        }

        //public static SetStatement BitwiseNotAssign(Parameter target, ExpressionToken expression)
        //{
        //    return new SetStatement
        //    {
        //        Assign = new BitwiseNotToken {First = target, Second = expression, Equal = true}
        //    };
        //}

        public static SetStatement ModuloAssign([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new ModuloToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MultiplyAssign([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new MultiplyToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement Set([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new AssignToken { First = target, Second = expression }
            };
        }

        public static SetStatement PlusSet([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new PlusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MinusSet([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new MinusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement DivideSet([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new DivideToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseAndSet([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new BitwiseAndToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseOrSet([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new BitwiseOrToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseXorSet([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new BitwiseXorToken { First = target, Second = expression, Equal = true }
            };
        }

        //public static SetStatement BitwiseNotSet(Parameter target, ExpressionToken expression)
        //{
        //    return new SetStatement
        //    {
        //        Assign = new BitwiseNotToken {First = target, Second = expression, Equal = true}
        //    };
        //}

        public static SetStatement ModuloSet([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new ModuloToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MultiplySet([NotNull] Parameter target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new MultiplyToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement Assign([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new AssignToken { First = target, Second = expression }
            };
        }

        public static SetStatement PlusAssign([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new PlusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MinusAssign([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new MinusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement DivideAssign([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new DivideToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseAndAssign([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new BitwiseAndToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseOrAssign([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new BitwiseOrToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseXorAssign([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new BitwiseXorToken { First = target, Second = expression, Equal = true }
            };
        }

        //public static SetStatement BitwiseNotAssign(Name target, ExpressionToken expression)
        //{
        //    return new SetStatement
        //    {
        //        Assign = new BitwiseNotToken { First = target, Second = expression, Equal = true }
        //    };
        //}

        public static SetStatement ModuloAssign([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new ModuloToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MultiplyAssign([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new MultiplyToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement Set([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new AssignToken { First = target, Second = expression }
            };
        }

        public static SetStatement PlusSet([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new PlusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MinusSet([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new MinusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement DivideSet([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new DivideToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseAndSet([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new BitwiseAndToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseOrSet([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new BitwiseOrToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseXorSet([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new BitwiseXorToken { First = target, Second = expression, Equal = true }
            };
        }

        //public static SetStatement BitwiseNotSet(Name target, ExpressionToken expression)
        //{
        //    return new SetStatement
        //    {
        //        Assign = new BitwiseNotToken { First = target, Second = expression, Equal = true }
        //    };
        //}

        public static SetStatement ModuloSet([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new ModuloToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MultiplySet([NotNull] Name target, [NotNull] ExpressionToken expression)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            return new SetStatement
            {
                Assign = new MultiplyToken { First = target, Second = expression, Equal = true }
            };
        }

        #endregion Set

        #region Statements
        public static PrepareStatement Prepare()
        {
            return new PrepareStatement();
        }

        public static PrepareStatement Prepare([NotNull] Name name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new PrepareStatement
            {
                Name = name
            };
        }

        public static PrepareStatement Prepare([NotNull] Name name, [NotNull] Name targetName)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (targetName == null) throw new ArgumentNullException(nameof(targetName));
            var statement = new PrepareStatement
            {
                Name = name,
                Target = {Name = targetName}
            };
            return statement;
        }

        public static PrepareStatement Prepare([NotNull] Name name, [NotNull] IStatement targetStatement,
            [CanBeNull] IEnumerable<Parameter> parameters)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (targetStatement == null) throw new ArgumentNullException(nameof(targetStatement));
            var statement = new PrepareStatement
            {
                Name = name,
                Target = {Target = targetStatement}
            };
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    statement.Parameters.Add(parameter);
                }
            }
            return statement;
        }

        public static PrepareStatement Prepare([NotNull] Name name, [NotNull] IStatement targetStatement,
            [CanBeNull] params Parameter[] parameters)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (targetStatement == null) throw new ArgumentNullException(nameof(targetStatement));
            return Prepare(name, targetStatement, (IEnumerable<Parameter>)parameters);
        }

        public static ExecuteStatement Execute([NotNull] string statement, [CanBeNull] IEnumerable<Parameter> parameters)
        {
            if (string.IsNullOrWhiteSpace(statement))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(statement));
            return Execute(SnippetStatement(statement), parameters);
        }

        public static ExecuteStatement Execute([NotNull] string statement, [CanBeNull] params Parameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(statement))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(statement));
            return Execute(SnippetStatement(statement), (IEnumerable<Parameter>)parameters);
        }

        public static ExecuteStatement Execute([NotNull] IStatement statement, [CanBeNull] params Parameter[] parameters)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return Execute(statement, (IEnumerable<Parameter>)parameters);
        }

        public static ExecuteStatement Execute([NotNull] IStatement statement,
            [CanBeNull] IEnumerable<Parameter> parameters)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));

            ExecuteStatement stat = new ExecuteStatement {Target = {Target = statement}};

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    stat.Parameters.Add(parameter);
                }
            }
            return stat;
        }

        public static ExecuteStatement Execute([NotNull] Name statName, [CanBeNull] params Parameter[] parameters)
        {
            if (statName == null) throw new ArgumentNullException(nameof(statName));

            ExecuteStatement stat = new ExecuteStatement {Name = statName};

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    stat.Parameters.Add(parameter);
                }
            }
            return stat;
        }

        public static DeallocateStatement Deallocate()
        {
            return new DeallocateStatement();
        }

        public static DeallocateStatement Deallocate([NotNull] Name name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new DeallocateStatement
            {
                Name = name
            };
        }

        public static PerformStatement Perform([NotNull] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(query));
            return new PerformStatement
            {
                Query = query
            };
        }

        public static PerformStatement Perform([NotNull] string statement, [CanBeNull] IEnumerable<Parameter> parameters)
        {
            if (string.IsNullOrWhiteSpace(statement))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(statement));
            return Perform(SnippetStatement(statement), parameters);
        }

        public static PerformStatement Perform([NotNull] string statement, [CanBeNull] params Parameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(statement))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(statement));
            return Perform(SnippetStatement(statement), (IEnumerable<Parameter>)parameters);
        }

        public static PerformStatement Perform([NotNull] IStatement statement, [CanBeNull] params Parameter[] parameters)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return Perform(statement, (IEnumerable<Parameter>)parameters);
        }

        public static PerformStatement Perform([NotNull] IStatement statement,
            [CanBeNull] IEnumerable<Parameter> parameters)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            PerformStatement stat = new PerformStatement {Target = {Target = statement}};

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    stat.Parameters.Add(parameter);
                }
            }
            return stat;
        }

        public static ExecuteProcedureStatement ExecuteStoredProcedure([NotNull] Name storedProcedureName,
            [CanBeNull] params Parameter[] parameters)
        {
            if (storedProcedureName == null) throw new ArgumentNullException(nameof(storedProcedureName));
            return ExecuteStoredProcedure(storedProcedureName, (IEnumerable<Parameter>)parameters);
        }

        public static ExecuteProcedureStatement ExecuteStoredProcedure([NotNull] Name storedProcedureName,
            [CanBeNull] IEnumerable<Parameter> parameters)
        {
            if (storedProcedureName == null) throw new ArgumentNullException(nameof(storedProcedureName));
            var stat = new ExecuteProcedureStatement
            {
                Name = storedProcedureName
            };

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    stat.Parameters.Add(parameter);
                }
            }
            return stat;
        }

        public static DropProcedureStatement DropProcedure([NotNull] Name name, bool checkExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new DropProcedureStatement
            {
                Name = name,
                CheckExists = checkExists
            };
        }

        public static CreateProcedureStatement CreateProcedure([NotNull] Name name, bool checkIfNotExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new CreateProcedureStatement
            {
                Name = name,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static AlterProcedureStatement AlterProcedure([NotNull] Name name, bool createIfNotExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new AlterProcedureStatement
            {
                Name = name,
                CreateIfNotExists = createIfNotExists
            };
        }

        public static CreateFunctionStatement CreateFunction([NotNull] Name name, bool checkIfNotExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new CreateFunctionStatement
            {
                Name = name,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static AlterFunctionStatement AlterFunction([NotNull] Name name, bool checkIfNotExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new AlterFunctionStatement
            {
                Name = name,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static DropFunctionStatement DropFunction([NotNull] Name name, bool checkExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new DropFunctionStatement
            {
                Name = name,
                CheckExists = checkExists
            };
        }

        public static ExecuteFunctionStatement ExecuteFunction([NotNull] Name storedProcedureName,
            [CanBeNull] params Parameter[] parameters)
        {
            if (storedProcedureName == null) throw new ArgumentNullException(nameof(storedProcedureName));
            return ExecuteFunction(storedProcedureName, (IEnumerable<Parameter>)parameters);
        }

        public static ExecuteFunctionStatement ExecuteFunction([NotNull] Name storedProcedureName,
            [CanBeNull] IEnumerable<Parameter> parameters)
        {
            if (storedProcedureName == null) throw new ArgumentNullException(nameof(storedProcedureName));
            var stat = new ExecuteFunctionStatement
            {
                Name = storedProcedureName
            };

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    stat.Parameters.Add(parameter);
                }
            }
            return stat;
        }

        public static SelectStatement Select => new SelectStatement();

        public static DeleteStatement Delete => new DeleteStatement();

        public static InsertStatement Insert => new InsertStatement();

        public static MergeStatement Merge => new MergeStatement();

        #region Transaction

        public static BeginTransactionStatement BeginTransaction([CanBeNull] Name name = null,
            [CanBeNull] string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description
            };
        }

        public static BeginTransactionStatement BeginSerializableTransaction([CanBeNull] Name name = null,
            [CanBeNull] string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description,
                IsolationLevel = IsolationLevelType.Serializable
            };
        }

        public static BeginTransactionStatement BeginRepeatebleReadTransaction([CanBeNull] Name name = null,
            [CanBeNull] string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description,
                IsolationLevel = IsolationLevelType.RepeatableRead
            };
        }

        public static BeginTransactionStatement BeginReadCommitedTransaction([CanBeNull] Name name = null,
            [CanBeNull] string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description,
                IsolationLevel = IsolationLevelType.ReadCommited
            };
        }

        public static BeginTransactionStatement BeginReadUnCommitedTransaction([CanBeNull] Name name = null,
            [CanBeNull] string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description,
                IsolationLevel = IsolationLevelType.ReadUnCommited
            };
        }

        public static BeginTransactionStatement BeginReadOnlyTransaction([CanBeNull] Name name = null,
            [CanBeNull] string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description,
                AccessType = TransactionAccessType.ReadOnly
            };
        }

        public static BeginTransactionStatement BeginReadWriteTransaction([CanBeNull] Name name = null,
            [CanBeNull] string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description,
                AccessType = TransactionAccessType.ReadWrite
            };
        }

        public static CommitTransactionStatement CommitTransaction([CanBeNull] Name name = null)
        {
            return new CommitTransactionStatement
            {
                Name = name
            };
        }

        public static CommitTransactionStatement ReleaseToSavepoint([CanBeNull] Name name)
        {
            return new CommitTransactionStatement
            {
                Name = name
            };
        }

        public static RollbackTransactionStatement RollbackTransaction([CanBeNull] Name name = null)
        {
            return new RollbackTransactionStatement
            {
                Name = name
            };
        }

        public static RollbackTransactionStatement RollbackToSavepoint([CanBeNull] Name name)
        {
            return new RollbackTransactionStatement
            {
                Name = name
            };
        }

        public static SaveTransactionStatement SaveTransaction([CanBeNull] Name name = null)
        {
            return new SaveTransactionStatement
            {
                Name = name
            };
        }

        public static SaveTransactionStatement SaveTransaction([NotNull] Parameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            return new SaveTransactionStatement
            {
                Parameter = parameter
            };
        }

        public static SaveTransactionStatement Savepoint([CanBeNull] Name name = null)
        {
            return new SaveTransactionStatement
            {
                Name = name
            };
        }


        public static BeginTransactionStatement BeginTransaction([NotNull] Parameter parameter,
            [CanBeNull] string description = null)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            return new BeginTransactionStatement
            {
                Parameter = parameter,
                Description = description
            };
        }

        public static CommitTransactionStatement CommitTransaction([NotNull] Parameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            return new CommitTransactionStatement
            {
                Parameter = parameter
            };
        }

        public static RollbackTransactionStatement RollbackTransaction([NotNull] Parameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            return new RollbackTransactionStatement
            {
                Parameter = parameter
            };
        }

        #endregion Transaction

        public static StatementsStatement Statements([NotNull] params IStatement[] statements)
        {
            if (statements == null) throw new ArgumentNullException(nameof(statements));
            var statement = new StatementsStatement();
            statement.Statements.AddRange(statements);
            return statement;
        }

        public static StatementsStatement Statements([NotNull] IStatement statement,
            [NotNull] params IStatement[] statements)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            if (statements == null) throw new ArgumentNullException(nameof(statements));
            var newstatement = new StatementsStatement();
            newstatement.Statements.Add(statement);
            newstatement.Statements.AddRange(statements);
            return newstatement;
        }

        public static StatementsStatement Statements([CanBeNull] IEnumerable<IStatement> statements)
        {
            var statement = new StatementsStatement();
            if (statements != null)
            {
                statement.Statements.AddRange(statements);
            }
            return statement;
        }

        public static DeclareStatement Declare([NotNull] Parameter variable, [CanBeNull] Token initializer = null)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return new DeclareStatement
            {
                Variable = variable,
                Initializer = initializer
            };
        }

        public static IfStatement If([NotNull] Token condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            return new IfStatement
            {
                Condition = condition
            };
        }

        public static UpdateStatement Update([NotNull] string target)
        {
            if (string.IsNullOrWhiteSpace(target))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(target));
            return new UpdateStatement
            {
                Target = Name(target)
            };
        }

        public static UpdateStatement Update([NotNull] Name target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            return new UpdateStatement
            {
                Target = target
            };
        }


        /*
        CREATE SCHEMA schema_name_clause [ <schema_element> [ ...n ] ]

<schema_name_clause> ::=
    {
    schema_name
    | AUTHORIZATION owner_name
    | schema_name AUTHORIZATION owner_name
    }

<schema_element> ::= 
    { 
        table_definition | view_definition | grant_statement | 
        revoke_statement | deny_statement 
    }
    
            
            DROP SCHEMA  [ IF EXISTS ] schema_name - IOF EXISTS FOR 2016 (and azure ?)
            
            */

        public static CreateSchemaStatement CreateSchema([NotNull] Name name, bool checkIfNotExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new CreateSchemaStatement { Name = name, CheckIfNotExists = checkIfNotExists };
        }

        public static DropSchemaStatement DropSchema([NotNull] Name name, bool checkExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new DropSchemaStatement { Name = name, CheckExists = checkExists };
        }

        public static AlterSchemaStatement AlterSchema([NotNull] Name name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new AlterSchemaStatement { Name = name };
        }

        public static DropTableStatement DropTable([NotNull] Name name, bool checkExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new DropTableStatement { Name = name, CheckExists = checkExists };
        }

        public static DropTableStatement DropTemporaryTable([NotNull] Name name, bool checkExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new DropTableStatement
            {
                Name = name,
                IsTemporary = true,
                CheckExists = checkExists
            };
        }

        public static CreateTableStatement CreateTable([NotNull] Name name, bool checkIfNotExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new CreateTableStatement
            {
                Name = name,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static CreateTableStatement CreateTemporaryTable([NotNull] Name name, bool checkIfNotExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new CreateTableStatement
            {
                Name = name,
                IsTemporary = true,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static CreateTableStatement CreateTableVariable([NotNull] Name name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new CreateTableStatement
            {
                Name = name,
                IsTableVariable = true
            };
        }

        public static CreateViewStatement CreateView([NotNull] Name name, [NotNull] IStatement definitionStatement,
            bool checkIfNotExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (definitionStatement == null) throw new ArgumentNullException(nameof(definitionStatement));
            return new CreateViewStatement
            {
                Name = name,
                DefinitionStatement = definitionStatement,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static CreateViewStatement CreateTemporaryView([NotNull] Name name,
            [NotNull] IStatement definitionStatement,
            bool checkIfNotExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (definitionStatement == null) throw new ArgumentNullException(nameof(definitionStatement));
            return new CreateViewStatement
            {
                Name = name,
                DefinitionStatement = definitionStatement,
                IsTemporary = true,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static CreateOrAlterViewStatement CreateOrAlterView([NotNull] Name name,
            [NotNull] IStatement definitionStatement)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (definitionStatement == null) throw new ArgumentNullException(nameof(definitionStatement));
            return new CreateOrAlterViewStatement
            {
                Name = name,
                DefinitionStatement = definitionStatement
            };
        }


        public static AlterViewStatement AlterView([NotNull] Name name, [NotNull] IStatement definitionStatement)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (definitionStatement == null) throw new ArgumentNullException(nameof(definitionStatement));
            return new AlterViewStatement
            {
                Name = name,
                DefinitionStatement = definitionStatement
            };
        }

        public static DropViewStatement DropView([NotNull] Name name, bool checkExists = false)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return new DropViewStatement
            {
                Name = name,
                CheckExists = checkExists
            };
        }

        public static CommentToken Comment([NotNull] string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(comment));
            return new CommentToken { Content = Snippet(comment) };
        }

        public static CommentStatement Comment([NotNull] IStatement statement)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return new CommentStatement { Content = statement };
        }

        public static BreakStatement Break => new BreakStatement();

        public static ContinueStatement Continue => new ContinueStatement();

        public static ExitStatement Exit => new ExitStatement();

        public static GotoStatement Goto([NotNull] string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(label));
            return new GotoStatement
            {
                Label = label
            };
        }

        public static LabelStatement Label([NotNull] string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(label));
            return new LabelStatement
            {
                Label = label
            };
        }


        public static ReturnStatement Return(int value)
        {
            return new ReturnStatement
            {
                ReturnExpression = Scalar(value)
            };
        }

        public static ReturnStatement Return([CanBeNull] Token value = null)
        {
            return new ReturnStatement
            {
                ReturnExpression = value
            };
        }

        public static ThrowStatement Throw()
        {
            return new ThrowStatement();
        }

        public static ThrowStatement Throw(int errorNumber, [NotNull] string message, int state)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            return new ThrowStatement
            {
                ErrorNumber = Scalar(errorNumber),
                Message = Scalar(message),
                State = Scalar(state)
            };
        }

        public static ThrowStatement Throw(string message)
        {
            return new ThrowStatement
            {
                Message = Sql.Scalar(message)
            };
        }
        public static ThrowStatement Throw([NotNull] Token errorNumber, [NotNull] Token message, [NotNull] Token state)
        {
            if (errorNumber == null) throw new ArgumentNullException(nameof(errorNumber));
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (state == null) throw new ArgumentNullException(nameof(state));
            return new ThrowStatement
            {
                ErrorNumber = errorNumber,
                Message = message,
                State = state
            };
        }

        public static TryCatchStatement Try([NotNull] IStatement tryStatement)
        {
            if (tryStatement == null) throw new ArgumentNullException(nameof(tryStatement));
            return new TryCatchStatement
            {
                TryStatement = tryStatement
            };
        }

        public static WhileStatement While([NotNull] Token condition)
        {
            if (condition == null) throw new ArgumentNullException(nameof(condition));
            return new WhileStatement
            {
                Condition = condition
            };
        }

        #endregion Statements
    }
}