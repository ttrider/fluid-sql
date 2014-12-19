using System;
using System.Collections.Generic;
using System.Linq;

namespace TTRider.FluidSql.Providers.SqlServer
{
    public static class SqlServerVisitor
    {
        private const string EqualsVal = " = ";
        private const string AssignVal = " = ";
        private const string NotEqualVal = " <> ";
        private const string LessVal = " < ";
        private const string NotLessVal = " !< ";
        private const string LessOrEqualVal = " <= ";
        private const string GreaterVal = " > ";
        private const string NotGreaterVal = " !> ";
        private const string GreaterOrEqualVal = " >= ";
        private const string AndVal = " AND ";
        private const string OrVal = " OR ";

        private const string PlusVal = " + ";
        private const string MinusVal = " - ";
        private const string DivideVal = " / ";
        private const string ModuloVal = " % ";
        private const string MultiplyVal = " * ";

        private const string BitwiseAndVal = " & ";
        private const string BitwiseOrVal = " | ";
        private const string BitwiseXorVal = " ^ ";
        private const string BitwiseNotVal = " ~ ";

        private const string PlusEqVal = " += ";
        private const string MinusEqVal = " -= ";
        private const string DivideEqVal = " /= ";
        private const string ModuloEqVal = " %= ";
        private const string MultiplyEqVal = " *= ";

        private const string BitwiseAndEqVal = " &= ";
        private const string BitwiseOrEqVal = " |= ";
        private const string BitwiseXorEqVal = " ^= ";
        private const string BitwiseNotEqVal = " ~= ";

        private static readonly string[] DbTypeStrings =
        {
            "BIGINT",//BigInt = 0,
            "BINARY",//Binary = 1,      ()
            "BIT",//Bit = 2,
            "CHAR",//Char = 3,          ()
            "DATETIME",//DateTime = 4,
            "DECIMAL",//Decimal = 5,    (,)
            "FLOAT",//Float = 6,
            "IMAGE",//Image = 7,
            "INT", //Int = 8,
            "MONEY",//Money = 9,
            "NCHAR", //NChar = 10,      ()
            "NTEXT", //NText = 11,
            "NVARCHAR",//NVarChar = 12, ()
            "REAL", //Real = 13,
            "UNIQUEIDENTIFIER",//UniqueIdentifier = 14,
            "SMALLDATETIME",//SmallDateTime = 15,
            "SMALLINT", //SmallInt = 16,
            "SMALLMONEY",//SmallMoney = 17,
            "TEXT",//Text = 18,
            "TIMESTAMP",//Timestamp = 19,
            "TINYINT",//TinyInt = 20,
            "VARBINARY",//VarBinary = 21,   ()
            "VARCHAR",//VarChar = 22,       ()
            "SQL_VARIANT", //Variant = 23,
            "Xml", //Xml = 25,
            "", //Udt = 29,
            "", //Structured = 30,
            "DATE",//Date = 31,
            "TIME", //Time = 32,            ()
            "DATETIME2", //DateTime2 = 33,  ()
            "DateTimeOffset", //DateTimeOffset = 34, ()
        };



        private static readonly string[] JoinStrings = new string[]
        {
            " INNER JOIN ", //Inner = 0,
            " LEFT OUTER JOIN ", //LeftOuter = 1,
            " RIGHT OUTER JOIN ", //RightOuter = 2,
            " FULL OUTER JOIN ", //FullOuter = 3,
            " CROSS JOIN ", //Cross = 4,
        };



        private static readonly Dictionary<Type, Action<Token, VisitorState>> TokenVisitors =
            new Dictionary<Type, Action<Token, VisitorState>>()
            {
                {typeof (SelectStatement), VisitStatementToken},
                {typeof (Union), VisitStatementToken},
                {typeof (Intersect), VisitStatementToken},
                {typeof (Except), VisitStatementToken},

                {typeof (Scalar), VisitScalarToken},
                {typeof (Name), VisitNameToken},
                {typeof (Parameter), VisitParameterToken},
                {typeof (Snippet), VisitSnippetToken},
                {typeof (Function), VisitFunctionToken},

                {typeof (IsEqualsToken), VisitIsEqualsToken},
                {typeof (NotEqualToken), VisitNotEqualToken},
                {typeof (LessToken), VisitLessToken},
                {typeof (NotLessToken), VisitNotLessToken},
                {typeof (LessOrEqualToken), VisitLessOrEqualToken},
                {typeof (GreaterToken), VisitGreaterToken},
                {typeof (NotGreaterToken), VisitNotGreaterToken},
                {typeof (GreaterOrEqualToken), VisitGreaterOrEqualToken},
                {typeof (AndToken), VisitAndToken},
                {typeof (OrToken), VisitOrToken},

                {typeof (PlusToken), VisitPlusToken},
                {typeof (MinusToken), VisitMinusToken},
                {typeof (DivideToken), VisitDivideToken},
                {typeof (ModuloToken), VisitModuloToken},
                {typeof (MultiplyToken), VisitMultiplyToken},
                {typeof (BitwiseAndToken),VisitBitwiseAndToken},
                {typeof (BitwiseOrToken ),VisitBitwiseOrToken },
                {typeof (BitwiseXorToken),VisitBitwiseXorToken},
                {typeof (BitwiseNotToken),VisitBitwiseNotToken},

                {typeof (ContainsToken), VisitContainsToken},
                {typeof (StartsWithToken), VisitStartsWithToken},
                {typeof (EndsWithToken), VisitEndsWithToken},
                {typeof (LikeToken), VisitLikeToken},
                
                {typeof (GroupToken), VisitGroupToken},
                {typeof (NotToken), VisitNotToken},
                {typeof (IsNullToken), VisitIsNullToken},
                {typeof (IsNotNullToken), VisitIsNotNullToken},
                {typeof (ExistsToken), VisitExistsToken},
                {typeof (AllToken), VisitAllToken},
                {typeof (AnyToken), VisitAnyToken},
                {typeof (AssignToken), VisitAssignToken},
                

                {typeof (BetweenToken), VisitBetweenToken},
                {typeof (InToken), VisitInToken},
                {typeof (NotInToken), VisitNotInToken},

                {typeof (CommentToken), VisitCommentToken},
                {typeof (StringifyToken), VisitStringifyToken},
            };

        private static readonly Dictionary<Type, Action<IStatement, VisitorState>> StatementVisitors =
            new Dictionary<Type, Action<IStatement, VisitorState>>()
            {
                {typeof (DeleteStatement), VisitDelete},

                {typeof (InsertStatement), VisitInsert},

                {typeof (SelectStatement), VisitSelect},

                {typeof (SetStatement), VisitSet},

                {typeof (Union), VisitUnion},
                {typeof (Intersect), VisitIntersect},
                {typeof (Except), VisitExcept},

                {typeof (BeginTransactionStatement), VisitBeginTransaction},
                {typeof (CommitTransactionStatement), VisitCommitTransaction},
                {typeof (RollbackTransactionStatement), VisitRollbackTransaction},
                {typeof (StatementsStatement), VisitStatementsStatement},
                {typeof (SaveTransactionStatement), VisitSaveTransaction},

                {typeof (DeclareStatement), VisitDeclareStatement},
                {typeof (IfStatement), VisitIfStatement},

                {typeof (CreateTableStatement), VisitCreateTableStatement},
                {typeof (DropTableStatement), VisitDropTableStatement},
                
                {typeof (CreateIndexStatement), VisitCreateIndexStatement},
                {typeof (AlterIndexStatement), VisitAlterIndexStatement},
                {typeof (DropIndexStatement), VisitDropIndexStatement},

                {typeof (CommentStatement), VisitCommentStatement},
                {typeof (StringifyStatement), VisitStringifyStatement},
                {typeof (SnippetStatement), VisitSnippetStatement},
            };

        public static VisitorState Compile(IStatement statement)
        {
            var state = new VisitorState();

            VisitStatement(statement, state);
            // we need to make sure that the last non-whitespace character
            // is ';' unless it is */
            //TODO: need a better code here
            for (var i = state.Buffer.Length - 1; i >= 0; i--)
            {
                var ch = state.Buffer[i];
                if (!Char.IsWhiteSpace(ch))
                {
                    if (ch == ';')
                    {
                        break;
                    }
                    if (ch == '/')
                    {
                        if (i > 0 && state.Buffer[i - 1] == '*')
                        {
                            break;
                        }
                    }

                    state.Buffer.Append(";");
                    break;
                }
            }

            return state;
        }

        internal static Name GetTempTableName(Name name)
        {
            var namePart = name.Parts.Last();

            if (!namePart.StartsWith("#"))
            {
                namePart = "#" + namePart;
            }
            var n = Sql.Name("tempdb");
            n.Parts.Add("");
            n.Parts.Add(namePart);
            return n;
        }

        internal static string GetTableVariableName(Name name)
        {
            var namePart = name.Parts.Last();

            if (!namePart.StartsWith("@"))
            {
                namePart = "@" + namePart;
            }
            return namePart;
        }

        #region Statements
        static void VisitStatement(IStatement statement, VisitorState state)
        {
            StatementVisitors[statement.GetType()](statement, state);
        }

        static void VisitUnion(IStatement statement, VisitorState state)
        {
            var unionStatement = (Union)statement;

            VisitStatement(unionStatement.First, state);

            state.Buffer.Append((unionStatement.All ? " UNION ALL " : " UNION "));

            VisitStatement(unionStatement.Second, state);
        }


        static void VisitExcept(IStatement statement, VisitorState state)
        {
            var exceptStatement = (Except)statement;

            VisitStatement(exceptStatement.First, state);

            state.Buffer.Append(" EXCEPT ");

            VisitStatement(exceptStatement.Second, state);
        }
        static void VisitIntersect(IStatement statement, VisitorState state)
        {
            var intersectStatement = (Intersect)statement;

            VisitStatement(intersectStatement.First, state);
            state.Buffer.Append(" INTERSECT ");
            VisitStatement(intersectStatement.Second, state);
        }

        private static void VisitSet(IStatement statement, VisitorState state)
        {
            var setStatement = (SetStatement)statement;

            state.Buffer.Append("SET ");

            if (setStatement.Assign != null)
            {
                VisitToken(setStatement.Assign, false, state);
            }
        }

        private static void VisitSelect(IStatement statement, VisitorState state)
        {
            var selectStatement = (SelectStatement)statement;

            state.Buffer.Append("SELECT");

            if (selectStatement.Distinct)
            {
                state.Buffer.Append(" DISTINCT");
            }

            VisitTop(selectStatement.Top, state);

            // assignments
            if (selectStatement.Assign.Count > 0)
            {
                VisitTokenSet(" ", ", ", "", selectStatement.Assign, false, state);
            }
            else
            {
                // output columns
                if (selectStatement.Output.Count == 0)
                {
                    state.Buffer.Append(" *");
                }
                else
                {
                    VisitTokenSet(" ", ", ", "", selectStatement.Output, true, state);
                }
            }

            VisitInto(selectStatement.Into, state);

            VisitFrom(selectStatement.From, state);

            VisitJoin(selectStatement.Joins, state);

            VisitWhere(selectStatement.Where, state);

            VisitGroupBy(selectStatement.GroupBy, state);

            VisitHaving(selectStatement.Having, state);

            VisitOrderBy(selectStatement.OrderBy, state);


            //WHERE
            //WITH CUBE or WITH ROLLUP
        }

        private static void VisitInto(Name into, VisitorState state)
        {
            if (into != null)
            {
                state.Buffer.Append(" INTO ");
                state.Buffer.Append(into.FullName);
            }
        }

        static void VisitDelete(IStatement statement, VisitorState state)
        {
            var deleteStatement = (DeleteStatement)statement;

            state.Buffer.Append("DELETE");

            VisitTop(deleteStatement.Top, state);

            if (deleteStatement.Joins.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(deleteStatement.From.Alias))
                {
                    state.Buffer.Append(" [");
                    state.Buffer.Append(deleteStatement.From.Alias);
                    state.Buffer.Append("]");
                }

                VisitOutput(deleteStatement.Output, deleteStatement.OutputInto, state);

                VisitFrom(deleteStatement.From, state);

                VisitJoin(deleteStatement.Joins, state);

                VisitWhere(deleteStatement.Where, state);
            }
            else
            {
                VisitFrom(deleteStatement.From, state);

                VisitOutput(deleteStatement.Output, deleteStatement.OutputInto, state);

                VisitWhere(deleteStatement.Where, state);
            }
        }

        static void VisitInsert(IStatement statement, VisitorState state)
        {
            var insertStatement = (InsertStatement)statement;

            state.Buffer.Append("INSERT");

            VisitTop(insertStatement.Top, state);

            VisitInto(insertStatement.Into, state);

            VisitTokenSet(" (", ", ", ")", insertStatement.Columns, true, state);

            VisitOutput(insertStatement.Output, insertStatement.OutputInto, state);

            if (insertStatement.DefaultValues)
            {
                state.Buffer.Append(" DEFAULT VALUES");
            }
            else if (insertStatement.Values.Count > 0)
            {
                var separator = " VALUES";
                foreach (var valuesSet in insertStatement.Values)
                {
                    state.Buffer.Append(separator);
                    separator = ",";

                    VisitTokenSet(" (", ", ", ")", valuesSet, false, state);
                }
            }
            else if (insertStatement.From != null)
            {
                state.Buffer.Append(" ");
                VisitStatement(insertStatement.From, state);
            }
        }


        private static bool VisitTransactionName(TransactionStatement statement, VisitorState state)
        {
            if (statement.Name != null)
            {
                state.Buffer.Append(" ");
                state.Buffer.Append(statement.Name.FullName);
                return true;
            }
            if (statement.Parameter != null)
            {
                state.Buffer.Append(" ");
                state.Buffer.Append(statement.Parameter.Name);
                state.Parameters.Add(statement.Parameter);
                return true;
            }
            return false;
        }

        private static void VisitBeginTransaction(IStatement statement, VisitorState state)
        {
            var ts = (BeginTransactionStatement)statement;
            state.Buffer.Append("BEGIN TRANSACTION");
            if (VisitTransactionName(ts, state) && !string.IsNullOrWhiteSpace(ts.Description))
            {
                state.Buffer.Append(" WITH MARK '");
                state.Buffer.Append(ts.Description);
                state.Buffer.Append("'");
            }
        }
        private static void VisitCommitTransaction(IStatement statement, VisitorState state)
        {
            var ts = (TransactionStatement)statement;
            state.Buffer.Append("COMMIT TRANSACTION");
            VisitTransactionName(ts, state);
        }
        private static void VisitRollbackTransaction(IStatement statement, VisitorState state)
        {
            var ts = (TransactionStatement)statement;
            state.Buffer.Append("ROLLBACK TRANSACTION");
            VisitTransactionName(ts, state);
        }
        private static void VisitSaveTransaction(IStatement statement, VisitorState state)
        {
            var ts = (TransactionStatement)statement;
            state.Buffer.Append("SAVE TRANSACTION");
            VisitTransactionName(ts, state);
        }


        private static void VisitStatementsStatement(IStatement statement, VisitorState state)
        {
            var ss = (StatementsStatement)statement;
            foreach (var subStatement in ss.Statements)
            {
                VisitStatement(subStatement, state);
                state.Buffer.AppendLine(";");
            }
        }

        private static void VisitDeclareStatement(IStatement statement, VisitorState state)
        {
            var ss = (DeclareStatement)statement;

            if (ss.Variable != null)
            {
                state.Variables.Add(ss.Variable);

                state.Buffer.Append("DECLARE ");
                state.Buffer.Append(ss.Variable.Name);

                VisitType(ss.Variable, state);

                if (ss.Initializer != null)
                {
                    state.Buffer.Append(" = ");
                    VisitToken(ss.Initializer, false, state);
                }
            }
        }

        private static void VisitType(TypedToken typedToken, VisitorState state)
        {
            if (typedToken.DbType.HasValue)
            {
                state.Buffer.Append(" ");
                state.Buffer.Append(DbTypeStrings[(int)typedToken.DbType]);
            }

            if (typedToken.Length.HasValue || typedToken.Precision.HasValue || typedToken.Scale.HasValue)
            {
                state.Buffer.Append("(");
                if (typedToken.Length.HasValue)
                {
                    state.Buffer.Append(typedToken.Length.Value == -1 ? "MAX" : typedToken.Length.Value.ToString());
                }
                else if (typedToken.Precision.HasValue)
                {
                    state.Buffer.Append(typedToken.Precision.Value);

                    if (typedToken.Scale.HasValue)
                    {
                        state.Buffer.Append(",");
                        state.Buffer.Append(typedToken.Scale.Value);
                    }
                }

                state.Buffer.Append(")");
            }

        }

        private static void VisitIfStatement(IStatement statement, VisitorState state)
        {
            var ifs = (IfStatement)statement;

            if (ifs.Condition != null)
            {
                state.Buffer.Append("IF ");
                VisitToken(ifs.Condition, false, state);

                if (ifs.Then != null)
                {
                    state.Buffer.Append("\r\nBEGIN;\r\n");
                    VisitStatement(ifs.Then, state);
                    state.Buffer.Append("END;\r\n");

                    if (ifs.Else != null)
                    {
                        state.Buffer.Append("ELSE\r\nBEGIN;\r\n");
                        VisitStatement(ifs.Else, state);
                        state.Buffer.Append("END;\r\n");
                    }
                }
            }
        }

        private static void VisitDropTableStatement(IStatement statement, VisitorState state)
        {
            var s = (DropTableStatement)statement;

            var tableName = ((s.IsTemporary) ? GetTempTableName(s.Name) : s.Name).FullName;

            if (s.CheckExists)
            {
                state.Buffer.Append("IF OBJECT_ID(N'");
                state.Buffer.Append(tableName);
                state.Buffer.Append("',N'U') IS NOT NULL ");
            }

            state.Buffer.Append("DROP TABLE ");
            state.Buffer.Append(tableName);
            state.Buffer.Append(";");
        }

        private static void VisitCreateTableStatement(IStatement statement, VisitorState state)
        {
            var createStatement = (CreateTableStatement)statement;

            if (createStatement.IsTableVariable)
            {
                state.Buffer.Append("DECLARE ");
                state.Buffer.Append(GetTableVariableName(createStatement.Name));
                state.Buffer.Append(" TABLE");
            }
            else
            {
                var tableName = ((createStatement.IsTemporary) ? GetTempTableName(createStatement.Name) : createStatement.Name).FullName;

                if (createStatement.CheckIfNotExists)
                {
                    state.Buffer.Append("IF OBJECT_ID(N'");
                    state.Buffer.Append(tableName);
                    state.Buffer.Append("',N'U') IS NULL ");
                    state.Buffer.Append(" BEGIN; ");
                }

                state.Buffer.Append("CREATE TABLE ");
                state.Buffer.Append(tableName);
                if (createStatement.AsFiletable)
                {
                    state.Buffer.Append(" AS FileTable");
                }
            }


            var separator = " (";
            foreach (var column in createStatement.Columns)
            {
                state.Buffer.Append(separator);
                separator = ", ";

                state.Buffer.Append("[");
                state.Buffer.Append(column.Name);
                state.Buffer.Append("]");

                VisitType(column, state);

                if (column.Sparse)
                {
                    state.Buffer.Append(" SPARSE");
                }
                if (column.Null.HasValue)
                {
                    state.Buffer.Append(column.Null.Value ? " NULL" : " NOT NULL");
                }
                if (column.Identity.On)
                {
                    state.Buffer.AppendFormat(" IDENTITY ({0}, {1})", column.Identity.Seed, column.Identity.Increment);
                }
                if (column.RowGuid)
                {
                    state.Buffer.Append(" ROWGUIDCOL");
                }
                if (column.DefaultValue != null)
                {
                    state.Buffer.Append(" DEFAULT (");
                    VisitToken(column.DefaultValue, false, state);
                    state.Buffer.Append(" )");
                }
            }

            if (createStatement.PrimaryKey != null)
            {
                if (createStatement.IsTableVariable)
                {
                    state.Buffer.Append(",");
                }
                else
                {
                    state.Buffer.Append(", CONSTRAINT ");
                    state.Buffer.Append(createStatement.PrimaryKey.Name.FullName);
                }

                state.Buffer.Append(" PRIMARY KEY (");
                state.Buffer.Append(string.Join(", ", createStatement.PrimaryKey.Columns.Select(n => n.Column.FullName + (n.Direction == Direction.Asc ? " ASC" : " DESC"))));
                state.Buffer.Append(" )");
            }

            foreach (var unique in createStatement.UniqueConstrains)
            {
                if (createStatement.IsTableVariable)
                {
                    state.Buffer.Append(",");
                }
                else
                {
                    state.Buffer.Append(", CONSTRAINT ");
                    state.Buffer.Append(unique.Name.FullName);
                }
                state.Buffer.Append(" UNIQUE");
                if (unique.Clustered.HasValue)
                {
                    state.Buffer.Append(unique.Clustered.Value ? " CLUSTERED (" : " NONCLUSTERED (");
                }
                state.Buffer.Append(string.Join(", ", unique.Columns.Select(n => n.Column.FullName + (n.Direction == Direction.Asc ? " ASC" : " DESC"))));
                state.Buffer.Append(" )");
            }


            state.Buffer.Append(");");

            // if indecies are set, create them
            if (createStatement.Indicies.Count > 0 && !createStatement.IsTableVariable)
            {
                foreach (var createIndexStatement in createStatement.Indicies)
                {
                    VisitCreateIndexStatement(createIndexStatement, state);
                }
            }


            if (createStatement.CheckIfNotExists && !createStatement.IsTableVariable)
            {
                state.Buffer.Append(" END;");
            }

        }


        private static void VisitCommentStatement(IStatement statement, VisitorState state)
        {
            var commentStatement = (CommentStatement)statement;

            state.Buffer.Append(" /* ");

            VisitStatement(commentStatement.Content, state);

            state.Buffer.Append(" */ ");
        }

        private static void VisitStringifyStatement(IStatement statement, VisitorState state)
        {
            var commentStatement = (StringifyStatement)statement;

            state.Buffer.Append("N'");

            var startIndex = state.Buffer.Length;
            VisitStatement(commentStatement.Content, state);
            var endIndex = state.Buffer.Length;

            // replace all "'" characters with "''"
            if (endIndex > startIndex)
            {
                state.Buffer.Replace("'", "''", startIndex, endIndex - startIndex);
            }

            state.Buffer.Append("'");
        }


        private static void VisitSnippetStatement(IStatement statement, VisitorState state)
        {
            var snippetStatement = (SnippetStatement)statement;
            state.Buffer.Append(snippetStatement.Value);
        }

        private static void VisitCreateIndexStatement(IStatement statement, VisitorState state)
        {
            var createIndexStatement = (CreateIndexStatement)statement;

            state.Buffer.Append("CREATE");

            if (createIndexStatement.Unique)
            {
                state.Buffer.Append(" UNIQUE");
            }

            if (createIndexStatement.Clustered.HasValue)
            {
                state.Buffer.Append(createIndexStatement.Clustered.Value ? " CLUSTERED" : " NONCLUSTERED");
            }
            state.Buffer.Append(" INDEX ");

            VisitToken(createIndexStatement.Name, false, state);

            state.Buffer.Append(" ON ");

            VisitToken(createIndexStatement.On, false, state);

            // columns
            state.Buffer.Append(" (");
            state.Buffer.Append(string.Join(", ", createIndexStatement.Columns.Select(n => n.Column.FullName + (n.Direction == Direction.Asc ? " ASC" : " DESC"))));
            state.Buffer.Append(")");

            VisitTokenSet(" INCLUDE (", ", ", ")", createIndexStatement.Include, false, state);

            VisitWhere(createIndexStatement.Where, state);

            if (createIndexStatement.With.IsDefined)
            {
                state.Buffer.Append(" WITH (");

                VisitWith(createIndexStatement.With.PadIndex, "PAD_INDEX", state);

                VisitWith(createIndexStatement.With.Fillfactor, "FILLFACTOR", state);
                VisitWith(createIndexStatement.With.SortInTempdb, "SORT_IN_TEMPDB", state);
                VisitWith(createIndexStatement.With.IgnoreDupKey, "IGNORE_DUP_KEY", state);
                VisitWith(createIndexStatement.With.StatisticsNorecompute, "STATISTICS_NORECOMPUTE", state);
                VisitWith(createIndexStatement.With.DropExisting, "DROP_EXISTING", state);
                VisitWith(createIndexStatement.With.Online, "ONLINE", state);
                VisitWith(createIndexStatement.With.AllowRowLocks, "ALLOW_ROW_LOCKS", state);
                VisitWith(createIndexStatement.With.AllowPageLocks, "ALLOW_PAGE_LOCKS", state);
                VisitWith(createIndexStatement.With.MaxDegreeOfParallelism, "MAXDOP", state);

                state.Buffer.Append(" )");
            }

            state.Buffer.Append(";");
        }

        private static void VisitAlterIndexStatement
            (IStatement statement, VisitorState state)
        {
            var alterStatement = (AlterIndexStatement)statement;

            state.Buffer.Append("ALTER INDEX ");

            if (alterStatement.Name == null)
            {
                state.Buffer.Append("ALL");
            }
            else
            {
                VisitToken(alterStatement.Name, false, state);
            }

            state.Buffer.Append(" ON ");

            VisitToken(alterStatement.On, false, state);

            if (alterStatement.Rebuild)
            {
                state.Buffer.Append(" REBUILD");

                //TODO: [PARTITION = ALL]
                if (alterStatement.RebuildWith.IsDefined)
                {
                    state.Buffer.Append(" WITH (");

                    VisitWith(alterStatement.RebuildWith.PadIndex, "PAD_INDEX", state);
                    VisitWith(alterStatement.RebuildWith.Fillfactor, "FILLFACTOR", state);
                    VisitWith(alterStatement.RebuildWith.SortInTempdb, "SORT_IN_TEMPDB", state);
                    VisitWith(alterStatement.RebuildWith.IgnoreDupKey, "IGNORE_DUP_KEY", state);
                    VisitWith(alterStatement.RebuildWith.StatisticsNorecompute, "STATISTICS_NORECOMPUTE", state);
                    VisitWith(alterStatement.RebuildWith.DropExisting, "DROP_EXISTING", state);
                    VisitWith(alterStatement.RebuildWith.Online, "ONLINE", state);
                    VisitWith(alterStatement.RebuildWith.AllowRowLocks, "ALLOW_ROW_LOCKS", state);
                    VisitWith(alterStatement.RebuildWith.AllowPageLocks, "ALLOW_PAGE_LOCKS", state);
                    VisitWith(alterStatement.RebuildWith.MaxDegreeOfParallelism, "MAXDOP", state);

                    state.Buffer.Append(" )");
                }
            }
            else if (alterStatement.Disable)
            {
                state.Buffer.Append(" DISABLE");
            }
            else if (alterStatement.Reorganize)
            {
                state.Buffer.Append(" REORGANIZE");
            }
            else
            {
                VisitWith(alterStatement.Set.AllowRowLocks, "ALLOW_ROW_LOCKS", state);
                VisitWith(alterStatement.Set.AllowPageLocks, "ALLOW_PAGE_LOCKS", state);
                VisitWith(alterStatement.Set.IgnoreDupKey, "IGNORE_DUP_KEY", state);
                VisitWith(alterStatement.Set.StatisticsNorecompute, "STATISTICS_NORECOMPUTE", state);
            }

        }

        private static void VisitDropIndexStatement(IStatement statement, VisitorState state)
        {
            var dropIndexStatement = (DropIndexStatement)statement;

            state.Buffer.Append("DROP INDEX ");
            VisitToken(dropIndexStatement.Name, false, state);

            state.Buffer.Append(" ON ");

            VisitToken(dropIndexStatement.On, false, state);

            if (dropIndexStatement.With.IsDefined)
            {
                state.Buffer.Append(" WITH (");

                if (dropIndexStatement.With.Online.HasValue)
                {
                    state.Buffer.AppendFormat(" ONLINE = {0}", dropIndexStatement.With.Online.Value ? "ON" : "OFF");
                }
                if (dropIndexStatement.With.MaxDegreeOfParallelism.HasValue)
                {
                    state.Buffer.AppendFormat(" MAXDOP = {0}", dropIndexStatement.With.MaxDegreeOfParallelism.Value);
                }

                state.Buffer.Append(" )");
            }

        }



        #endregion Statements

        #region Select Parts
        private static void VisitJoin(List<Join> list, VisitorState state)
        {
            if (list.Count > 0)
            {
                foreach (var join in list)
                {
                    state.Buffer.Append(JoinStrings[(int)join.Type]);
                    VisitToken(join.Source, true, state);

                    if (join.On != null)
                    {
                        state.Buffer.Append(" ON ");
                        VisitToken(join.On, false, state);
                    }
                }
            }
        }

        private static void VisitOrderBy(List<Order> orderBy, VisitorState state)
        {
            if (orderBy.Count > 0)
            {
                state.Buffer.Append(" ORDER BY ");
                state.Buffer.Append(string.Join(", ", orderBy.Select(n => n.Column.FullName + (n.Direction == Direction.Asc ? " ASC" : " DESC"))));
            }
        }

        static void VisitGroupBy(List<Name> groupBy, VisitorState state)
        {
            if (groupBy.Count > 0)
            {
                state.Buffer.Append(" GROUP BY ");
                state.Buffer.Append(string.Join(", ", groupBy.Select(n => n.FullName)));
            }
        }

        static void VisitFrom(List<Token> recordsets, VisitorState state)
        {
            VisitTokenSet(" FROM ", ", ", "", recordsets, true, state);
        }
        static void VisitFrom(Token recordset, VisitorState state)
        {
            if (recordset != null)
            {
                state.Buffer.Append(" FROM ");
                VisitToken(recordset, true, state);
            }
        }

        static void VisitOutput(IEnumerable<Token> columns, Name outputInto, VisitorState state)
        {
            VisitTokenSet(" OUTPUT ", ", ", outputInto == null ? String.Empty : " INTO " + outputInto.FullName, columns, true, state);
        }

        static void VisitTop(Top top, VisitorState state)
        {
            if (top != null)
            {
                state.Buffer.Append(" TOP (");
                if (top.Value.HasValue)
                {
                    state.Buffer.Append(top.Value.Value);
                }
                else if (top.Parameters.Count > 0)
                {
                    foreach (var parameter in top.Parameters)
                    {
                        state.Parameters.Add(parameter);
                    }
                    state.Buffer.Append(top.Parameters[0].Name);
                }
                state.Buffer.Append(")");

                if (top.Percent)
                {
                    state.Buffer.Append(" PERCENT");
                }
                if (top.WithTies)
                {
                    state.Buffer.Append(" WITH TIES");
                }

            }

        }

        private static void VisitWhere(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Buffer.Append(" WHERE ");
                VisitToken(whereToken, false, state);
            }
        }

        private static void VisitHaving(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Buffer.Append(" HAVING ");
                VisitToken(whereToken, false, state);
            }
        }


        private static void VisitWith(bool? value, string name, VisitorState state)
        {
            if (value.HasValue)
            {
                state.Buffer.AppendFormat(" {0} = {1}", name, value.Value ? "ON" : "OFF");
            }

        }
        private static void VisitWith(int? value, string name, VisitorState state)
        {
            if (value.HasValue)
            {
                state.Buffer.AppendFormat(" {0} = {1}", name, value.Value);
            }

        }
        #endregion Select Parts

        #region Tokens
        static void VisitToken(Token token, bool includeAlias, VisitorState state)
        {
            TokenVisitors[token.GetType()](token, state);

            if (includeAlias && !string.IsNullOrWhiteSpace(token.Alias))
            {
                state.Buffer.Append(" AS [");
                state.Buffer.Append(token.Alias);
                state.Buffer.Append("]");
            }

            state.Parameters.AddRange(token.Parameters);
            state.ParameterValues.AddRange(token.ParameterValues);
        }
        static void VisitScalarToken(Token token, VisitorState state)
        {
            var value = ((Scalar)token).Value;

            if (value == null) return;

            if ((value is Boolean)
                || (value is SByte)
                || (value is Byte)
                || (value is Int16)
                || (value is UInt16)
                || (value is Int32)
                || (value is UInt32)
                || (value is Int64)
                || (value is UInt64)
                || (value is Single)
                || (value is Double)
                || (value is Decimal))
            {
                state.Buffer.Append(value);
            }
            else
            {
                state.Buffer.Append("N'" + value + "'");
            }
        }
        static void VisitStatementToken(Token token, VisitorState state)
        {
            var value = ((IStatement)token);

            state.Buffer.Append("(");
            VisitStatement(value, state);
            state.Buffer.Append(")");

        }
        static void VisitNameToken(Token token, VisitorState state)
        {
            var value = ((Name)token).FullName;
            state.Buffer.Append(value);
        }
        static void VisitParameterToken(Token token, VisitorState state)
        {
            var value = ((Parameter)token);
            state.Buffer.Append(value.Name);
        }
        static void VisitSnippetToken(Token token, VisitorState state)
        {
            var value = ((Snippet)token).Value;
            state.Buffer.Append(value);
        }
        static void VisitFunctionToken(Token token, VisitorState state)
        {
            var value = ((Function)token);
            state.Buffer.Append(" ");
            state.Buffer.Append(value.Name);

            VisitTokenSet("(", ", ", ")", value.Arguments, false, state);
        }
        static void VisitBinaryToken(Token token, VisitorState state, string operation)
        {
            var value = (BinaryToken)token;
            VisitToken(value.First, false, state);
            state.Buffer.Append(operation);
            VisitToken(value.Second, false, state);
        }
        static void VisitIsEqualsToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, EqualsVal);
        }
        static void VisitNotEqualToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, NotEqualVal);
        }
        static void VisitLessToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, LessVal);
        }
        static void VisitNotLessToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, NotLessVal);
        }
        static void VisitLessOrEqualToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, LessOrEqualVal);
        }
        static void VisitGreaterToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, GreaterVal);
        }
        static void VisitNotGreaterToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, NotGreaterVal);
        }
        static void VisitGreaterOrEqualToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, GreaterOrEqualVal);
        }
        static void VisitAllToken(Token token, VisitorState state)
        {
            state.Buffer.Append(" ALL ");
            var value = (AllToken)token;
            VisitToken(value.Token, false, state);
        }
        static void VisitAnyToken(Token token, VisitorState state)
        {
            state.Buffer.Append(" ANY ");
            var value = (AnyToken)token;
            VisitToken(value.Token, false, state);
        }
        static void VisitAndToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, AndVal);
        }
        static void VisitOrToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, OrVal);
        }
        static void VisitPlusToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, ((BinaryEqualToken)token).Equal ? PlusEqVal : PlusVal);
        }
        static void VisitMinusToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, ((BinaryEqualToken)token).Equal ? MinusEqVal : MinusVal);
        }
        static void VisitDivideToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, ((BinaryEqualToken)token).Equal ? DivideEqVal : DivideVal);
        }
        static void VisitModuloToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, ((BinaryEqualToken)token).Equal ? ModuloEqVal : ModuloVal);
        }
        static void VisitMultiplyToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, ((BinaryEqualToken)token).Equal ? MultiplyEqVal : MultiplyVal);
        }
        private static void VisitBitwiseAndToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, ((BinaryEqualToken)token).Equal ? BitwiseAndEqVal : BitwiseAndVal);
        }
        private static void VisitBitwiseOrToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, ((BinaryEqualToken)token).Equal ? BitwiseOrEqVal : BitwiseOrVal);
        }
        private static void VisitBitwiseXorToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, ((BinaryEqualToken)token).Equal ? BitwiseXorEqVal : BitwiseXorVal);
        }
        private static void VisitBitwiseNotToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, ((BinaryEqualToken)token).Equal ? BitwiseNotEqVal : BitwiseNotVal);
        }
        private static void VisitAssignToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, AssignVal);
        }

        static void VisitLikeToken(Token token, VisitorState state)
        {
            var value = (BinaryToken)token;
            VisitToken(value.First, false, state);
            state.Buffer.Append(" LIKE ");
            VisitToken(value.Second, false, state);
        }
        static void VisitContainsToken(Token token, VisitorState state)
        {
            var value = (BinaryToken)token;
            VisitToken(value.First, false, state);
            state.Buffer.Append(" LIKE '%' + ");
            VisitToken(value.Second, false, state);
            state.Buffer.Append(" + '%'");
        }
        static void VisitStartsWithToken(Token token, VisitorState state)
        {
            var value = (BinaryToken)token;
            VisitToken(value.First, false, state);
            state.Buffer.Append(" LIKE ");
            VisitToken(value.Second, false, state);
            state.Buffer.Append(" + '%'");
        }
        static void VisitEndsWithToken(Token token, VisitorState state)
        {
            var value = (BinaryToken)token;
            VisitToken(value.First, false, state);
            state.Buffer.Append(" LIKE '%' + ");
            VisitToken(value.Second, false, state);
        }

        static void VisitGroupToken(Token token, VisitorState state)
        {
            state.Buffer.Append(" (");
            var value = (GroupToken)token;
            VisitToken(value.Token, false, state);
            state.Buffer.Append(" )");
        }

        static void VisitExistsToken(Token token, VisitorState state)
        {
            state.Buffer.Append(" EXISTS ");
            var value = (ExistsToken)token;
            VisitToken(value.Token, false, state);
        }
        static void VisitNotToken(Token token, VisitorState state)
        {
            state.Buffer.Append(" NOT");
            var value = (NotToken)token;
            VisitToken(value.Token, false, state);
        }
        static void VisitIsNullToken(Token token, VisitorState state)
        {
            var value = (IsNullToken)token;
            VisitToken(value.Token, false, state);
            state.Buffer.Append(" IS NULL");
        }
        static void VisitIsNotNullToken(Token token, VisitorState state)
        {
            var value = (IsNotNullToken)token;
            VisitToken(value.Token, false, state);
            state.Buffer.Append(" IS NOT NULL");
        }
        static void VisitBetweenToken(Token token, VisitorState state)
        {
            var value = (BetweenToken)token;

            VisitToken(value.Token, false, state);
            state.Buffer.Append(" BETWEEN");
            VisitToken(value.First, false, state);
            state.Buffer.Append(" AND");
            VisitToken(value.Second, false, state);
        }
        static void VisitInToken(Token token, VisitorState state)
        {
            var value = (InToken)token;

            VisitToken(value.Token, false, state);
            VisitTokenSet(" IN (", ", ", ")", value.Set, false, state);
        }
        static void VisitNotInToken(Token token, VisitorState state)
        {
            var value = (NotInToken)token;

            VisitToken(value.Token, false, state);
            VisitTokenSet(" NOT IN (", ", ", ")", value.Set, false, state);
        }

        private static void VisitTokenSet(string prefix, string separator, string suffix, IEnumerable<Token> tokens,
            bool includeAlias, VisitorState state)
        {
            var enumerator = tokens.GetEnumerator();
            if (enumerator.MoveNext())
            {
                state.Buffer.Append(prefix);
                VisitToken(enumerator.Current, includeAlias, state);

                while (enumerator.MoveNext())
                {
                    state.Buffer.Append(separator);
                    VisitToken(enumerator.Current, includeAlias, state);
                }
                state.Buffer.Append(suffix);
            }
        }

        private static void VisitCommentToken(Token token, VisitorState state)
        {
            var commentToken = (CommentToken)token;

            state.Buffer.Append(" /* ");

            VisitToken(commentToken.Content, false, state);

            state.Buffer.Append(" */ ");
        }


        private static void VisitStringifyToken(Token token, VisitorState state)
        {
            var strToken = (StringifyToken)token;

            state.Buffer.Append("N'");

            var startIndex = state.Buffer.Length;
            VisitToken(strToken.Content, false, state);
            var endIndex = state.Buffer.Length;

            // replace all "'" characters with "''"
            if (endIndex > startIndex)
            {
                state.Buffer.Replace("'", "''", startIndex, endIndex - startIndex);
            }

            state.Buffer.Append("'");
        }


        #endregion Tokens
    }
}
