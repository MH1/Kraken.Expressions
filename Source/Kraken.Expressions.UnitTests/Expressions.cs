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
		/// Test special characters.
		/// </summary>
		[Test]
		public void EvaluateCharConstant1()
		{
			EvaluationContext context = new EvaluationContext();
			char v = context.EvaluateExpression<char>(@"'\''");
			Assert.AreEqual('\'', v);
			v = context.EvaluateExpression<char>(@"'\a'");
			Assert.AreEqual('\a', v);
			v = context.EvaluateExpression<char>(@"'\b'");
			Assert.AreEqual('\b', v);
			v = context.EvaluateExpression<char>(@"'\f'");
			Assert.AreEqual('\f', v);
			v = context.EvaluateExpression<char>(@"'\n'");
			Assert.AreEqual('\n', v);
			v = context.EvaluateExpression<char>(@"'\r'");
			Assert.AreEqual('\r', v);
			v = context.EvaluateExpression<char>(@"'\t'");
			Assert.AreEqual('\t', v);
			v = context.EvaluateExpression<char>(@"'\v'");
			Assert.AreEqual('\v', v);
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
