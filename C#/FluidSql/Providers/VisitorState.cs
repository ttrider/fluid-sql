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
            this.ParameterValues = new List<ParameterValue>();
            this.Buffer = new StringBuilder();
        }

        public StringBuilder Buffer { get; private set; }

        public List<Parameter> Parameters { get; private set; }
        public List<Parameter> Variables { get; private set; }
        public List<ParameterValue> ParameterValues { get; private set; }

        public string Value
        {
            get { return this.Buffer.ToString(); }
        }

        public void AppendLine()
        {
            this.Buffer.AppendLine();
        }
        public void Append(string value)
        {
            this.Buffer.Append(value);
        }
        public void Append(int value)
        {
            this.Buffer.Append(value);
        }
        public void Append(object value)
        {
            this.Buffer.Append(value);
        }

        public IEnumerable<SqlParameter> GetDbParameters()
        {
            // we have a list of parameters, some of them are duplicates
            // some contain type, some not
            // we need to get a final list, preferable with types

            return this.Parameters
                .GroupBy(p => p.Name)
                .Select(pg => pg.FirstOrDefault(p => p.DbType.HasValue) ?? pg.First())
                .Except(this.Variables, ParameterEqualityComparer.Default)
                .Select(p =>
                {
                    var sp = new SqlParameter
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

                    var value = this.ParameterValues.FirstOrDefault(pp => string.Equals(pp.Name, p.Name));
                    if (value != null && value.Value != null)
                    {
                        sp.Value = value.Value;
                    }
                    else if (p.DefaultValue != null)
                    {
                        sp.Value = p.DefaultValue;
                    }
                    return sp;
                });
        }
    }
}