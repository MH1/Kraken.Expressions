using System;

namespace Kraken.Expressions
{
	/// <summary>
	/// Temporary structure to store the pre-compilation result.
	/// </summary>
	public class ExpressionData
	{
		/// <summary>
		/// Result type to cast.
		/// </summary>
		public Type TypeToCast { get; internal set; }
	}
}