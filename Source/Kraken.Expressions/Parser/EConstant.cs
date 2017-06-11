using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Kraken.Expressions.Parser
{
	/// <summary>
	/// The <see cref="ExpressionPart"/> implementation of the number constant.
	/// </summary>
	public class EConstant : ExpressionPart
	{
		/// <summary>
		/// The list of <see cref="ExpressionPart"/> types which follows after the number constant.
		/// </summary>
		public override Type[] ExpectedParts => new[]
		{
			typeof(EExpressionEnd),
			typeof(EBinaryOperator),
		};

		/// <summary>
		/// The regular expression to match the number constant.
		/// </summary>
		public override string Regexp => Constants.REGEX_Number;

		/// <summary>
		/// Method to convert the number constant from string to <see cref="Expression"/>.
		/// </summary>
		/// <param name="context">The context of evaluation</param>
		/// <param name="message">The message to convert</param>
		/// <returns>The result expression</returns>
		public override Expression GetExpression(EvaluationContext context, string message)
		{
			object value;
			string strval = message.TrimEnd(new[] { 'm', 'l', 'f' }).Trim();
			if (message.EndsWith("m"))
			{
				value = decimal.Parse(strval, CultureInfo.InvariantCulture);
			}
			else if (message.EndsWith("l"))
			{
				value = long.Parse(strval, CultureInfo.InvariantCulture);
			}
			else if (message.EndsWith("f"))
			{
				value = float.Parse(strval, CultureInfo.InvariantCulture);
			}
			else if (message.Contains("."))
			{
				value = double.Parse(strval, CultureInfo.InvariantCulture);
			}
			else if (message.Length > 12)
			{
				value = long.Parse(strval, CultureInfo.InvariantCulture);
			}
			else
			{
				value = int.Parse(strval, CultureInfo.InvariantCulture);
			}
			return Expression.Constant(value, value.GetType());
		}
	}
}
