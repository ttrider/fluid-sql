// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class InsertStatement : RecordsetStatement
        , ITopStatement
        , IIntoStatement
        , ICTEStatement
        , IOnConflict
        , ISelectStatement
    {
        public InsertStatement()
        {
            this.Columns = new List<Name>();
            this.Values = new List<Token[]>();
            this.CommonTableExpressions = new List<CTEDefinition>();
        }

        public Name Into { get; set; }
        public RecordsetStatement From { get; set; }
        public bool DefaultValues { get; set; }
        public List<Name> Columns { get; private set; }
        public List<Token[]> Values { get; private set; }
        public bool IdentityInsert { get; set; }
        public List<CTEDefinition> CommonTableExpressions { get; set; }

        public OnConflict? Conflict { get; set; }
        public Top Top { get; set; }
    }
}