using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql
{
    public class MergeStatement : RecordsetStatement
                                , ITopStatement
                                , IIntoStatement
                                
    {
        public MergeStatement()
        {
            this.WhenMatched = new List<WhenMatched>();
            this.WhenNotMatched = new List<WhenMatched>();
            this.WhenNotMatchedBySource = new List<WhenMatched>();
        }


        public Top Top { get; set; }
        public Name Into { get; set; }
        public RecordsetStatement Using { get; set; }
        public Token On { get; set; }

        public List<WhenMatched> WhenMatched { get; private set; }
        public List<WhenMatched> WhenNotMatched { get; private set; }
        public List<WhenMatched> WhenNotMatchedBySource { get; private set; }
    }

    public class WhenMatched : Token
    {
        public WhenMatched()
        {
        }

        public Token AndCondition { get; set; }


        
    }

    public class WhenMatchedThenDelete : WhenMatched
    {
    }
    public class WhenMatchedThenUpdateSet : WhenMatched
    {
        public WhenMatchedThenUpdateSet()
        {
            this.Set = new List<BinaryEqualToken>();
        }
        public IList<BinaryEqualToken> Set { get; private set; }
    }
    public class WhenNotMatchedThenInsert : WhenMatched
    {
        public WhenNotMatchedThenInsert()
        {
            this.Columns = new List<Name>();
            this.Values = new List<Token>();
        }

        public List<Name> Columns { get; private set; }

        public List<Token> Values { get; private set; }
    }
}
