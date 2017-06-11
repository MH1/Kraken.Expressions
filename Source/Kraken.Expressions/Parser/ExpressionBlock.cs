using System;

namespace Kraken.Expressions.Parser
{
	/// <summary>
	/// The <see cref="ExpressionPart"/> implementation of the expression block (brackets).
	/// </summary>
	public class ExpressionBlock : ExpressionPart
	{
		/// <summary>
		/// The list of <see cref="ExpressionPart"/> types which follows after the expression block.
		/// </summary>
		public override Type[] ExpectedParts => new[]
		{
			typeof(ExpressionBlock),
			typeof(EDateTime),
			typeof(EConstant),
			typeof(EUnaryOperator),
			typeof(EIdentifier),
			typeof(EString),
		};

		/// <summary>
		/// The method to find out the expression block.
		/// </summary>
		/// <param name="context">The context of evaluation</param>
		/// <param name="expression">The rest of expression to parse</param>
		/// <returns>The string block to parse or null if the block doesn't match the current <see cref="ExpressionBlock"/></returns>
		public override string Get(EvaluationContext context, ref string expression)
		{
			if (expression[0] != '(')
				return null;
			expression = expression.Substring(1);
			return "(";
		}
	}
}
