using System;

namespace Kraken.Expressions.Parser
{
	/// <summary>
	/// The <see cref="ExpressionPart"/> implementation of the ?: operator.
	/// </summary>
	public class EConditionalOperator : ExpressionPart
	{
		/// <summary>
		/// The list of <see cref="ExpressionPart"/> types which follows after the ?: operator.
		/// </summary>
		public override Type[] ExpectedParts => new[]
		{
			typeof(ExpressionBlock),
			typeof(EExpressionEnd),
			typeof(EDateTime),
			typeof(EConstant),
			typeof(EUnaryOperator),
			typeof(EIdentifier),
			typeof(EString),
		};

		/// <summary>
		/// The method to find out the ?: operators.
		/// </summary>
		/// <param name="context">The context of evaluation</param>
		/// <param name="expression">The rest of expression to parse</param>
		/// <returns>The string block to parse or null if the block doesn't match the current <see cref="EConditionalOperator"/></returns>
		public override string Get(EvaluationContext context, ref string expression)
		{
			if (expression.Length == 0 || (expression[0] != '?' && expression[0] != ':'))
				return null;
			var result = expression[0].ToString();
			expression = expression.Substring(1);
			return result;
		}
	}
}
