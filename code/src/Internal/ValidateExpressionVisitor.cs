using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Collections;
using System.Linq;

namespace Guardian.Internal
{
    public class ValidateExpressionVisitor : ExpressionVisitor
    {

        private readonly Stack<object> _state;
        public readonly Stack<object> _errors;
        private readonly Stack<Expression> _traversal;
        private readonly IComparerFactory _compareFactory;
        private readonly IGetterFactory _getterFactory;
        //private Expression entryPoint;

        public ValidateExpressionVisitor(IComparerFactory compareFactory, IGetterFactory getterFactory)
        {
            _compareFactory = compareFactory;
            _getterFactory = getterFactory;
            _state = new Stack<object>();
            _errors = new Stack<object>();
            _traversal = new Stack<Expression>();
        }


        protected override Expression VisitConstant(ConstantExpression node)
        {
            _state.Push(node.Value);
            var expression = base.VisitConstant(node);

            return expression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Visit(node.Object);
            object target = null;
            if (_state.Any())
            {
                target = _state.Pop();
            }

            object[] parameters = new object[node.Arguments.Count];
            int index = 0;
            foreach (var expression in node.Arguments)
            {
                Visit(expression);
                parameters[index] = _state.Pop();
                index++;
            }

            var value = node.Method.Invoke(target, parameters);
            _state.Push(value);
            if (_traversal.Count < 2 && object.ReferenceEquals(node, _traversal.Peek()) && !(bool)value)
            {
                //TODO: Need to think about how to get a meaninful message. 
                //error
                _errors.Push("TODO: create a error object that can be used to create a message from later.");
            }
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var expression = base.VisitUnary(node);

            var success = !(bool)_state.Pop();

            if (!success)
            {
                //TODO: Need to think about how to get a meaninful message. 
                //error
                _errors.Push("TODO: create a error object that can be used to create a message from later.");
            }
            _state.Push(success);
            return expression;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var expression = base.VisitBinary(node);

            var right = _state.Pop();
            var left = _state.Pop();

            switch (node.NodeType)
            {
                case ExpressionType.ArrayIndex:
                    _state.Push(((Array)left).GetValue((int)right));
                    return expression;
            }

            IComparer comparer = _compareFactory.GetComparer((left ?? right).GetType());

            bool success = false;
            switch (node.NodeType)
            {
                case ExpressionType.AndAlso:
                    success = (bool)left && (bool)right;
                    break;
                case ExpressionType.OrElse:
                    success = (bool)left || (bool)right;
                    break;
                case ExpressionType.Equal:
                    success = comparer.Compare(left, right) == 0;
                    break;
                case ExpressionType.NotEqual:
                    success = comparer.Compare(left, right) != 0;
                    break;
                case ExpressionType.GreaterThan:
                    success = comparer.Compare(left, right) > 0;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    success = comparer.Compare(left, right) >= 0;
                    break;
                case ExpressionType.LessThan:
                    success = comparer.Compare(left, right) < 0;
                    break;
                case ExpressionType.LessThanOrEqual:
                    success = comparer.Compare(left, right) <= 0;
                    break;
                default:
                    throw new NotSupportedException($"The node type {node.NodeType} is not supported.");
            }

            if (!success && (_traversal.Count < 2 || _traversal.Count >= 2 && _traversal.Skip(1).First().NodeType != ExpressionType.Not && _traversal.Skip(1).First().Type == typeof(bool)))
            {
                //TODO: Need to think about how to get a meaninful message. 
                _errors.Push("TODO: create a error object that can be used to create a message from later.");
            }
            _state.Push(success);

            return expression;
        }
        
        protected override Expression VisitMember(MemberExpression node)
        {
            var expression = base.VisitMember(node);

            var target = _state.Pop();

            Func<object, object> @delegate = _getterFactory.Create(node.Member);

            object value = @delegate.Invoke(target);

            _state.Push(value);
            return expression;
        }


        public override Expression Visit(Expression node)
        {
            _traversal.Push(node);
          
            var expression = base.Visit(node);
            
            _traversal.Pop();
            return expression;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            _state.Push(((LambdaExpression)node).Compile());
            return node;
        }

        /*
        
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return base.VisitConditional(node);
        }
        protected override Expression VisitIndex(IndexExpression node)
        {
            return base.VisitIndex(node);
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }
        protected override Expression VisitBlock(BlockExpression node)
        {
            return base.VisitBlock(node);
        }
        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return base.VisitCatchBlock(node);
        }
        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            return base.VisitDebugInfo(node);
        }
        protected override Expression VisitDefault(DefaultExpression node)
        {
            return base.VisitDefault(node);
        }
        protected override ElementInit VisitElementInit(ElementInit node)
        {
            return base.VisitElementInit(node);
        }
        protected override Expression VisitExtension(Expression node)
        {
            return base.VisitExtension(node);
        }
        protected override Expression VisitGoto(GotoExpression node)
        {
            return base.VisitGoto(node);
        }
        protected override Expression VisitInvocation(InvocationExpression node)
        {
            return base.VisitInvocation(node);
        }
        protected override Expression VisitLabel(LabelExpression node)
        {
            return base.VisitLabel(node);
        }
        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            return base.VisitLabelTarget(node);
        }
        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            return base.VisitTypeBinary(node);
        }
        protected override Expression VisitTry(TryExpression node)
        {
            return base.VisitTry(node);
        }
        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            return base.VisitSwitchCase(node);
        }
        protected override Expression VisitSwitch(SwitchExpression node)
        {
            return base.VisitSwitch(node);
        }
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return base.VisitRuntimeVariables(node);
        }
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            return base.VisitNewArray(node);
        }
        protected override Expression VisitNew(NewExpression node)
        {
            return base.VisitNew(node);
        }
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            return base.VisitMemberMemberBinding(node);
        }
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            return base.VisitMemberListBinding(node);
        }
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return base.VisitMemberInit(node);
        }
        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return base.VisitMemberBinding(node);
        }
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return base.VisitMemberAssignment(node);
        }
        protected override Expression VisitLoop(LoopExpression node)
        {
            return base.VisitLoop(node);
        }
        protected override Expression VisitListInit(ListInitExpression node)
        {
            return base.VisitListInit(node);
        }
        */
    }
}
