using System;
using System.Globalization;

namespace TTRider.FluidSql.Providers
{
    public abstract partial class Visitor
    {
        protected virtual void FormatScalarValue(DBNull value)
        {
            State.Write(Symbols.NULL);
        }

        protected virtual void FormatScalarValue(bool value)
        {
            State.Write(value ? Symbols.TRUE : Symbols.FALSE);
        }

        protected virtual void FormatScalarValue(string value)
        {
            var stringValue = value.Replace(this.LiteralCloseQuote,
                    this.LiteralCloseQuote + this.LiteralCloseQuote);

            if (!stringValue.StartsWith("$") && !stringValue.StartsWith("?"))
            {
                State.Write(this.LiteralOpenQuote, stringValue, this.LiteralCloseQuote);
            }
            else
            {
                State.Write(stringValue);
            }
        }

        protected virtual void FormatScalarValue(DateTimeOffset value)
        {
            State.Write(this.LiteralOpenQuote, value.ToString("yyyy-MM-ddTHH:mm:ss"), this.LiteralCloseQuote);
        }

        protected virtual void FormatScalarValue(DateTime value)
        {
            State.Write(this.LiteralOpenQuote, value.ToString("yyyy-MM-ddTHH:mm:ss"), this.LiteralCloseQuote);
        }

        protected virtual void FormatScalarValue(TimeSpan value)
        {
            State.Write(this.LiteralOpenQuote, value.ToString("HH:mm:ss"), this.LiteralCloseQuote);
        }

        protected virtual void FormatScalarValue(decimal value)
        {
            State.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual void FormatScalarValue(double value)
        {
            State.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual void FormatScalarValue(float value)
        {
            State.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual void FormatScalarValue(ulong value)
        {
            State.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual void FormatScalarValue(long value)
        {
            State.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual void FormatScalarValue(uint value)
        {
            State.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual void FormatScalarValue(int value)
        {
            State.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual void FormatScalarValue(ushort value)
        {
            State.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual void FormatScalarValue(short value)
        {
            State.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual void FormatScalarValue(byte value)
        {
            State.Write(value.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual void FormatScalarValue(sbyte value)
        {
            State.Write(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
