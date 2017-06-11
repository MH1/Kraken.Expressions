using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kraken.Expressions
{
	/// <summary>
	/// The engine used for translate and evaluate the result expression.
	/// </summary>
	public class EvaluationEngine
    {
		/// <summary>
		/// Evaluate the <paramref name="expression"/> and returns the result of evaluation.
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		/// <returns>Result of evaluation.</returns>
		public virtual object EvaluateExpression(EvaluationContext context, string expression)
		{
			return EvaluateExpression<object>(context, expression);
		}

		/// <summary>
		/// Evaluate the <paramref name="expression"/> and returns the result of evaluation.
		/// </summary>
		/// <typeparam name="T">Type of the result.</typeparam>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		/// <returns>Result of evaluation.</returns>
		public virtual T EvaluateExpression<T>(EvaluationContext context, string expression)
		{
			Expression body = TranslateExpression(context, expression);
			LambdaExpression lambda = Expression.Lambda(body, parameters: context.ParamListExpressions
#if NET35
				.ToArray()
#endif
				);
			Delegate compiled = lambda.Compile();
			object result;
			try
			{
				result = compiled.DynamicInvoke(context.ParamListValues.ToArray());
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException != null)
					throw ex.InnerException;
				throw;
			}
			return (T)result;
		}

		/// <summary>
		/// Translate <paramref name="expression"/> into the expression.
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		/// <returns>The result expression.</returns>
		public virtual Expression TranslateExpression(EvaluationContext context, string expression)
		{
			return context.Parser.Parse(context, ref expression);
		}
	}
}
