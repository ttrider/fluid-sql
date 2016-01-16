using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers.PostgreSQL
{
    internal partial class PostgreSQLVisitor
    {
        protected override void VisitScalarToken(Scalar token) { throw new NotImplementedException(); }
        protected override void VisitNameToken(Name token) { throw new NotImplementedException(); }
        protected override void VisitParameterToken(Parameter token) { throw new NotImplementedException(); }
        protected override void VisitSnippetToken(Snippet token) { throw new NotImplementedException(); }
        protected override void VisitFunctionToken(Function token) { throw new NotImplementedException(); }
        protected override void VisitBitwiseNotToken(BitwiseNotToken token) { throw new NotImplementedException(); }
        protected override void VisitGroupToken(GroupToken token) { throw new NotImplementedException(); }
        protected override void VisitUnaryMinusToken(UnaryMinusToken token) { throw new NotImplementedException(); }
        protected override void VisitNotToken(NotToken token) { throw new NotImplementedException(); }
        protected override void VisitIsNullToken(IsNullToken token) { throw new NotImplementedException(); }
        protected override void VisitIsNotNullToken(IsNotNullToken token) { throw new NotImplementedException(); }
        protected override void VisitExistsToken(ExistsToken token) { throw new NotImplementedException(); }
        protected override void VisitAllToken(AllToken token) { throw new NotImplementedException(); }
        protected override void VisitAnyToken(AnyToken token) { throw new NotImplementedException(); }
        protected override void VisitBetweenToken(BetweenToken token) { throw new NotImplementedException(); }
        protected override void VisitInToken(InToken token) { throw new NotImplementedException(); }
        protected override void VisitNotInToken(NotInToken token) { throw new NotImplementedException(); }
        protected override void VisitCommentToken(CommentToken token) { throw new NotImplementedException(); }
        protected override void VisitStringifyToken(StringifyToken token) { throw new NotImplementedException(); }
        protected override void VisitWhenMatchedThenDelete(WhenMatchedTokenThenDeleteToken token) { throw new NotImplementedException(); }
        protected override void VisitWhenMatchedThenUpdateSet(WhenMatchedTokenThenUpdateSetToken token) { throw new NotImplementedException(); }
        protected override void VisitWhenNotMatchedThenInsert(WhenNotMatchedTokenThenInsertToken token) { throw new NotImplementedException(); }
        protected override void VisitOrderToken(Order token) { throw new NotImplementedException(); }
        protected override void VisitCommonTableExpression(CTEDefinition token) { throw new NotImplementedException(); }
        protected override void VisitFromToken(RecordsetSourceToken token) { throw new NotImplementedException(); }
        protected override void VisitCaseToken(CaseToken token) { throw new NotImplementedException(); }

    }
}
