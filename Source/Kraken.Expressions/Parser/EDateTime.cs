using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Kraken.Expressions.Parser
{
	/// <summary>
	/// The <see cref="ExpressionPart"/> implementation of the date or date+time constant.
	/// </summary>
	public class EDateTime : ExpressionPart
	{
		/// <summary>
		/// The list of <see cref="ExpressionPart"/> types which follows after the date or data+time constant.
		/// </summary>
		public override Type[] ExpectedParts => new Type[]
		{
			//typeof(EExpressionEnd),
			//typeof(EBinaryOperator),
			typeof(EMemberAccess),
		};

		/// <summary>
		/// The regular expression to match the date+time.
		/// </summary>
		public override string Regexp => Constants.REGEX_DateTime;

		/// <summary>
		/// Method to convert the date+time from string to <see cref="Expression"/>.
		/// </summary>
		/// <param name="context">The context of evaluation</param>
		/// <param name="message">The message to convert</param>
		/// <param name="data">Result expression data</param>
		/// <returns>The result expression</returns>
		public override Expression GetExpression(EvaluationContext context, string message, ref ExpressionData data)
		{
			DateTime value = DateTime.ParseExact(message, context.DateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);
			return Expression.Constant(value, value.GetType());
		}
	}
}
