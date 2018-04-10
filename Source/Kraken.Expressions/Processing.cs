using Kraken.Expressions.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kraken.Expressions
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TParserEntry">The entry point for parsing expressions</typeparam>
	public class Processing<TParserEntry> where TParserEntry : IExpressionPart
	{
		private static readonly MethodInfo StringConcat;
		private static readonly MethodInfo CharToString;

		static Processing()
		{
			StringConcat = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
			CharToString = typeof(char).GetMethod("ToString", new Type[0]);
		}

		#region Single instance evaluation objects

		private static Dictionary<Type, IExpressionPart> EvalObjects = new Dictionary<Type, IExpressionPart>();
		private static object EvalObjectsLock = new object();

		/// <summary>
		/// Get the <seealso cref="ExpressionPart"/> object instance to parse the expression.
		/// </summary>
		/// <typeparam name="TExpressionPart">The type of the <seealso cref="ExpressionPart"/>.</typeparam>
		/// <returns></returns>
		protected static TExpressionPart GetObject<TExpressionPart>() where TExpressionPart : IExpressionPart
		{
			Type t = typeof(TExpressionPart);
			return (TExpressionPart)GetObject(t);
		}

		/// <summary>
		/// Get the <seealso cref="ExpressionPart"/> object instance to parse the expression.
		/// </summary>
		/// <param name="type">The type of the <seealso cref="ExpressionPart"/>.</param>
		/// <returns></returns>
		protected static IExpressionPart GetObject(Type type)
		{
			lock (EvalObjectsLock)
			{
				if (EvalObjects.ContainsKey(type))
					return EvalObjects[type];
				IExpressionPart newObj = (IExpressionPart)Activator.CreateInstance(type);
				EvalObjects.Add(type, newObj);
				return EvalObjects[type];
			}
		}

		#endregion

		internal virtual Expression Parse(EvaluationContext context, ref string code)
		{
			if (code == null)
			{
				throw new ArgumentNullException(nameof(code));
			}
			IList<ProcessedItem> expressionTree = new List<ProcessedItem>();
			IExpressionPart current = GetObject<TParserEntry>();
			code = code.TrimStart(context.Whitespaces);
			string source = code;
			while (!string.IsNullOrEmpty(code))
			{
				bool found = false;
				foreach (Type partType in current.ExpectedParts)
				{
					IExpressionPart newPart = GetObject(partType);
					string part = newPart.Get(context, ref code);
					if (!string.IsNullOrEmpty(part))
					{
						expressionTree.Add(ProcessedItem.Create(partType, part));
						current = newPart;
						found = true;
						break;
					}
				}
				if (!found)
				{
					string dependentObjects = string.Join(", ",
						current.ExpectedParts.Select(o => o.Name)
#if NET35
						.ToArray()
#endif
						);
					throw new EvaluationException($"Invalid expression, unable to parse.\nCurrent part: {current.GetType().Name}\nFollowing parts: {dependentObjects}\n{code}") { SourceExpression = source, Position = source.Length - code.Length + 1 };
				}
				code = code.TrimStart(context.Whitespaces);
			}

			Expression compiled = Compile(context, expressionTree);
			return compiled;
		}

		/// <summary>
		/// Compile the expression tree produced by parser.
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expressionTree">The expression tree.</param>
		/// <returns>Result expression.</returns>
		protected virtual Expression Compile(EvaluationContext context, IList<ProcessedItem> expressionTree)
		{
			// move brackets into subtrees
			TreeLoop(expressionTree, i =>
			{
				ProcessedItem item = expressionTree[i];
				if (item.ItemType == typeof(ExpressionBlock))
				{
					int num = 1;
					int j = i + 1;
					while (num > 0 && j < expressionTree.Count)
					{
						ProcessedItem itemToMove = expressionTree[j];
						if (itemToMove.ItemType == typeof(EExpressionEnd))
							num--;
						else if (itemToMove.ItemType == typeof(ExpressionBlock))
							num++;
						if (itemToMove.ItemType != typeof(EExpressionEnd) || num > 0)
							item.Items.Add(itemToMove);
						expressionTree.RemoveAt(j);
					}
					if (num > 0)
					{
						throw new EvaluationException("Expected ')' at the end of the expression.");
					}
					return true;
				}
				return false;
			});

			// compile subtrees
			ReverseLoop(expressionTree, i =>
			{
				ProcessedItem item = expressionTree[i];
				if (item.ItemType == typeof(ExpressionBlock))
				{
					Expression expr = Compile(context, item.Items);
					ProcessedItem newItem = ProcessedItem.Create(expr);
					expressionTree.RemoveAt(i);
					expressionTree.Insert(i, newItem);
					return true;
				}
				return false;
			});

			bool changed;
			do
			{
				changed = false;

				changed |= ReverseLoop(expressionTree, i =>
				{
					ProcessedItem item1 = expressionTree[i];
					if (item1.ItemType != typeof(ETypeCast) || item1.TypeToCast == null)
						return false;
					if (i + 1 >= expressionTree.Count)
						return false;
					ProcessedItem item2 = expressionTree[i + 1];
					if (item2 is ProcessedItem)
					{
						Expression expr = Expression.Convert(item2.Expression, item1.TypeToCast);
						ProcessedItem newItem = ProcessedItem.Create(expr);
						expressionTree.RemoveAt(i);
						expressionTree.RemoveAt(i);
						expressionTree.Insert(i, newItem);
						return true;
					}
					return false;
				});

				changed |= CompileSingleItem(context, expressionTree);
				changed |= CompileMemberAccess(expressionTree);

				changed |= CompileUnaryOperators(context, expressionTree);
				changed |= CompileBinaryOperators(context, expressionTree);
				changed |= CompileIIfOperators(context, expressionTree);

				if (expressionTree.Count == 1 && expressionTree[0].Expression != null)
					return expressionTree[0].Expression;

				if (expressionTree.Count == 1 && expressionTree[0].TypeToCast != null)
					return null;

			} while (changed);

			throw new EvaluationException($"Expression compilation failed.\n{string.Join("\n", expressionTree.Select(o => o.Content).ToArray())}");
		}

		/// <summary>
		/// Compile the single item if it is possible to compile.
		/// It concerns those <see cref="ExpressionPart"/>s items which returns expression in <seealso cref="ExpressionPart.GetExpression(EvaluationContext, string, ref ExpressionData)"/>.
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expressionTree">The expression tree.</param>
		protected virtual bool CompileSingleItem(EvaluationContext context, IList<ProcessedItem> expressionTree)
		{
			// compile single item
			return ReverseLoop(expressionTree, i =>
			{
				ProcessedItem item = expressionTree[i];
				if (item.ItemType != null)
				{
					IExpressionPart obj = GetObject(item.ItemType);
					ExpressionData data = new ExpressionData();
					Expression compiled = obj.GetExpression(context, item.Content, ref data);
					if (compiled != null)
					{
						ProcessedItem newItem = ProcessedItem.Create(compiled);
						expressionTree.RemoveAt(i);
						expressionTree.Insert(i, newItem);
						return true;
					}
					else if (data.TypeToCast != null)
					{
						ProcessedItem newItem = ProcessedItem.CreateCast(data.TypeToCast);
						expressionTree.RemoveAt(i);
						expressionTree.Insert(i, newItem);
						return true;
					}
				}
				return false;
			});
		}

		/// <summary>
		/// Compile the binary operators in order of priority which is set on the <seealso cref="EvaluationContext.Priority"/>.
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expressionTree">The expression tree.</param>
		/// <returns>True if it changed the expression tree.</returns>
		protected virtual bool CompileBinaryOperators(EvaluationContext context, IList<ProcessedItem> expressionTree)
		{
			bool changed = false;
			for (int p = 0; p < context.Priority.Length && expressionTree.Count > 1; p++)
			{
				changed |= CompileBinaryOperatorsByPriority(context, expressionTree, p);
			}
			return changed;
		}

		/// <summary>
		/// Compile the binary operators on the same level of priority. It uses the <seealso cref="EvaluationContext.Priority"/> setting.
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expressionTree">The expression tree.</param>
		/// <param name="priority">The priority level.</param>
		/// <returns>True if it changed the expression tree.</returns>
		protected virtual bool CompileBinaryOperatorsByPriority(EvaluationContext context, IList<ProcessedItem> expressionTree, int priority) =>
			ReverseLoop(expressionTree, i =>
			{
				ProcessedItem item = expressionTree[i];
				if (i > 0 && expressionTree.Count > i + 1
					&& item.ItemType == typeof(EBinaryOperator)
					&& expressionTree[i - 1].Expression != null
					&& expressionTree[i + 1].Expression != null
					&& (context.Priority[priority].Contains(item.Content, StringComparer.OrdinalIgnoreCase)))
				{
					Expression expr1 = expressionTree[i - 1].Expression;
					Expression expr2 = expressionTree[i + 1].Expression;

					Type resultType = context.ConsolidateBinaryTypes(context, expr1.Type, expr2.Type);

					if ((expr1.Type == typeof(string) || expr1.Type == typeof(char)) && item.Content == "+")
					{
						resultType = typeof(string);
					}
					if (expr1.Type != resultType)
						expr1 = ConvertExpression(expr1, resultType);
					if (expr2.Type != resultType)
						expr2 = ConvertExpression(expr2, resultType);
					Expression expr;
					if ((expr1.Type == typeof(string) || expr2.Type == typeof(string)) && item.Content == "+")
					{
						expr = Expression.Call(StringConcat, new[] { expr1, expr2 });
					}
					else
					{
						expr =
							Expression.MakeBinary(context.BinaryNodeTypes[item.Content],
							expr1,
							expr2);
					}
					ProcessedItem newItem = ProcessedItem.Create(expr);
					expressionTree.RemoveAt(i - 1);
					expressionTree.RemoveAt(i - 1);
					expressionTree.RemoveAt(i - 1);
					expressionTree.Insert(i - 1, newItem);
					return true;
				}
				return false;
			});

		private Expression ConvertExpression(Expression expression, Type resultType)
		{
			if (expression.Type == typeof(char) && resultType == typeof(string))
				return Expression.Call(expression, CharToString);
			else
				return Expression.Convert(expression, resultType);
		}

		/// <summary>
		/// Compile the conditional operators.
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expressionTree">The expression tree.</param>
		/// <returns>True if it changed the expression tree.</returns>
		protected virtual bool CompileIIfOperators(EvaluationContext context, IList<ProcessedItem> expressionTree) =>
			ReverseLoop(expressionTree, i =>
			{
				ProcessedItem item = expressionTree[i];
				if (i > 0 && expressionTree.Count > i + 3
					&& expressionTree[i - 1].Expression != null
					&& item.ItemType == typeof(EConditionalOperator)
					&& item.Content == "?"
					&& expressionTree[i + 1].Expression != null
					&& expressionTree[i + 2].ItemType == typeof(EConditionalOperator)
					&& expressionTree[i + 2].Content == ":"
					&& expressionTree[i + 3].Expression != null
					)
				{
					Expression expr0 = expressionTree[i - 1].Expression;
					Expression expr1 = expressionTree[i + 1].Expression;
					Expression expr2 = expressionTree[i + 3].Expression;

					Type resultType = context.ConsolidateBinaryTypes(context, expr1.Type, expr2.Type);
					if (expr1.Type != resultType)
						expr1 = Expression.Convert(expr1, resultType);
					if (expr2.Type != resultType)
						expr2 = Expression.Convert(expr2, resultType);

					Expression expr = Expression.Condition(expr0, expr1, expr2);
					ProcessedItem newItem = ProcessedItem.Create(expr);
					expressionTree.RemoveAt(i - 1);
					expressionTree.RemoveAt(i - 1);
					expressionTree.RemoveAt(i - 1);
					expressionTree.RemoveAt(i - 1);
					expressionTree.RemoveAt(i - 1);
					expressionTree.Insert(i - 1, newItem);
					return true;
				}
				return false;
			});

		/// <summary>
		/// Compile the unary operators.
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expressionTree">The expression tree.</param>
		/// <returns>True if it changed the expression tree.</returns>
		protected virtual bool CompileUnaryOperators(EvaluationContext context, IList<ProcessedItem> expressionTree) =>
			TreeLoop(expressionTree, i =>
			{
				ProcessedItem item = expressionTree[i];
				if (expressionTree.Count > i + 1
					&& item.ItemType == typeof(EUnaryOperator)
					&& expressionTree[i + 1].Expression != null)
				{
					Expression expr = expressionTree[i + 1].Expression;
					expr =
						Expression.MakeUnary(context.UnaryNodeTypes[item.Content],
							expr,
							expr.Type
						);
					ProcessedItem newItem = ProcessedItem.Create(expr);
					expressionTree.RemoveAt(i);
					expressionTree.RemoveAt(i);
					expressionTree.Insert(i, newItem);
					return true;
				}
				return false;
			});

		/// <summary>
		/// Compile the member access.
		/// </summary>
		/// <param name="expressionTree">The expression tree.</param>
		/// <returns>True if it changed the expression tree.</returns>
		protected virtual bool CompileMemberAccess(IList<ProcessedItem> expressionTree)
			=> ReverseLoop(expressionTree, i =>
			{
				ProcessedItem item = expressionTree[i];
				if (i > 0
					&& item.ItemType == typeof(EMemberAccess)
					&& expressionTree[i - 1].Expression != null)
				{
					Expression expr = expressionTree[i - 1].Expression;
					expr = Expression.PropertyOrField(expr, item.Content.Substring(1));
					ProcessedItem newItem = ProcessedItem.Create(expr);
					expressionTree.RemoveAt(i - 1);
					expressionTree.RemoveAt(i - 1);
					expressionTree.Insert(i - 1, newItem);
					return true;
				}
				return false;
			});

		/// <summary>
		/// Cycle through the <paramref name="expressionTree"/> and process items with <paramref name="func"/>.
		/// </summary>
		/// <param name="expressionTree">The expression tree.</param>
		/// <param name="func">Delegate which defines what to do with expression tree.</param>
		/// <returns>True if it changed the expression tree.</returns>
		protected virtual bool TreeLoop(IList<ProcessedItem> expressionTree, Func<int, bool> func)
		{
			bool changed = false;
			for (int i = 0; i < expressionTree.Count; i++)
			{
				changed |= func(i);
			}
			return changed;
		}

		/// <summary>
		/// Reverse cycle through the <paramref name="expressionTree"/> and process items with <paramref name="func"/>.
		/// </summary>
		/// <param name="expressionTree">The expression tree.</param>
		/// <param name="func">Delegate which defines what to do with expression tree.</param>
		/// <returns>True if it changed the expression tree.</returns>
		protected virtual bool ReverseLoop(IList<ProcessedItem> expressionTree, Func<int, bool> func)
		{
			bool changed = false;
			for (int i = expressionTree.Count - 1; i >= 0; i--)
			{
				if (i < expressionTree.Count)
					changed |= func(i);
			}
			return changed;
		}
	}
}
