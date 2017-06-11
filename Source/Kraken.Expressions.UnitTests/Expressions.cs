using System;
using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Kraken.Expressions.UnitTests
{
	/// <summary>
	/// Test expressions which cannot be included in unit test.
	/// </summary>
	[TestFixture]
	public class Expressions
	{
		/// <summary>
		/// Test special characters.
		/// </summary>
		[Test]
		public void EvaluateStringConstant1()
		{
			EvaluationContext context = new EvaluationContext();
			string v = context.EvaluateExpression<string>(@"""\\\""\a\b\f\n\r\t\v""");
			Assert.AreEqual("\\\"\a\b\f\n\r\t\v", v);
		}

		/// <summary>
		/// Expression with parameter.
		/// </summary>
		[Test]
		public void EvaluateExpression1()
		{
			EvaluationContext context = new EvaluationContext();
			int v = context.EvaluateExpression<int>("1 + a", new { a = 4 });
			Assert.AreEqual(5, v);
		}
	}
}
