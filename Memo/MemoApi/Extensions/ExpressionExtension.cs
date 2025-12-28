using System.Linq.Expressions;

namespace MemoApi.Extensions
{
    /// <summary>
    /// 最简单实用的 Expression 扩展方法
    /// 只有2个方法：And 和 Or
    /// </summary>
    public static class ExpressionExtension
    {
        /// <summary>
        /// 合并两个表达式：两个条件都要满足（AND）
        /// 用法：expr1.And(expr2)
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expr1">第一个表达式</param>
        /// <param name="expr2">第二个表达式</param>
        /// <returns>合并后的表达式</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            // 如果第一个表达式为空，返回第二个
            if (expr1 == null) return expr2;
            // 如果第二个表达式为空，返回第一个
            if (expr2 == null) return expr1;

            // 创建一个新的参数 x
            // 相当于Lambda表达式中(ToDo todo) => todo.age > 8中的(ToDo todo)
            ParameterExpression param = Expression.Parameter(typeof(T), "t");

            // 替换第一个表达式的参数为新的参数
            Expression left = ReplaceParameter(expr1.Body, expr1.Parameters[0], param);
            // 替换第二个表达式的参数为新的参数
            Expression right = ReplaceParameter(expr2.Body, expr2.Parameters[0], param);

            // 创建 AND 表达式：left && right
            Expression andExpr = Expression.AndAlso(left, right);

            // 返回新的表达式：x => left && right
            return Expression.Lambda<Func<T, bool>>(andExpr, param);
        }

        /// <summary>
        /// 合并两个表达式：满足其中一个条件即可（OR）
        /// 用法：expr1.Or(expr2)
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="expr1">第一个表达式</param>
        /// <param name="expr2">第二个表达式</param>
        /// <returns>合并后的表达式</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            // 如果第一个表达式为空，返回第二个
            if (expr1 == null) return expr2;
            // 如果第二个表达式为空，返回第一个
            if (expr2 == null) return expr1;

            // 创建一个新的参数 x
            // 相当于Lambda表达式中(ToDo todo) => todo.age > 8中的(ToDo todo)
            ParameterExpression param = Expression.Parameter(typeof(T), "t");

            // 替换第一个表达式的参数为新的参数
            Expression left = ReplaceParameter(expr1.Body, expr1.Parameters[0], param);
            // 替换第二个表达式的参数为新的参数
            Expression right = ReplaceParameter(expr2.Body, expr2.Parameters[0], param);

            // 创建 OR 表达式：left || right
            Expression orExpr = Expression.OrElse(left, right);

            // 返回新的表达式：x => left || right
            return Expression.Lambda<Func<T, bool>>(orExpr, param);
        }



        /// <summary>
        /// 替换表达式中的参数（辅助方法）
        /// </summary>
        /// ParameterExpression 是 System.Linq.Expressions 命名空间下的类，专门表示表达式树中的 “参数 / 变量节点”，是构成表达式树的基础 “积木” 之一。
        /// (T t) => t.age > 18 中，Lambda 里的 t 就是由编译器自动创建的 ParameterExpression
        private static Expression ReplaceParameter(
            Expression body,  // 要搜索的"文档"（表达式主体），比如 p => p.age > 8 中的 p.age > 8
            ParameterExpression oldParam,  // 要查找的"关键词"（旧参数）,比如 p => p.age > 8 中的 p
            ParameterExpression newParam)  // 要替换成的"新文本"（新参数）,比如把 p => p.age > 8 中的 p 换成其他类型 t
        {
            // 创建智能搜索替换工具
            ParameterReplacer replacer = new ParameterReplacer(oldParam, newParam);

            // 使用工具搜索整个文档（表达式主体）
            // Visit() 方法会自动遍历整个表达式树
            return replacer.Visit(body);
        }

        /// <summary>
        /// 参数替换器（辅助类，只需要15行）
        /// </summary>
        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParam;  // 目标：要找的旧参数
            private readonly ParameterExpression _newParam;  // 替换：要换成的新参数

            // 构造函数：告诉替换器要找什么，换什么
            public ParameterReplacer(ParameterExpression oldParam, ParameterExpression newParam)
            {
                _oldParam = oldParam;  // 记住要查找的旧参数
                _newParam = newParam;  // 记住要替换成的新参数
            }

            // 关键：当系统遍历到参数节点时会自动调用此方法
            /*
             * VisitParameter 方法的 node 参数：
                ✅ 是 p（参数表达式）
                ❌ 不是 p.Age > 8（二元表达式）
                ❌ 不是 p.Age（成员表达式）
                ❌ 不是 8（常量表达式）
            */
            protected override Expression VisitParameter(ParameterExpression node)
            {
                // 检查当前遍历到的参数是不是我们要找的那个
                // 如果是，就返回新参数；如果不是，保持原样
                return node == _oldParam ? _newParam : node;
            }
        }
    }
}


// =======================================================
// Expression<Func<Product, bool>> expr = p => p.Price > 100;
// 整个表达式树的完整分解（各节点类型及属性说明）
// =======================================================

// -------------------------------------------------------
// 1. 整个表达式对象（expr）
// -------------------------------------------------------
// 类型：Expression<Func<Product, bool>>
// 说明：一个表示 Lambda 表达式的表达式树
// 重要属性：
//   - NodeType: ExpressionType.Lambda（Lambda 表达式节点）
//   - Type: System.Func<Product, bool>（委托类型）
//   - Body: Expression 类型，表达式主体部分（p.Price > 100）
//   - Parameters: 参数集合（包含一个 ParameterExpression）
//   - ReturnType: System.Boolean（返回值类型）

// -------------------------------------------------------
// 2. 参数部分（expr.Parameters[0]）
// -------------------------------------------------------
// 类型：ParameterExpression
// 说明：Lambda 表达式的参数 p
// 重要属性：
//   - NodeType: ExpressionType.Parameter（参数节点）
//   - Type: Product（参数类型）
//   - Name: "p"（参数名称）
//   - IsByRef: false（是否为引用传递）

// -------------------------------------------------------
// 3. 表达式主体（expr.Body）
// -------------------------------------------------------
// 类型：BinaryExpression（二元表达式）
// 说明：表示二元运算 p.Price > 100
// 重要属性：
//   - NodeType: ExpressionType.GreaterThan（大于运算符）
//   - Type: System.Boolean（运算结果类型）
//   - Left: Expression 类型，左操作数（p.Price）
//   - Right: Expression 类型，右操作数（100）

// -------------------------------------------------------
// 4. 左操作数（expr.Body.Left）
// -------------------------------------------------------
// 类型：MemberExpression（成员访问表达式）
// 说明：访问属性 p.Price
// 重要属性：
//   - NodeType: ExpressionType.MemberAccess（成员访问节点）
//   - Type: System.Decimal（属性类型）
//   - Member: System.Reflection.PropertyInfo（Price 属性信息）
//   - Expression: Expression 类型，访问的对象（p）

// -------------------------------------------------------
// 5. 左操作数中的参数（expr.Body.Left.Expression）
// -------------------------------------------------------
// 类型：ParameterExpression
// 说明：与 expr.Parameters[0] 是同一个对象
// 重要属性：同参数部分

// -------------------------------------------------------
// 6. 右操作数（expr.Body.Right）
// -------------------------------------------------------
// 类型：ConstantExpression（常量表达式）
// 说明：常量值 100
// 重要属性：
//   - NodeType: ExpressionType.Constant（常量节点）
//   - Type: System.Decimal（常量类型）
//   - Value: 100（常量值）

// -------------------------------------------------------
// 7. 委托信息（Func<Product, bool>）
// -------------------------------------------------------
// 说明：表达式编译后的委托类型
// 重要属性：
//   - 返回类型：bool（最后比较的结果）
//   - 参数列表：一个 Product 类型参数
//   - 泛型签名：Func<T, TResult> 类型

// =======================================================
// 表达式树结构图示：
// =======================================================
// LambdaExpression (expr)
// ├── Parameters[0] → ParameterExpression (p)
// └── Body → BinaryExpression (>)
//     ├── Left → MemberExpression (p.Price)
//     │    └── Expression → ParameterExpression (p)
//     └── Right → ConstantExpression (100)
// =======================================================

/*
// 实际使用示例（如何访问这些属性）：
Expression<Func<Product, bool>> expr = p => p.Price > 100;

// 1. 获取参数
ParameterExpression parameter = expr.Parameters[0];  // 参数 p
string paramName = parameter.Name;                   // "p"
Type paramType = parameter.Type;                     // typeof(Product)

// 2. 获取表达式主体
BinaryExpression body = (BinaryExpression)expr.Body; // p.Price > 100
ExpressionType nodeType = body.NodeType;             // ExpressionType.GreaterThan

// 3. 获取左操作数（属性访问）
MemberExpression left = (MemberExpression)body.Left; // p.Price
string propertyName = left.Member.Name;              // "Price"

// 4. 获取右操作数（常量值）
ConstantExpression right = (ConstantExpression)body.Right; // 100
decimal constantValue = (decimal)right.Value;              // 100

// 5. 编译为委托执行
Func<Product, bool> compiledFunc = expr.Compile();   // 编译为可执行代码
bool result = compiledFunc(new Product { Price = 200 }); // 执行返回 true
*/