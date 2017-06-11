namespace Kraken.Expressions
{
	/// <summary>
	/// Internal parser constants.
	/// </summary>
	internal static class Constants
	{
		#region Regular expressions

		internal const string REGEX_DateTime = @"#(?:[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}|[0-9]{2}/[0-9]{1,2}/[0-9]{1,2}|[0-9]{4}-[0-9]{1,2}-[0-9]{1,2}|[0-9]{2}-[0-9]{1,2}-[0-9]{1,2}|[0-9]{1,2}.[0-9]{1,2}.[0-9]{4}|[0-9]{1,2}.[0-9]{1,2}.[0-9]{2})(?:[\s\xa0]+[0-9]{1,2}\:[0-9]{1,2}(?:\:[0-9]{1,2})?)?#";
		internal const string REGEX_Number = @"[+-]?(?:[0-9]*\.[0-9]+(e\+[0-9]+)?[mf]?|[0-9]+[lmf]?)";
		internal const string REGEX_String = @"""(?:\\.|[^""]|"""")*""";
		internal const string REGEX_Identifier =   @"[a-zA-Z_][a-zA-Z0-9_]*(?:\.[a-zA-Z_][a-zA-Z0-9_]*)*";
		internal const string REGEX_Call = @"[a-zA-Z_][a-zA-Z0-9_]*(?:\.[a-zA-Z_][a-zA-Z0-9_]*)*\(";

		#endregion
	}
}
