using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Kraken.Expressions.Parser
{
	/// <summary>
	/// The <see cref="ExpressionPart"/> implementation of the char constants.
	/// </summary>
	public class EChar : ExpressionPart
	{
		/// <summary>
		/// The list of <see cref="ExpressionPart"/> types which follows after the char constants.
		/// </summary>
		public override Type[] ExpectedParts => new[]
		{
			typeof(EExpressionEnd),
			typeof(EBinaryOperator),
			typeof(EConditionalOperator),
		};

		/// <summary>
		/// The regular expression to match the char constant.
		/// </summary>
		public override string Regexp => Constants.REGEX_Char;

		/// <summary>
		/// Method to convert the char constant from string to <see cref="Expression"/>.
		/// </summary>
		/// <param name="context">The context of evaluation</param>
		/// <param name="message">The message to convert</param>
		/// <param name="data">Result expression data</param>
		/// <returns>The result expression</returns>
		public override Expression GetExpression(EvaluationContext context, string message, ref ExpressionData data)
		{
			// replace escaped quotes
			string value = message.Replace(@"\'", "\'");
			// remove quotes at the beginning and at the end of string
			value = Regex.Replace(value, @"^'", "");
			value = Regex.Replace(value, @"'$", "");
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
			return Expression.Constant(value?[0], typeof(char));
		}
	}
}
