// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class MergeStatement : RecordsetStatement
        , ITopStatement
        , IIntoStatement
        , ICTEStatement

    {
        public MergeStatement()
        {
            this.WhenMatched = new List<WhenMatchedToken>();
            this.WhenNotMatched = new List<WhenMatchedToken>();
            this.WhenNotMatchedBySource = new List<WhenMatchedToken>();
            this.CommonTableExpressions = new List<CTEDefinition>();
        }

        public Token Using { get; set; }
        public Token On { get; set; }

        public List<WhenMatchedToken> WhenMatched { get; private set; }
        public List<WhenMatchedToken> WhenNotMatched { get; private set; }
        public List<WhenMatchedToken> WhenNotMatchedBySource { get; private set; }
        public List<CTEDefinition> CommonTableExpressions { get; set; }
        public Name Into { get; set; }
        public Top Top { get; set; }
    }
}