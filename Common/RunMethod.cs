using System;
using System.Reflection;

namespace Lextalionis.LambdaTools.Common
{
    public class RunMethod
    {
        public MethodInfo Method { get; set; }
        public object[] Arguments { get; set; }
        public Type Return { get; set; }
    }
}
