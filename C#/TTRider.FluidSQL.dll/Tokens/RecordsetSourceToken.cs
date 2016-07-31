// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class RecordsetSourceToken : Token, IAliasToken
    {
        private string alias;

        public Token Source { get; set; } //it can be one of Name,RecordsetStatement,Variable,Parameter,Snippet

        public string Alias
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.alias))
                {
                    return alias;
                }
                if (this.Source is IAliasToken)
                {
                    var a = ((IAliasToken) this.Source).Alias;
                    if (!string.IsNullOrWhiteSpace(a))
                    {
                        return a;
                    }
                }
                return null;
            }
            set { this.alias = value; }
        }


        public static implicit operator RecordsetSourceToken(Name source)
        {
            return new RecordsetSourceToken { Source = source };
        }

        public static implicit operator RecordsetSourceToken(Parameter source)
        {
            return new RecordsetSourceToken { Source = source };
        }

        public static implicit operator RecordsetSourceToken(RecordsetStatement source)
        {
            return new RecordsetSourceToken { Source = source };
        }

        public static implicit operator RecordsetSourceToken(Snippet source)
        {
            return new RecordsetSourceToken { Source = source };
        }

        public static implicit operator RecordsetSourceToken(SnippetStatement source)
        {
            return new RecordsetSourceToken { Source = source };
        }
    }
}