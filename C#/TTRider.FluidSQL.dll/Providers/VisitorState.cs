// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace TTRider.FluidSql.Providers
{
    public class VisitorState
    {
        readonly StringBuilder buffer;
        string stringifyPrefix;
        string stringifySuffix;

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
            if (string.IsNullOrWhiteSpace(prefix)) throw new ArgumentNullException(nameof(prefix));
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

            AddToTheEnd(value, values);
        }

        public void AddToTheEnd(string value, params string[] values)
        {
            if (value == null) return;

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
        ///     Ensures that statement has proper terminating character (;)
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
    }
}