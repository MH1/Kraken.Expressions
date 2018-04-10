using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Kraken.Expressions.Parser
{
	/// <summary>
	/// The <see cref="ExpressionPart"/> implementation of the type cast.
	/// </summary>
	public class ETypeCast : EIdentifier
    {
		/// <summary>
		/// The list of <see cref="ExpressionPart"/> types which follows after the identifier.
		/// </summary>
		public override Type[] ExpectedParts => new[]
		{
			typeof(EExpressionEnd),
			typeof(EBinaryOperator),
			typeof(EConditionalOperator),

			typeof(ETypeCast),
			typeof(ExpressionBlock),
			typeof(EDateTime),
			typeof(EConstant),
			typeof(EUnaryOperator),
			typeof(EIdentifier),
			typeof(EString),
			typeof(EChar),
		};
		
		/// <summary>
		/// The regular expression to match the identifier.
		/// </summary>
		public override string Regexp => $@"\({base.Regexp}\)";

		/// <summary>
		/// Result Type to cast.
		/// </summary>
		/// <param name="t">Result type</param>
		/// <param name="data">Output data</param>
		protected override void CastTo(Type t, ref ExpressionData data)
		{
			data.TypeToCast = t;
		}

		/// <summary>
		/// Method to convert the identifier from string to <see cref="Expression"/>.
		/// </summary>
		/// <param name="context">The context of evaluation</param>
		/// <param name="message">The message to convert</param>
		/// <param name="data">Result expression data</param>
		/// <returns>The result expression</returns>
		public override Expression GetExpression(EvaluationContext context, string message, ref ExpressionData data)
		{
			if ((message?.Length ?? 0) < 2)
				return null;
			message = message.Substring(1, message.Length - 2);
			return base.GetExpression(context, message, ref data);
		}
	}
}
