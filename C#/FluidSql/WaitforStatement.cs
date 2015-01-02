// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014, 2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql
{
    public abstract class WaitforStatement : Token, IStatement
    {
    }

    public class WaitforDelayStatement : WaitforStatement
    {
        public TimeSpan Delay { get; set; }
    }
    public class WaitforTimeStatement : WaitforStatement
    {
        public DateTime Time { get; set; }
    }
    
}
