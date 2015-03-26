// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class WhenMatchedTokenThenUpdateSetToken : WhenMatchedToken
    {
        public WhenMatchedTokenThenUpdateSetToken()
        {
            this.Set = new List<BinaryEqualToken>();
        }
        public IList<BinaryEqualToken> Set { get; private set; }
    }
}