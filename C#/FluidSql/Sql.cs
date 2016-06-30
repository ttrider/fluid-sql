// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;

namespace TTRider.FluidSql
{
    public partial class Sql
    {
        public static CaseToken Case => new CaseToken();

        public static Name Star(string source = null)
        {
            return !String.IsNullOrWhiteSpace(source) ? new Name(source, "*") : new Name("*");
        }

        public static Name Default(string source = null)
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

        public static Name NameAs(string name, string alias)
        {
            return new Name(name) { Alias = alias };
        }

        public static Name NameAs(string part1, string part2, string alias)
        {
            return new Name(part1, part2) { Alias = alias };
        }

        public static Name NameAs(string part1, string part2, string part3, string alias)
        {
            return new Name(part1, part2, part3) { Alias = alias };
        }

        public static Name NameAs(string part1, string part2, string part3, string part4, string alias)
        {
            return new Name(part1, part2, part3, part4) { Alias = alias };
        }

        public static Snippet Snippet(string value, params Parameter[] parameters)
        {
            var val = new Snippet { Value = value };

            foreach (var p in parameters)
            {
                val.Parameters.Add(p);
            }
            return val;
        }

        public static Snippet Template(string value, Token argument, params Token[] arguments)
        {
            return Template(value, Enumerable.Repeat(argument, 1).Union(arguments));
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

        public static Snippet Template(string value, IEnumerable<Token> arguments)
        {
            var val = new Snippet { Value = value };
            if (arguments != null)
            {
                foreach (var argument in arguments)
                {
                    val.Arguments.Add(argument);
                    if (argument is Parameter)
                    {
                        val.Parameters.Add((Parameter) argument);
                    }
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

        public static SnippetStatement TemplateStatement(string value, IEnumerable<Token> arguments)
        {
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
                        snippetStatement.Parameters.Add((Parameter) argument);
                    }
                }
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
                {
                    snippetStatement.Parameters.Add(parameter);
                }
            }
            return snippetStatement;
        }

        public static SnippetStatement TemplateStatement(string value, Token argument, params Token[] arguments)
        {
            return TemplateStatement(value, Enumerable.Repeat(argument, 1).Union(arguments));
        }

        public static Scalar Scalar(object value)
        {
            if (value is Scalar)
            {
                return value as Scalar;
            }
            return new Scalar { Value = value };
        }

        public static UnaryMinusToken Minus(Token value)
        {
            return new UnaryMinusToken { Token = value };
        }

        public static GroupToken Group(Token value)
        {
            return new GroupToken { Token = value };
        }

        public static ExpressionToken Exists(ExpressionToken value)
        {
            return new ExistsToken { Token = value };
        }

        public static ExpressionToken NotExists(ExpressionToken value)
        {
            return new ExistsToken { Token = value }.Not();
        }

        public static ExpressionToken Not(ExpressionToken value)
        {
            return value.Not();
        }

        public static ExpressionToken All(SelectStatement subQuery)
        {
            return new AllToken { Token = subQuery };
        }

        public static ExpressionToken Any(SelectStatement subQuery)
        {
            return new AnyToken { Token = subQuery };
        }

        public static ExpressionToken Some(SelectStatement subQuery)
        {
            return new AnyToken { Token = subQuery };
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

        public static DropIndexStatement DropIndex(Name name, Name on, bool checkExists)
        {
            return new DropIndexStatement
            {
                Name = name,
                On = on,
                CheckExists = checkExists
            };
        }

        public static DropIndexStatement DropIndex(Name name, bool checkExists)
        {
            return new DropIndexStatement
            {
                Name = name,
                CheckExists = checkExists
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

        public static AlterIndexStatement AlterIndexAll(Name on)
        {
            return new AlterIndexStatement
            {
                Name = null,
                On = on
            };
        }

        public static AlterIndexStatement Reindex(Name on)
        {
            return new AlterIndexStatement
            {
                Name = null,
                On = on
            }.Rebuild();
        }

        public static AlterIndexStatement Reindex(Name name, Name on)
        {
            return new AlterIndexStatement
            {
                Name = name,
                On = on
            }.Rebuild();
        }


        public static CTEDeclaration With(string name, params string[] columnNames)
        {
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

        public static CTEDeclaration With(string name, IEnumerable<string> columnNames)
        {
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

        public static SetStatement Assign(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new AssignToken { First = target, Second = expression }
            };
        }

        public static SetStatement PlusAssign(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new PlusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MinusAssign(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new MinusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement DivideAssign(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new DivideToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseAndAssign(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseAndToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseOrAssign(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseOrToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseXorAssign(Parameter target, ExpressionToken expression)
        {
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

        public static SetStatement ModuloAssign(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new ModuloToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MultiplyAssign(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new MultiplyToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement Set(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new AssignToken { First = target, Second = expression }
            };
        }

        public static SetStatement PlusSet(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new PlusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MinusSet(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new MinusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement DivideSet(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new DivideToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseAndSet(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseAndToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseOrSet(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseOrToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseXorSet(Parameter target, ExpressionToken expression)
        {
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

        public static SetStatement ModuloSet(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new ModuloToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MultiplySet(Parameter target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new MultiplyToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement Assign(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new AssignToken { First = target, Second = expression }
            };
        }

        public static SetStatement PlusAssign(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new PlusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MinusAssign(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new MinusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement DivideAssign(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new DivideToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseAndAssign(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseAndToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseOrAssign(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseOrToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseXorAssign(Name target, ExpressionToken expression)
        {
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

        public static SetStatement ModuloAssign(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new ModuloToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MultiplyAssign(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new MultiplyToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement Set(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new AssignToken { First = target, Second = expression }
            };
        }

        public static SetStatement PlusSet(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new PlusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MinusSet(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new MinusToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement DivideSet(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new DivideToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseAndSet(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseAndToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseOrSet(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new BitwiseOrToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement BitwiseXorSet(Name target, ExpressionToken expression)
        {
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

        public static SetStatement ModuloSet(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new ModuloToken { First = target, Second = expression, Equal = true }
            };
        }

        public static SetStatement MultiplySet(Name target, ExpressionToken expression)
        {
            return new SetStatement
            {
                Assign = new MultiplyToken { First = target, Second = expression, Equal = true }
            };
        }

        #endregion Set

        #region Statements

        public static ExecuteStatement Execute(string statement, IEnumerable<Parameter> parameters)
        {
            return Execute(SnippetStatement(statement), parameters);
        }

        public static ExecuteStatement Execute(string statement, params Parameter[] parameters)
        {
            return Execute(SnippetStatement(statement), (IEnumerable<Parameter>) parameters);
        }

        public static ExecuteStatement Execute(IStatement statement, params Parameter[] parameters)
        {
            return Execute(statement, (IEnumerable<Parameter>) parameters);
        }

        public static ExecuteStatement Execute(IStatement statement, IEnumerable<Parameter> parameters)
        {
            var stat = new ExecuteStatement
            {
                Target = statement
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

        public static ExecuteProcedureStatement ExecuteStoredProcedure(Name storedProcedureName,
            params Parameter[] parameters)
        {
            return ExecuteStoredProcedure(storedProcedureName, (IEnumerable<Parameter>) parameters);
        }

        public static ExecuteProcedureStatement ExecuteStoredProcedure(Name storedProcedureName,
            IEnumerable<Parameter> parameters)
        {
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

        public static DropProcedureStatement DropProcedure(Name name, bool checkExists = false)
        {
            return new DropProcedureStatement
            {
                Name = name,
                CheckExists = checkExists
            };
        }

        public static CreateProcedureStatement CreateProcedure(Name name, bool checkIfNotExists = false)
        {
            return new CreateProcedureStatement
            {
                Name = name,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static AlterProcedureStatement AlterProcedure(Name name, bool createIfNotExists = false)
        {
            return new AlterProcedureStatement
            {
                Name = name,
                CreateIfNotExists = createIfNotExists
            };
        }

        public static SelectStatement Select => new SelectStatement();

        public static DeleteStatement Delete => new DeleteStatement();

        public static InsertStatement Insert => new InsertStatement();

        public static MergeStatement Merge => new MergeStatement();

        #region Transaction

        public static BeginTransactionStatement BeginTransaction(Name name = null, string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description
            };
        }

        public static BeginTransactionStatement BeginSerializableTransaction(Name name = null, string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description,
                IsolationLevel = IsolationLevelType.Serializable
            };
        }

        public static BeginTransactionStatement BeginRepeatebleReadTransaction(Name name = null, string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description,
                IsolationLevel = IsolationLevelType.RepeatableRead
            };
        }

        public static BeginTransactionStatement BeginReadCommitedTransaction(Name name = null, string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description,
                IsolationLevel = IsolationLevelType.ReadCommited
            };
        }

        public static BeginTransactionStatement BeginReadUnCommitedTransaction(Name name = null, string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description,
                IsolationLevel = IsolationLevelType.ReadUnCommited
            };
        }

        public static BeginTransactionStatement BeginReadOnlyTransaction(Name name = null, string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description,
                AccessType = TransactionAccessType.ReadOnly
            };
        }

        public static BeginTransactionStatement BeginReadWriteTransaction(Name name = null, string description = null)
        {
            return new BeginTransactionStatement
            {
                Name = name,
                Description = description,
                AccessType = TransactionAccessType.ReadWrite
            };
        }

        public static CommitTransactionStatement CommitTransaction(Name name = null)
        {
            return new CommitTransactionStatement
            {
                Name = name
            };
        }

        public static CommitTransactionStatement ReleaseToSavepoint(Name name)
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

        public static RollbackTransactionStatement RollbackToSavepoint(Name name)
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

        public static SaveTransactionStatement SaveTransaction(Parameter parameter)
        {
            return new SaveTransactionStatement
            {
                Parameter = parameter
            };
        }

        public static SaveTransactionStatement Savepoint(Name name = null)
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

        #endregion Transaction

        public static StatementsStatement Statements(params IStatement[] statements)
        {
            var statement = new StatementsStatement();
            statement.Statements.AddRange(statements);
            return statement;
        }

        public static StatementsStatement Statements(IStatement statement, params IStatement[] statements)
        {
            var newstatement = new StatementsStatement();
            newstatement.Statements.Add(statement);
            newstatement.Statements.AddRange(statements);
            return newstatement;
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
                Target = Name(target)
            };
        }

        public static UpdateStatement Update(Name target)
        {
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

        public static CreateSchemaStatement CreateSchema(Name name, bool checkIfNotExists = false)
        {
            return new CreateSchemaStatement { Name = name, CheckIfNotExists = checkIfNotExists };
        }

        public static DropSchemaStatement DropSchema(Name name, bool checkExists = false)
        {
            return new DropSchemaStatement { Name = name, CheckExists = checkExists };
        }


        public static DropTableStatement DropTable(Name name, bool checkExists = false)
        {
            return new DropTableStatement { Name = name, CheckExists = checkExists };
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

        public static CreateViewStatement CreateView(Name name, IStatement definitionStatement,
            bool checkIfNotExists = false)
        {
            return new CreateViewStatement
            {
                Name = name,
                DefinitionStatement = definitionStatement,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static CreateViewStatement CreateTemporaryView(Name name, IStatement definitionStatement,
            bool checkIfNotExists = false)
        {
            return new CreateViewStatement
            {
                Name = name,
                DefinitionStatement = definitionStatement,
                IsTemporary = true,
                CheckIfNotExists = checkIfNotExists
            };
        }

        public static CreateOrAlterViewStatement CreateOrAlterView(Name name, IStatement definitionStatement)
        {
            return new CreateOrAlterViewStatement
            {
                Name = name,
                DefinitionStatement = definitionStatement
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

        public static DropViewStatement DropView(Name name, bool checkExists = false)
        {
            return new DropViewStatement
            {
                Name = name,
                CheckExists = checkExists
            };
        }

        public static CommentToken Comment(string comment)
        {
            return new CommentToken { Content = Snippet(comment) };
        }

        public static CommentStatement Comment(IStatement statement)
        {
            return new CommentStatement { Content = statement };
        }

        public static BreakStatement Break => new BreakStatement();

        public static ContinueStatement Continue => new ContinueStatement();

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
                ReturnExpression = Scalar(value)
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
                ErrorNumber = Scalar(errorNumber),
                Message = Scalar(message),
                State = Scalar(state)
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