// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class DateAddFunctionToken : DateFunctionExpressionToken
    {
        public DatePart DatePart { get; set; }

        public Token Number { get; set; }

        public Token Token { get; set; }

        public bool Subtract { get; set; }
    }
}