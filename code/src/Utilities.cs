using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Guardian
{
    public static class Utilities
    { /// <summary>
      /// Derives a method, property, or field's name from a Linq expression when an instance of the object is not available
      /// </summary>
      /// <typeparam name="TContainer">The type of the object the property, field, or method is declared on.</typeparam>
      /// <typeparam name="TMember">The type of the member</typeparam>
      /// <param name="expression">A Linq expression that returns a property, field, or method when an instance of the object is not available</param>
      /// <remarks>
      /// Usage:
      /// ExpressionUtilities.DeriveNameFromExpression((Type o)=> o.Property);
      /// ExpressionUtilities.DeriveNameFromExpression{ObjectType,PropertyType}((o)=> o._field);
      /// ExpressionUtilities.DeriveNameFromExpression{ObjectType,MethodReturnType}((o)=> o.GetValue());
      /// </remarks>
      /// <returns>The name of the property or field used in the expression.</returns>
        public static string DeriveNameFromExpression<TContainer, TMember>(Expression<Func<TContainer, TMember>> expression)
        {
            return DeriveNameFromExpression(expression.Body);
        }

        /// <summary>
        /// Derives a property, parameter, local variable, field, or method's name from a Lambda expression.
        /// </summary>
        /// <typeparam name="TMember">The type of the property, parameter, local, field, or method who's name is being requested.</typeparam>
        /// <param name="expression">A expression that returns a property , parameter, local variable, or field</param>
        /// <remarks>
        /// Usage: 
        /// ExpressionUtilities.DeriveNameFromExpression(()=> Object.Property);
        /// ExpressionUtilities.DeriveNameFromExpression(()=> Object.field);
        /// ExpressionUtilities.DeriveNameFromExpression(()=> localVariable);
        /// ExpressionUtilities.DeriveNameFromExpression(()=> methodParameter)
        /// ExpressionUtilities.DeriveNameFromExpression(()=> Object.Method())
        /// </remarks>
        /// <returns>The name of the property, parameter, local variable, or field used in the Lambda expression.</returns>
        public static string DeriveNameFromExpression<TMember>(Expression<Func<TMember>> expression)
        {
            return DeriveNameFromExpression(expression.Body);
        }

        /// <summary>
        /// Derives a name from a Lambda expression. 
        /// </summary>
        /// <param name="expression">the Lambda expression to gather the name from.</param>
        /// <returns>the name of the member used in the Lambda expression</returns>
        private static string DeriveNameFromExpression(System.Linq.Expressions.Expression expression)
        {
            // check to make sure a non-null expression was provided.
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            // is the expression's body a unary expression.
            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
            {
                expression = unaryExpression.Operand;
            }

            // is the expression's body a parameter expression.
            var parameterExpression = expression as ParameterExpression;
            if (parameterExpression != null)
            {
                return parameterExpression.Name;
            }

            // is the expression's body a member expression.
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }

            // is the expression's body a method call expression.
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return methodCallExpression.Method.Name;
            }

            // unable to derive name. throw an exception.
            throw new Exception(
                string.Format("Failed to derive name from expression '{0}'",
                expression));
        }

        public static object GetValue(System.Linq.Expressions.Expression expression)
        {
            // Is the provided expression a Lambda Expression. If so unwrap.
            var lambdaExpression = expression as LambdaExpression;
            if (lambdaExpression != null)
            {
                expression = lambdaExpression.Body;
            }


            // Is the expression a unchanging value.
            var constantExpression = expression as ConstantExpression;
            if (constantExpression != null)
            {
                return constantExpression.Value;
            }

            // Is the expression a property or field.
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                // Get the instance of the object the member is declared on
                object instance = GetValue(memberExpression.Expression);

                if (memberExpression.Member.MemberType == MemberTypes.Property)
                {
                    // TODO: Add support for property Indexers
                    return ((PropertyInfo)memberExpression.Member).GetValue(instance, null);
                }
                else
                {// member is a field
                    return ((FieldInfo)memberExpression.Member).GetValue(instance);
                }
            }

            // Is the expression a cast.
            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
            {
                object result = GetValue(unaryExpression.Operand);

                // TODO: Is it enough to do this for primitives types, or do we need 
                // to also do this for value types (value types also include enumerations and 
                // user created structs. they are different from primitives), and or reference 
                // types. If we need to do this for more than just primitives we need to rethink 
                // how to do this. The Convert.ChangeType method will not support user defined 
                // value types and does not support reference types. It's possible to use 
                // TypeConverters, but this will add a significant amount of time to this process.
                // http://www.hanselman.com/blog/TypeConvertersTheresNotEnoughTypeDescripterGetConverterInTheWorld.aspx
                // (http://msdn.microsoft.com/en-us/library/system.componentmodel.typeconverter%28v=vs.110%29.aspx)
                // http://jfoscoding.blogspot.com/2007/11/easy-mistake-using-typeconverter.html
                if (unaryExpression.Type.GetTypeInfo().IsPrimitive)
                {
                    //http://blogs.msdn.com/b/ericlippert/archive/2009/03/19/representation-and-identity.aspx
                    // This is important for values types to make sure double casting is not necessary if/when the user tries to unbox the result.
                    // This takes care of the representation-changing conversion that needs to take place when casting between an int and long for example.
                    return Convert.ChangeType(result, unaryExpression.Type);
                }

                return result;
            }

            // Is the expression a method call.
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return methodCallExpression.Method.Invoke(methodCallExpression.Object, GetMethodArguments(methodCallExpression));
            }

            // Last resort fallback to compiling the expression.
            // This is less than ideal and should be eliminated if possible. It will perform poorly.
            // http://www.minddriven.de/index.php/technology/dot-net/c-sharp/efficient-expression-values
            if (!expression.Type.Equals(typeof(object)))
            {
                // cast the expressions value to an object.
                expression = Expression.Convert(expression, typeof(object));
            }
            return Expression.Lambda<Func<object>>(expression)
                .Compile()
                .Invoke();
        }

        private static object[] GetMethodArguments(MethodCallExpression expression)
        {
            object[] result = new object[expression.Arguments.Count];
            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                result[i] = GetValue(expression.Arguments[i]);
            }
            return result;
        }
    }
}
