using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TTRider.FluidSql.Providers
{
    public class VisitorState
    {
        public VisitorState()
        {
            this.Parameters = new List<Parameter>();
            this.Variables = new List<Parameter>();
            this.Buffer = new StringBuilder();
        }

        public StringBuilder Buffer { get; private set; }

        public List<Parameter> Parameters { get; private set; }
        public List<Parameter> Variables { get; private set; } 
        public string Value 
        {
            get { return this.Buffer.ToString(); }
        }
       

        internal void AppendParameters(IEnumerable<Parameter> list)
        {
            this.Parameters.AddRange(list);
        }

        internal IEnumerable<SqlParameter> GetDbParameters()
        {
            // we have a list of parameters, some of them are duplicates
            // some contain type, some not
            // we need to get a final list, preferable with types

            return this.Parameters
                .GroupBy(p => p.Name)
                .Select(pg => pg.FirstOrDefault(p => p.DbType.HasValue) ?? pg.First())
                .Select(p =>
                {
                    var sp = new SqlParameter()
                    {
                        ParameterName = p.Name,
                    };
                    if (p.DbType.HasValue)
                    {
                        sp.SqlDbType = p.DbType.Value;
                    }

                    if (p.DbType.HasValue)
                    {
                        sp.SqlDbType = p.DbType.Value;
                    }

                    if (p.Length.HasValue)
                    {
                        sp.Size = p.Length.Value;
                    }

                    if (p.Precision.HasValue)
                    {
                        sp.Precision = p.Precision.Value;
                    }

                    if (p.Scale.HasValue)
                    {
                        sp.Scale = p.Scale.Value;
                    }

                    return sp;
                });

        }
    }
}
