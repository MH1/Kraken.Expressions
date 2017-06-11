using System;
using System.Linq.Expressions;

namespace Kraken.Expressions
{
	/// <summary>
	/// The parsed part of the expression.
	/// </summary>
	public interface IExpressionPart
	{
		/// <summary>
		/// Dependent parts.
		/// The list of <see cref="ExpressionPart"/> types which follows after the current one.
		/// </summary>
		Type[] ExpectedParts { get; }

		/// <summary>
		/// Method to get the next part of expression to process with this <see cref="ExpressionPart"/>.
		/// By default it contains the default regular expression parser.
		/// Override this if you want to parse more complicated part of expression.
		/// </summary>
		/// <param name="context">The context of evaluation</param>
		/// <param name="expression">The rest of expression to parse</param>
		/// <returns>The string block to parse or null if the block doesn't match the current <see cref="ExpressionPart"/></returns>
		string Get(EvaluationContext context, ref string expression);

		/// <summary>
		/// Regular expression which represents the current <see cref="ExpressionPart"/>.
		/// If it is not possible to formulate the block by mean the regular expression, leave this with null and override the <seealso cref="Get(EvaluationContext, ref string)"/>
		/// </summary>
		string Regexp { get; }

		/// <summary>
		/// If it is possible to convert part into message, write the code here.
		/// </summary>
		/// <param name="context">The context of evaluation</param>
		/// <param name="message">The message to convert</param>
		/// <returns>The result expression</returns>
		Expression GetExpression(EvaluationContext context, string message);
	}
}
