// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TTRider.FluidSql.Providers
{
    public abstract partial class Visitor
    {
        protected abstract void VisitNowFunctionToken(NowFunctionToken token);
        protected abstract void VisitUuidFunctionToken(UuidFunctionToken token);
        protected abstract void VisitIIFFunctionToken(IifFunctionToken token);
        protected abstract void VisitDatePartFunctionToken(DatePartFunctionToken token);
        protected abstract void VisitDurationFunctionToken(DurationFunctionToken token);
    }
}