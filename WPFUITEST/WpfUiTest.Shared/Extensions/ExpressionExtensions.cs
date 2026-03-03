using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WpfUiTest.Shared.Extensions
{
    #region 步骤2：表达式树拼接扩展类（核心逻辑，全注释）
    /// <summary>
    /// 表达式树拼接扩展类（支持AND/OR条件拼接，解决参数不匹配问题）
    /// 核心目标：将多个独立表达式（如p=>p.Age>18、q=>q.Name="张三"）拼接为单一表达式（p=>p.Age>18&&p.Name="张三"）
    /// </summary>
    #endregion
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 拼接AND条件（等价于C#的&&、SQL的AND）
        /// </summary>
        /// <typeparam name="T">查询的实体类型（如User）</typeparam>
        /// <param name="left">左边的基础表达式（如p=>p.Age>18）</param>
        /// <param name="right">右边待拼接的表达式（如q=>q.Name="张三"）</param>
        /// <returns>拼接后的完整表达式（如p=>p.Age>18&&p.Name="张三"）</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            #region 子步骤1：获取统一参数（解决多参数冲突的前提）
            // left.Parameters[0]：获取左边表达式的第一个（也是唯一）参数（如p）
            // 作用：所有拼接后的表达式必须共用同一个参数，否则EF Core无法解析
            ParameterExpression targetParameter = left.Parameters[0];
            #endregion

            #region 子步骤2：创建参数替换器（核心工具）
            // right.Parameters[0]：右边表达式的原始参数（如q）
            // targetParameter：要替换成的统一参数（如p）
            // 作用：初始化替换器，指定“把q换成p”
            ParameterReplacer parameterReplacer = new ParameterReplacer(right.Parameters[0], targetParameter);
            #endregion

            #region 子步骤3：替换右边表达式的参数（核心行）
            // right.Body：右边表达式的逻辑主体（如q.Name.Contains("张三")，不含参数q）
            // parameterReplacer.Visit()：遍历right.Body的所有节点，自动调用VisitParameter替换参数
            // 调用逻辑：Visit方法是ExpressionVisitor的核心遍历器，遇到Parameter节点自动调用VisitParameter
            // 返回值：替换后的逻辑主体（如p.Name.Contains("张三")）
            Expression replacedRightBody = parameterReplacer.Visit(right.Body);
            #endregion

            #region 子步骤4：拼接逻辑主体（仅拼判断规则，不碰参数）
            // Expression.AndAlso：将两个逻辑主体用“短路与”拼接（等价于&&）
            // left.Body：左边表达式的逻辑主体（如p.Age>18）
            // replacedRightBody：替换参数后的右边逻辑主体（如p.Name.Contains("张三")）
            // 结果：新的逻辑主体（如p.Age>18 && p.Name.Contains("张三")）
            BinaryExpression combinedLogicBody = Expression.AndAlso(left.Body, replacedRightBody);
            #endregion

            #region 子步骤5：组装完整Lambda表达式（绑定参数+逻辑）
            // Expression.Lambda：将“逻辑主体”和“统一参数”绑定，生成完整表达式
            // 参数1：combinedLogicBody → 表达式的判断规则（=>右边的部分）
            // 参数2：targetParameter → 表达式的参数（=>左边的p）
            // 结果：完整的Lambda表达式（p=>p.Age>18 && p.Name.Contains("张三")）
            Expression<Func<T, bool>> finalExpression = Expression.Lambda<Func<T, bool>>(combinedLogicBody, targetParameter);
            #endregion

            // 返回最终拼接好的表达式，可直接传入仓储的FindAsync方法
            return finalExpression;
        }

        /// <summary>
        /// 拼接OR条件（等价于C#的||、SQL的OR）
        /// 逻辑与AndAlso完全一致，仅拼接运算符从AndAlso改为OrElse
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            // 子步骤1：获取统一参数
            ParameterExpression targetParameter = left.Parameters[0];

            // 子步骤2：创建参数替换器
            ParameterReplacer parameterReplacer = new ParameterReplacer(right.Parameters[0], targetParameter);

            // 子步骤3：替换右边表达式的参数
            Expression replacedRightBody = parameterReplacer.Visit(right.Body);

            // 子步骤4：拼接逻辑主体（OR关系）
            BinaryExpression combinedLogicBody = Expression.OrElse(left.Body, replacedRightBody);

            // 子步骤5：组装完整Lambda表达式
            Expression<Func<T, bool>> finalExpression = Expression.Lambda<Func<T, bool>>(combinedLogicBody, targetParameter);

            return finalExpression;
        }

        /// <summary>
        /// 内部参数替换器（继承ExpressionVisitor，专门处理参数节点替换）
        /// ExpressionVisitor：.NET内置的表达式树遍历/修改工具，递归遍历所有节点
        /// </summary>
        private class ParameterReplacer : ExpressionVisitor
        {
            // 要替换的旧参数（如q）
            private readonly ParameterExpression _oldParameter;
            // 替换后的新参数（如p）
            private readonly ParameterExpression _newParameter;

            /// <summary>
            /// 构造函数：初始化新旧参数映射关系
            /// </summary>
            /// <param name="oldParameter">待替换的旧参数（如right表达式的q）</param>
            /// <param name="newParameter">目标新参数（如left表达式的p）</param>
            public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            /// <summary>
            /// 重写VisitParameter方法（仅处理参数节点）
            /// 调用时机：ExpressionVisitor的Visit方法遍历到Parameter节点时自动调用
            /// </summary>
            /// <param name="node">当前遍历到的参数节点（如q）</param>
            /// <returns>替换后的参数（如p）或原节点</returns>
            protected override Expression VisitParameter(ParameterExpression node)
            {
                // node的核心内容：存储参数的元信息（Name=q，Type=User）
                // 1. node.Name：参数名称（编译器自动生成，如q、p、r）
                // 2. node.Type：参数类型（如User，对应Expression<Func<User,bool>>的第一个泛型参数）

                // 判断当前节点是否是要替换的旧参数（用对象引用比较，比Name更可靠）
                if (node == _oldParameter)
                {
                    // 替换为新参数（如把q换成p）
                    return _newParameter;
                }
                else
                {
                    // 非目标参数，调用基类方法保留原节点（如其他参数/常量/属性节点）
                    return base.VisitParameter(node);
                }
            }
        }
    }
}
