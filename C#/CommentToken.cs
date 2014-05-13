// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class CommentToken : Token
    {
        public Token Content { get; set; }
    }
    public class CommentStatement : IStatement
    {
        public IStatement Content { get; set; }
    }
}