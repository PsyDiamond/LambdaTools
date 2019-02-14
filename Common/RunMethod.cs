using System;
using System.Reflection;

namespace Lextalionis.LambdaTools.Common
{
    /// <summary>
    /// Класс для выполнения метода (с аргументами)
    /// </summary>
    public class RunMethod
    {
        /// <summary>
        /// Метод
        /// </summary>
        public MethodInfo Method { get; set; }
        /// <summary>
        /// Аргументы
        /// </summary>
        public object[] Arguments { get; set; }
        /// <summary>
        /// Тип возвращаемого значения
        /// </summary>
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public Type Return { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global
    }
}
