using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Libra.orm.ExpressionExtend
{
    public class ExpressionToSql : ExpressionVisitor
    {
        /// <summary>
        /// 解析单向Lambda表达式 a => "1" 键值对形式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Tuple<string, object> VisitLambda(Expression node)
        {
            if (node.NodeType != ExpressionType.Lambda) throw new Exception("Not Lambda");
            var parameters = ((LambdaExpression)node).Parameters;
            if (parameters.Count > 1) throw new Exception("Lambda Parameter is Multiple Parameters");
            string key = parameters[0].Name;
            object value;
            if (((LambdaExpression)node).Body.NodeType == ExpressionType.Convert)
                value = ((ConstantExpression)((UnaryExpression)((LambdaExpression)node).Body).Operand).Value;
            else
                value = ((ConstantExpression)((LambdaExpression)node).Body).Value;
            return new Tuple<string, object>(key, value);
        }

        /// <summary>
        /// <remarks>SQL语句条件拼接</remarks>
        /// <remarks>stack:表示同一指定类型实例的可变大小后进先出（LIFO）集合</remarks>
        /// <remarks>string:指定堆栈中元素的类型</remarks>
        /// </summary>
        private readonly Stack<string> _sqlStringStack = new Stack<string>();

        /// <summary>
        /// <remarks>返回解析后的SQL条件语句</remarks>
        /// </summary>
        /// <returns></returns>
        public string GetSqlWhere()
        {
            // SQL语句条件字符串
            string sqlWhereStr = string.Join(" ", _sqlStringStack);
            // 清空栈
            _sqlStringStack.Clear();
            // 返回SQL条件语句解析结果字符串
            return sqlWhereStr;
        }

        /// <summary>
        /// <remarks>二元表达式：解读条件</remarks>
        /// </summary>
        /// <param name="binaryExpression">二元表达式</param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            // 拼接右括号：堆栈先进后出原则拼接
            _sqlStringStack.Push(")");
            // 解析表达式右边
            this.Visit(binaryExpression.Right);
            // 解析操作类型
            _sqlStringStack.Push(ExpressionOperator.ToSqlOperator(binaryExpression.NodeType));
            // 解析表达式左边
            this.Visit(binaryExpression.Left);
            // 拼接左括号
            _sqlStringStack.Push("(");
            return binaryExpression;
        }

        /// <summary>
        /// <remarks>解析属性/字段表达式</remarks>
        /// </summary>
        /// <param name="memberExpression">属性/字段表达式</param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            // 将属性/字段名称存入栈中
            this._sqlStringStack.Push(memberExpression.Member.Name);
            return memberExpression;
        }

        /// <summary>
        /// <remarks>解析常量表达式</remarks>
        /// </summary>
        /// <param name="constantExpression">常量表达式</param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression constantExpression)
        {
            // 将常量的值存入栈中
            this._sqlStringStack.Push(constantExpression.Value.ToString());
            return constantExpression;
        }

        /// <summary>
        /// <remarks>解析函数表达式</remarks>
        /// </summary>
        /// <param name="methodCallExpression">函数表达式</param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            string format = methodCallExpression.Method.Name switch
            {
                "StartWith" => "({0} like '{1}%')",
                "Contains" => "({0} like '%{1}%')",
                "EndWith" => "({0} like '%{1}')",
                "Equals" => "({0} = '{1}')",
                _ => throw new NotSupportedException(methodCallExpression.NodeType + " is not supported!"),
            };

            // 调用方法的属性：例如（name.contains("1")）,这里就是指name属性调用的contains函数 
            Expression instance = this.Visit(methodCallExpression.Object);

            // 参数：1就代表调用contains函数传递的参数值
            Expression expressionArray = this.Visit(methodCallExpression.Arguments[0]);

            // 返回栈顶部的对象并删除
            string right = this._sqlStringStack.Pop();
            string left = this._sqlStringStack.Pop();

            // 将解析后的结果存入栈中
            this._sqlStringStack.Push(String.Format(format, left, right));
            return methodCallExpression;
        }
    }
}