using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kraken.Expressions
{
	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression
	{
		private Delegate compiled;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression)
		{
			Expression body = context.TranslateExpression(expression);
			LambdaExpression lambda = Expression.Lambda(body, parameters: context.ParamListExpressions
#if NET35
				.ToArray()
#endif
				);
			this.compiled = lambda.Compile();
		}

		/// <summary>
		/// Internal method to invoke the compiled expression.
		/// </summary>
		/// <param name="p">The parameter list.</param>
		/// <returns>The invokation result.</returns>
		protected object InvokeInternal(params object[] p)
		{
			try
			{
				return compiled.DynamicInvoke(p);
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException != null)
					throw ex.InnerException;
				throw;
			}
		}
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1) => InvokeInternal(p1);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2) => InvokeInternal(p1, p2);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3) => InvokeInternal(p1, p2, p3);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4) => InvokeInternal(p1, p2, p3, p4);
	}

#if !NET35
	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4, T5> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4, T5}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => InvokeInternal(p1, p2, p3, p4, p5);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4, T5, T6> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4, T5, T6}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) => InvokeInternal(p1, p2, p3, p4, p5, p6);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4, T5, T6, T7}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7) => InvokeInternal(p1, p2, p3, p4, p5, p6, p7);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4, T5, T6, T7, T8}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) => InvokeInternal(p1, p2, p3, p4, p5, p6, p7, p8);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9) => InvokeInternal(p1, p2, p3, p4, p5, p6, p7, p8, p9);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10) => InvokeInternal(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11) => InvokeInternal(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12) => InvokeInternal(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13) => InvokeInternal(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14) => InvokeInternal(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15) => InvokeInternal(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15);
	}

	/// <summary>
	/// Precompiled object which can invoke to get the result.
	/// </summary>
	public class PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : PrecompiledExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PrecompiledExpression{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/>
		/// </summary>
		/// <param name="context">The context of evaluation.</param>
		/// <param name="expression">Source expression in string.</param>
		public PrecompiledExpression(EvaluationContext context, string expression) : base(context, expression) { }

		/// <summary>
		/// Evaluate the precompiled expression to get the result.
		/// </summary>
		/// <returns>The evaluation result.</returns>
		public object Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16) => InvokeInternal(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16);
	}
#endif
}
