// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

// ReSharper disable InconsistentNaming

using System.Linq;

namespace TTRider.FluidSql.Providers
{
    public abstract partial class Visitor
    {
        protected abstract void VisitNowFunctionToken(NowFunctionToken token);
        protected abstract void VisitUuidFunctionToken(UuidFunctionToken token);
        protected abstract void VisitIIFFunctionToken(IifFunctionToken token);
        protected abstract void VisitDatePartFunctionToken(DatePartFunctionToken token);
        protected abstract void VisitDurationFunctionToken(DurationFunctionToken token);
        protected abstract void VisitMakeDateFunctionToken(MakeDateFunctionToken token);
        protected abstract void VisitMakeTimeFunctionToken(MakeTimeFunctionToken token);
        protected virtual void VisitCoalesceFunctionToken(CoalesceFunctionToken token)
        {
            State.Write(Symbols.COALESCE);
            State.Write(Symbols.OpenParenthesis);
            VisitTokenSet(token.Arguments);
            State.Write(Symbols.CloseParenthesis);
        }

        protected virtual void VisitNullIfFunctionToken(NullIfFunctionToken token)
        {
            State.Write(Symbols.NULLIF);
            State.Write(Symbols.OpenParenthesis);
            VisitTokenSet(token.Arguments.Take(2));
            State.Write(Symbols.CloseParenthesis);
        }
    }
}