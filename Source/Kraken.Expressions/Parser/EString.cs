using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Kraken.Expressions.Parser
{
	/// <summary>
	/// The <see cref="ExpressionPart"/> implementation of the string constants.
	/// </summary>
	public class EString : ExpressionPart
	{
		/// <summary>
		/// The list of <see cref="ExpressionPart"/> types which follows after the string constants.
		/// </summary>
		public override Type[] ExpectedParts => new[]
		{
			typeof(EExpressionEnd),
			typeof(EBinaryOperator),
		};

		/// <summary>
		/// The regular expression to match the string constant.
		/// </summary>
		public override string Regexp => Constants.REGEX_String;

		/// <summary>
		/// Method to convert the string constant from string to <see cref="Expression"/>.
		/// </summary>
		/// <param name="context">The context of evaluation</param>
		/// <param name="message">The message to convert</param>
		/// <returns>The result expression</returns>
		public override Expression GetExpression(EvaluationContext context, string message)
		{
			// replace doubled quotes
			string value = message.Replace(@"\""", "\"");
			// remove quotes at the beginning and at the end of string
			value = Regex.Replace(value, @"^""", "");
			value = Regex.Replace(value, @"""$", "");
			// replace special chars
			value = value.Replace(@"\a", "\a");
			value = value.Replace(@"\b", "\b");
			value = value.Replace(@"\f", "\f");
			value = value.Replace(@"\n", "\n");
			value = value.Replace(@"\r", "\r");
			value = value.Replace(@"\t", "\t");
			value = value.Replace(@"\v", "\v");
			// replace characters escaped with backslash
			value = Regex.Replace(value, @"\\(.)", "$1");
			// return result string constant as expression
			return Expression.Constant(value, typeof(string));
		}
	}
}
