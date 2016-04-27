// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System;

namespace TTRider.FluidSql.Providers.Sqlite
{
    internal partial class SqliteVisitor
    {
        protected override void VisitNowFunctionToken(NowFunctionToken token)
        {
            if (token.Utc)
            {
                State.Write(Symbols.DATETIME);
                State.Write(Symbols.OpenParenthesis);
                State.Write(SqliteSymbols.current_timestamp);
                State.Write(Symbols.CloseParenthesis);
            }
            else
            {
                State.Write(Symbols.DATETIME);
                State.Write(Symbols.OpenParenthesis);
                State.Write(SqliteSymbols.current_timestamp);
                State.Write(Symbols.Comma);
                State.Write(Symbols.SingleQuote);
                State.Write(SqliteSymbols.localtime);
                State.Write(Symbols.SingleQuote);
                State.Write(Symbols.CloseParenthesis);
            }
        }

        protected override void VisitUuidFunctionToken(UuidFunctionToken token)
        {
            State.Write(SqliteSymbols.hex);
            State.Write(Symbols.OpenParenthesis);
            State.Write(SqliteSymbols.randomblob);
            State.Write(Symbols.OpenParenthesis);
            State.Write("16");
            State.Write(Symbols.CloseParenthesis);
            State.Write(Symbols.CloseParenthesis);
        }


        protected override void VisitIIFFunctionToken(IifFunctionToken token)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDatePartFunctionToken(DatePartFunctionToken token)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDurationFunctionToken(DurationFunctionToken token)
        {
            throw new NotImplementedException();
        }

        protected override void VisitMakeDateFunctionToken(MakeDateFunctionToken token)
        {
            throw new NotImplementedException();
        }

        protected override void VisitMakeTimeFunctionToken(MakeTimeFunctionToken token)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDateAddFunctionToken(DateAddFunctionToken token)
        {
            throw new NotImplementedException();
        }
    }
}