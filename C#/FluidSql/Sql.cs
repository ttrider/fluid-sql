// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TTRider.FluidSql
{
    public partial class Sql
    {
        public static Name Star(string source = null)
        {
            return !String.IsNullOrWhiteSpace(source) ? new Name(source, "*") : new Name("*");
        }

        public static Name Name(params string[] names)
        {
            return new Name(names);
        }

        public static Snippet Snippet(string value, params Parameter[] parameters)
        {
            var val = new Snippet {Value = value};

            foreach (var p in parameters)
            {
                val.Parameters.Add(p);
            }
            return val;
        }

        public static Snippet Snippet(string value, IEnumerable<Parameter> parameters)
        {
            var val = new Snippet {Value = value};

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    val.Parameters.Add(p);
                }
            }
            return val;
        }

        public static SnippetStatement SnippetStatement(string value, IEnumerable<Parameter> parameters)
        {
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

        public static SnippetStatement SnippetStatement(string value, params Parameter[] parameters)
        {
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

        public static Scalar Scalar(object value)
        {
            return new Scalar {Value = value};
        }


        public static Token Group(Token value)
        {
            return new GroupToken {Token = value};
        }

        public static Token Exists(Token value)
        {
            return new ExistsToken {Token = value};
        }

        public static Token NotExists(Token value)
        {
            return new ExistsToken {Token = value}.Not();
        }

        public static Token Not(Token value)
        {
            return value.Not();
        }

        public static Token All(SelectStatement subQuery)
        {
            return new AllToken {Token = subQuery};
        }

        public static Token Any(SelectStatement subQuery)
        {
            return new AnyToken {Token = subQuery};
        }

        public static Token Some(SelectStatement subQuery)
        {
            return new AnyToken {Token = subQuery};
        }


        public static Order Order(Name column, Direction direction = Direction.Asc)
        {
            return new Order {Column = column, Direction = direction};
        }

        public static Order Order(string column, Direction direction = Direction.Asc)
        {
            return new Order {Column = new Name(column), Direction = direction};
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
                Name = Name(name),
                On = Name(on)
            };
        }

        public static AlterIndexStatement AlterIndex(string name, string on)
        {
            return new AlterIndexStatement
            {
                Name = Name(name),
                On = Name(on)
            };
        }

        public static AlterIndexStatement AlterIndexAll(string on)
        {
            return new AlterIndexStatement
            {
                Name = null,
                On = Name(on)
            };
        }

        public static DropIndexStatement DropIndex(string name, string on)
        {
            return new DropIndexStatement
            {
                Name = Name(name),
                On = Name(on)
            };
        }



        public static SetStatement Assign(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new AssignToken {First = target, Second = expression}
            };
        }

        public static SetStatement PlusAssign(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new PlusToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement MinusAssign(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new MinusToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement DivideAssign(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new DivideToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement BitwiseAndAssign(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseAndToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement BitwiseOrAssign(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseOrToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement BitwiseXorAssign(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseXorToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement BitwiseNotAssign(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseNotToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement ModuloAssign(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new ModuloToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement MultiplyAssign(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new MultiplyToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement Set(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new AssignToken {First = target, Second = expression}
            };
        }

        public static SetStatement PlusSet(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new PlusToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement MinusSet(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new MinusToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement DivideSet(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new DivideToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement BitwiseAndSet(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseAndToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement BitwiseOrSet(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseOrToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement BitwiseXorSet(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseXorToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement BitwiseNotSet(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseNotToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement ModuloSet(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new ModuloToken {First = target, Second = expression, Equal = true}
            };
        }

        public static SetStatement MultiplySet(Name target, Token expression)
        {
            return new SetStatement
            {
                Assign = new MultiplyToken {First = target, Second = expression, Equal = true}
            };
        }

        #region Statements

        public static SelectStatement Select
        {
            get { return new SelectStatement(); }
        }

        public static DeleteStatement Delete
        {
            get { return new DeleteStatement(); }
        }

        public static InsertStatement Insert
        {
            get { return new InsertStatement(); }
        }

        public static MergeStatement Merge
        {
            get { return new MergeStatement(); }
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

        public static UpdateStatement Update(string target)
        {
            return new UpdateStatement
            {
                Target = Sql.Name(target)
            };
        }

        public static UpdateStatement Update(Name target)
        {
            return new UpdateStatement
            {
                Target = target
            };
        }

        public static DropTableStatement DropTable(Name name, bool checkExists = false)
        {
            return new DropTableStatement {Name = name, CheckExists = checkExists};
        }

        public static DropTableStatement DropTemporaryTable(Name name, bool checkExists = false)
        {
            return new DropTableStatement
            {
                Name = name,
                IsTemporary = true,
                CheckExists = checkExists
            };
        }

        public static CreateTableStatement CreateTable(Name name, bool checkIfNotExists = false)
        {
            return new CreateTableStatement
            {
                Name = name,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static CreateTableStatement CreateTemporaryTable(Name name, bool checkIfNotExists = false)
        {
            return new CreateTableStatement
            {
                Name = name,
                IsTemporary = true,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static CreateTableStatement CreateTableVariable(Name name)
        {
            return new CreateTableStatement
            {
                Name = name,
                IsTableVariable = true
            };
        }

        public static CreateViewStatement CreateView(Name name, IStatement definitionStatement)
        {
            return new CreateViewStatement
            {
                Name = name,
                DefinitionQuery = definitionStatement,
            };
        }

        public static AlterViewStatement AlterView(Name name, IStatement definitionStatement)
        {
            return new AlterViewStatement
            {
                Name = name,
                DefinitionStatement = definitionStatement
            };
        }
        public static DropViewStatement DropView(Name name)
        {
            return new DropViewStatement { Name = name};
        }

        public static CommentToken Comment(string comment)
        {
            return new CommentToken {Content = Snippet(comment)};
        }

        public static CommentStatement Comment(IStatement statement)
        {
            return new CommentStatement {Content = statement};
        }

        public static BreakStatement Break
        {
            get { return new BreakStatement(); }
        }

        public static ContinueStatement Continue
        {
            get { return new ContinueStatement(); }
        }

        public static GotoStatement Goto(string label)
        {
            return new GotoStatement
            {
                Label = label
            };
        }
        public static LabelStatement Label(string label)
        {
            return new LabelStatement
            {
                Label = label
            };
        }


        public static ReturnStatement Return(int value)
        {
            return new ReturnStatement
            {
                ReturnExpression = Sql.Scalar(value)
            };
        }
        public static ReturnStatement Return(Token value = null)
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
        public static ThrowStatement Throw(int errorNumber, string message, int state)
        {
            return new ThrowStatement
            {
                ErrorNumber = Sql.Scalar(errorNumber),
                Message = Sql.Scalar(message),
                State = Sql.Scalar(state)
            };
        }
        public static ThrowStatement Throw(Token errorNumber, Token message, Token state)
        {
            return new ThrowStatement
            {
                ErrorNumber = errorNumber,
                Message = message,
                State = state
            };
        }

        public static TryCatchStatement Try(IStatement tryStatement)
        {
            return new TryCatchStatement
            {
                TryStatement = tryStatement
            };
        }

        public static WhileStatement While(Token condition)
        {
            return new WhileStatement
            {
                Condition = condition
            };
        }

        #endregion Statements
    }
}