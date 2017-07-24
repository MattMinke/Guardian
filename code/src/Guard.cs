using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace System
{
    public static class Guard
    {
        public static void Requires(Expression<Func<bool>> expression)
        {
            // TODO: 
            // 1) Break expression in to individual parts. 
            //    example: Guard.Requires(() => argument != null && argument.Length > 10)
            //    should be broken into two expressions
            //        argument != null 
            //        argument.Length > 10
            // 2) extract parts
            // 3) Determine operator. !=, >, <, ==, etc. Should be able to use expression.NodeType for most.
            // 4) compare left to right
            // 5) build exception if condition not met
            // 5.1) 


        }
    }
}
