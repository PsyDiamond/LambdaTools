using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lextalionis.LambdaTools.Common;

namespace Lextalionis.LambdaTools
{
    /// <summary>
    /// Работа с лямба выражениями конкретного класса
    /// </summary>
    /// <typeparam name="T">класс</typeparam>
    public static class ClassOf<T>
    {
        /// <summary>
        /// Получить метод
        /// </summary>
        /// <typeparam name="TR">тип возвращаемого значения</typeparam>
        /// <param name="expression">выражение</param>
        /// <returns>метод</returns>
        public static MethodInfo GetMethod<TR>(Expression<Func<T, TR>> expression)
        {
            return GetMethodCore(expression);
        }
        /// <summary>
        /// Получить метод
        /// </summary>
        /// <param name="expression">выражение</param>
        /// <returns>метод</returns>
        private static MethodInfo GetMethod(Expression<Action<T>> expression)
        {
            return GetMethodCore(expression);
        }

        /// <summary>
        /// Получить метод (ядро)
        /// </summary>
        /// <param name="expression">выражение</param>
        /// <returns>метод</returns>
        private static MethodInfo GetMethodCore(LambdaExpression expression)
        {
            var method = (MethodCallExpression)expression.Body;
            return method.Method;
        }
        /// <summary>
        /// Получить аргументы выражения
        /// </summary>
        /// <typeparam name="TR">тип возвращаемого значения</typeparam>
        /// <param name="expression">выражение</param>
        /// <returns>аргументы</returns>
        public static object[] GetArguments<TR>(Expression<Func<T, TR>> expression)
        {
            return GetArgumentsCore(expression);
        }
        /// <summary>
        /// Получить аргументы выражения
        /// </summary>
        /// <param name="expression">выражение</param>
        /// <returns>аргументы</returns>
        public static object[] GetArguments(Expression<Action<T>> expression)
        {
            return GetArgumentsCore(expression);
        }
        /// <summary>
        /// Получить аргументы выражения (ядро)
        /// </summary>
        /// <param name="expression">выражение</param>
        /// <returns>аргументы</returns>
        private static object[] GetArgumentsCore(LambdaExpression expression)
        {

            var method = (MethodCallExpression)expression.Body;
            return GetArgumentsCore(method);
        }
        /// <summary>
        /// Получить аргументы выражения (ядро)
        /// </summary>
        /// <param name="expression">выражение</param>
        /// <returns>аргументы</returns>
        private static object[] GetArgumentsCore(MethodCallExpression expression)
        {
            var list = new List<object>();
            list.AddRange(expression.Arguments.Select(arg => GetArgument(arg)));

            return list.ToArray();
        }
        /// <summary>
        /// Получить 1 аргумент
        /// </summary>
        /// <param name="arg">выражение</param>
        /// <returns>аргумент</returns>
        private static object GetArgument(Expression arg)
        {
            if (arg == null)
                return null;

            object value;

            switch (arg.NodeType)
            {
                case ExpressionType.Constant:
                    value = GetConstant(arg);
                    break;
                case ExpressionType.MemberAccess:
                    value = GetMember(arg);
                    break;
                case ExpressionType.Call:
                    value = GetMethodResult(arg);
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    value = GetConvert(arg);
                    break;
                default:
                    throw new Exception(string.Format("{0} не предусмотрен", arg.NodeType));
            }
            return value;
        }
        /// <summary>
        /// Получить аругмент при использовании Конвертации
        /// </summary>
        /// <param name="expression">выражение</param>
        /// <returns>аргумент</returns>
        private static object GetConvert(Expression expression)
        {
            var value = (UnaryExpression) expression;
            return GetMember(value.Operand);
        }
        /// <summary>
        /// Получить результат выполнение метода
        /// </summary>
        /// <param name="expression">выражение</param>
        /// <returns>аргумент</returns>
        private static object GetMethodResult(Expression expression)
        {
            var value = (MethodCallExpression)expression;
            var obj = GetArgument(value.Object);
            return value.Method.Invoke(obj, GetArgumentsCore(value));
        }
        /// <summary>
        /// Получить член класса
        /// </summary>
        /// <param name="expression">выражение</param>
        /// <returns>аргумент</returns>
        private static object GetMember(Expression expression)
        {
            object result;
            var member = (MemberExpression)expression;
            var nodeType = member.Expression.NodeType;
            switch (nodeType)
            {
                case ExpressionType.Constant:
                    var constant = GetConstant(member.Expression);
                    var field = (FieldInfo)member.Member;
                    result = field.GetValue(constant);
                    break;
                case ExpressionType.MemberAccess:
                    result = GetMember(member.Expression);
                    break;
                default:
                    throw new ArgumentException("Хрень какая то! " + nodeType);
            }
            return result;
        }
        /// <summary>
        /// Получить константу
        /// </summary>
        /// <param name="expression">выражение</param>
        /// <returns>аргумент</returns>
        private static object GetConstant(Expression expression)
        {
            var constant = (ConstantExpression)expression;
            if (constant != null)
                return constant.Value;

            throw new ArgumentException("Не константа");
        }

        /// <summary>
        /// Подготовиться к запуску метода
        /// </summary>
        /// <typeparam name="TR">тип возвращаемого значения</typeparam>
        /// <param name="expression">выражение</param>
        /// <returns>информация для запуска метода</returns>
        public static RunMethod RunMethod<TR>(Expression<Func<T, TR>> expression)
        {
            var result = RunMethodCore(expression);
            result.Return = typeof(TR);
            return result;
        }

        /// <summary>
        /// Подготовиться к запуску метода
        /// </summary>
        /// <param name="expression">выражение</param>
        /// <returns>информация для запуска метода</returns>
        public static RunMethod RunMethod(Expression<Action<T>> expression)
        {
            return RunMethodCore(expression);
        }
        /// <summary>
        /// Подготовиться к запуску метода (ядро)
        /// </summary>
        /// <param name="expression">выражение</param>
        /// <returns>информация для запуска метода</returns>
        private static RunMethod RunMethodCore(LambdaExpression expression)
        {
            return new RunMethod
            {
                Method = GetMethodCore(expression),
                Arguments = GetArgumentsCore(expression)
            };
        }
    }
}
