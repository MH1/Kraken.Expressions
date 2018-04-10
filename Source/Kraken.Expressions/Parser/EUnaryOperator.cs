using System;
using System.Linq;

namespace Kraken.Expressions.Parser
{
	/// <summary>
	/// The <see cref="ExpressionPart"/> implementation of the unary operator.
	/// </summary>
	public class EUnaryOperator : ExpressionPart
	{
		/// <summary>
		/// The list of <see cref="ExpressionPart"/> types which follows after the unary operator.
		/// </summary>
		public override Type[] ExpectedParts => new[]
		{
			typeof(ETypeCast),
			typeof(ExpressionBlock),
			typeof(EConstant),
			typeof(EIdentifier),
			typeof(EString),
			typeof(EChar),
		};

		/// <summary>
		/// The method to find out the unary operators from the list (in the <seealso cref="EvaluationContext"/>).
		/// </summary>
		/// <param name="context">The context of evaluation</param>
		/// <param name="expression">The rest of expression to parse</param>
		/// <returns>The string block to parse or null if the block doesn't match the current <see cref="EUnaryOperator"/></returns>
		public override string Get(EvaluationContext context, ref string expression)
		{
			string result = null;
			string exp = expression;
			result = context.UnaryNodeTypes.Keys.Where(v => exp.StartsWith(v)).OrderByDescending(v => v.Length).FirstOrDefault();

			int len = result?.Length ?? 0;
			if (len > 0)
				expression = expression.Substring(len);
			return result;
		}
	}
}
