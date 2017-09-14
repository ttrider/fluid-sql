// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
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