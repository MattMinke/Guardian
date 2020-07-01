using Guardian.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Guardian
{
    public static class Ensure
    {
        public static void That(Expression<Func<bool>> expression)
        {
            //TODO: need to introduce DI/IoC.
            //
            //var value = Utilities.GetValue(expression);
            ValidateExpressionVisitor visitor = new ValidateExpressionVisitor(
                new ReflectiveDefaultComparerFactory(), 
                new ReflectiveGetterFactory()
            );
            var rewritten = visitor.Visit(expression.Body);



            if (visitor._errors.Any())
            {
                throw new Exception(string.Join(", ", visitor._errors.Select(o => o.ToString())));
            }


            // TODO: 
            // 5) build exception if condition not met
            // 5.1) Globalization and localization should be present. 
            //      https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization
            // 5.2)

            
        }
    }
}
