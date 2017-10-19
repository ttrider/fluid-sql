using System.Text;
using System.Linq;

namespace TTRider.FluidSql.Providers.Redshift
{
    public partial class RedshiftVisitor
    {
        private string uuidGeneration = "md5(random()::text || now()::text)";

        protected override void VisitNowFunctionToken(NowFunctionToken token)
        {
            State.Write(Symbols.GETDATE);
            State.Write(Symbols.OpenParenthesis);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitUuidFunctionToken(UuidFunctionToken token)
        {
            State.Write(this.uuidGeneration);
        }


        protected override void VisitDateAddFunctionToken(DateAddFunctionToken token)
        {
            State.Write(Symbols.DATEADD);
            State.Write(Symbols.OpenParenthesis);

            switch (token.DatePart)
            {
                case DatePart.Day:
                    State.Write(PostgrSQLSymbols.d);
                    break;
                case DatePart.Year:
                    State.Write(PostgrSQLSymbols.yy);
                    break;
                case DatePart.Month:
                    State.Write(PostgrSQLSymbols.m);
                    break;
                case DatePart.Week:
                    State.Write(PostgrSQLSymbols.ww);
                    break;
                case DatePart.Hour:
                    State.Write(PostgrSQLSymbols.hh);
                    break;
                case DatePart.Minute:
                    State.Write(PostgrSQLSymbols.mi);
                    break;
                case DatePart.Second:
                    State.Write(PostgrSQLSymbols.ss);
                    break;
                case DatePart.Millisecond:
                    State.Write(PostgrSQLSymbols.ms);
                    break;
            }
            State.Write(Symbols.Comma);
            VisitToken(token.Subtract ? new UnaryMinusToken { Token = token.Number } : token.Number);
            State.Write(Symbols.Comma);
            VisitToken(token.Token);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitDurationFunctionToken(DurationFunctionToken token)
        {
            State.Write(Symbols.DATEDIFF);
            State.Write(Symbols.OpenParenthesis);

            switch (token.DatePart)
            {
                case DatePart.Day:
                    State.Write(PostgrSQLSymbols.d);
                    break;
                case DatePart.Year:
                    State.Write(PostgrSQLSymbols.yy);
                    break;
                case DatePart.Month:
                    State.Write(PostgrSQLSymbols.m);
                    break;
                case DatePart.Week:
                    State.Write(PostgrSQLSymbols.ww);
                    break;
                case DatePart.Hour:
                    State.Write(PostgrSQLSymbols.hh);
                    break;
                case DatePart.Minute:
                    State.Write(PostgrSQLSymbols.mi);
                    break;
                case DatePart.Second:
                    State.Write(PostgrSQLSymbols.ss);
                    break;
                case DatePart.Millisecond:
                    State.Write(PostgrSQLSymbols.ms);
                    break;
            }
            State.Write(Symbols.Comma);
            VisitToken(token.Start);
            State.Write(Symbols.Comma);
            VisitToken(token.End);
            State.Write(Symbols.CloseParenthesis);
        }
    }
}
