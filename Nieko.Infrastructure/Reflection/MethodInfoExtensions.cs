using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace System.Reflection
{
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Creates a delegating executing the method indicated by <paramref name="methodInfo"/> using 
        /// parameters <paramref name="parameters"/>
        /// </summary>
        /// <param name="methodInfo">Current instance</param>
        /// <param name="parameters">Method parameters</param>
        /// <returns>Function delegate</returns>
        public static Func<object, object> ToLambdaInvocation(this MethodInfo methodInfo, params object[] parameters)
        {
            var objectParameter = Expression.Parameter(typeof(object), "obj");
            var callParameters = new List<ConstantExpression>();
            var methodParameters = methodInfo.GetParameters();

            for (int i = 0; i < methodParameters.Length; i++)
            {
                callParameters.Add(Expression.Constant(parameters[i], methodParameters[i].ParameterType));
            }

            var invocation = Expression.Lambda<Func<object, object>>(
                Expression.Call(
                    Expression.Convert(objectParameter, methodInfo.DeclaringType),
                    methodInfo,
                    callParameters.ToArray()),
                objectParameter)
            .Compile();

            return invocation;
        }
    }
}
