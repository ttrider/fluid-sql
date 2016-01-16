using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers.MySql
{
    internal partial class MySqlVisitor
    {
        protected override void VisitNowFunctionToken(NowFunctionToken token) { throw new NotImplementedException(); }
        protected override void VisitUuidFunctionToken(UuidFunctionToken token) { throw new NotImplementedException(); }
        protected override void VisitIIFFunctionToken(IifFunctionToken token) { throw new NotImplementedException(); }
        protected override void VisitDatePartFunctionToken(DatePartFunctionToken token) { throw new NotImplementedException(); }
        protected override void VisitDateAddFunctionToken(DateAddFunctionToken token) { throw new NotImplementedException(); }
        protected override void VisitDurationFunctionToken(DurationFunctionToken token) { throw new NotImplementedException(); }
        protected override void VisitMakeDateFunctionToken(MakeDateFunctionToken token) { throw new NotImplementedException(); }
        protected override void VisitMakeTimeFunctionToken(MakeTimeFunctionToken token) { throw new NotImplementedException(); }
        protected override void VisitCoalesceFunctionToken(CoalesceFunctionToken token) { throw new NotImplementedException(); }
        protected override void VisitNullIfFunctionToken(NullIfFunctionToken token) { throw new NotImplementedException(); }

    }
}
