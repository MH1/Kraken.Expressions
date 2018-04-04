using System;

namespace Kraken.Expressions.Parser
{
	/// <summary>
	/// The <see cref="ExpressionPart"/> implementation of members.
	/// </summary>
	public class EMemberAccess : ExpressionPart
	{
		/// <summary>
		/// The list of <see cref="ExpressionPart"/> types which follows after the member.
		/// </summary>
		public override Type[] ExpectedParts => new[]
		{
			typeof(EExpressionEnd),
			typeof(EBinaryOperator),
			typeof(EConditionalOperator),
		};

		/// <summary>
		/// The regular expression to match the member.
		/// </summary>
		public override string Regexp => @"\." + Constants.REGEX_Identifier;
	}
}
