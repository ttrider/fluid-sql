using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TTRider.FluidSql.Providers
{
    public class VisitorState
    {
        string stringifyPrefix;
        string stringifySuffix;
        StringBuilder buffer;

        public VisitorState()
        {
            this.Parameters = new List<Parameter>();
            this.Variables = new List<Parameter>();
            this.ParameterValues = new List<ParameterValue>();
            this.buffer = new StringBuilder();
        }



        public List<Parameter> Parameters { get; private set; }
        public List<Parameter> Variables { get; private set; }
        public List<ParameterValue> ParameterValues { get; private set; }

        public string Value
        {
            get
            {
                // ignore trailing crlf
                var length = this.buffer.Length;
                if (length > 0 && this.buffer[length - 1] == '\n')
                {
                    if (length > 1 && this.buffer[length - 2] == '\r')
                    {
                        return this.buffer.ToString(0, length - 2);
                    }
                    return this.buffer.ToString(0, length - 1);
                }
                return this.buffer.ToString();
            }
        }

        public void WriteBeginStringify(string prefix, string suffix = null)
        {
            if (string.IsNullOrWhiteSpace(prefix)) throw new ArgumentNullException("prefix");
            if (string.IsNullOrWhiteSpace(suffix)) suffix = prefix;

            if (string.IsNullOrWhiteSpace(this.stringifyPrefix) || string.IsNullOrWhiteSpace(this.stringifySuffix))
            {
                this.buffer.Append(prefix);
                this.stringifyPrefix = prefix;
                this.stringifySuffix = suffix;
            }
        }
        public void WriteEndStringify()
        {
            if (string.IsNullOrWhiteSpace(this.stringifyPrefix) || string.IsNullOrWhiteSpace(this.stringifySuffix))
            {
                return;
            }
            var suffix = this.stringifySuffix;
            this.stringifySuffix = null;
            this.stringifyPrefix = null;
            this.buffer.Append(suffix);
        }


        public void Write(string value, params string[] values)
        {
            if (value == null) return;

            // special threatment of COMMA and CRLF
            if (buffer.Length != 0 && value != "," && value != "\r\n" && buffer[buffer.Length - 1] != '\n')
            {
                this.buffer.Append(" ");
            }

            if (!string.IsNullOrWhiteSpace(this.stringifySuffix))
            {
                this.buffer.Append(value.Replace(this.stringifySuffix, this.stringifySuffix + this.stringifySuffix));
                foreach (var valItem in values)
                {
                    this.buffer.Append(valItem.Replace(this.stringifySuffix, this.stringifySuffix + this.stringifySuffix));
                }
            }
            else
            {
                this.buffer.Append(value);
                foreach (var valItem in values)
                {
                    this.buffer.Append(valItem);
                }
            }
        }


        /// <summary>
        /// Ensures that statement has proper terminating character (;)  
        /// </summary>
        public void WriteStatementTerminator(bool addCRLF = true)
        {
            // we need to make sure that the last non-whitespace character
            // is ';' unless it is */ or :
            //TODO: need a better code here
            for (var i = this.buffer.Length - 1; i >= 0; i--)
            {
                var ch = this.buffer[i];
                if (!Char.IsWhiteSpace(ch))
                {
                    if (ch == ';')
                    {
                        break;
                    }
                    if (ch == ':')
                    {
                        break;
                    }
                    if (ch == '/')
                    {
                        if (i > 0 && this.buffer[i - 1] == '*')
                        {
                            break;
                        }
                    }

                    this.buffer.Append(";");
                    break;
                }
            }

            if (addCRLF && this.buffer.Length > 0 && this.buffer[this.buffer.Length - 1] != '\n')
            {
                this.buffer.Append("\r\n");
            }
        }

        public void WriteCRLF(bool ifNotExists = false)
        {
            if (ifNotExists)
            {
                var last = this.buffer.Length - 1;
                if (last >= 0 && this.buffer[last] != '\n')
                {
                    this.buffer.Append("\r\n");
                }
            }
            else
            {
                this.buffer.Append("\r\n");
            }
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