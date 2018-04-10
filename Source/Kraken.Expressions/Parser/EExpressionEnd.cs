using System;

namespace Kraken.Expressions.Parser
{
	/// <summary>
	/// The <see cref="ExpressionPart"/> implementation of the end of the expression block (brackets).
	/// </summary>
	public class EExpressionEnd : ExpressionPart
	{
		/// <summary>
		/// The list of <see cref="ExpressionPart"/> types which follows after the end of expression block.
		/// </summary>
		public override Type[] ExpectedParts => new[]
		{
			typeof(EExpressionEnd),
			typeof(EBinaryOperator),
			typeof(EConditionalOperator),

			typeof(ETypeCast),
			typeof(EConstant),
			typeof(EIdentifier),
			typeof(EString),
			typeof(EChar),
		};

		/// <summary>
		/// The method to find out the end of the expression block.
		/// </summary>
		/// <param name="context">The context of evaluation</param>
		/// <param name="expression">The rest of expression to parse</param>
		/// <returns>The string block to parse or null if the block doesn't match the current <see cref="EExpressionEnd"/></returns>
		public override string Get(EvaluationContext context, ref string expression)
		{
			if (expression[0] != ')')
				return null;
			expression = expression.Substring(1);
			return ")";
		}
	}
}
