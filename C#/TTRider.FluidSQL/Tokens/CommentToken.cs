// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class CommentToken : Token
    {
        public Token Content { get; set; }

        public string Alias { get; set; }
    }
}