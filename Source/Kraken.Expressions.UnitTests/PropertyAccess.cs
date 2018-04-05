using System;
using NUnit.Framework;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Kraken.Expressions.UnitTests
{
	/// <summary>
	/// Test properties which cannot be included in unit test.
	/// </summary>
	[TestFixture]
	public class PropertyAccess
	{
		private static readonly object obj1 = new { par1 = 12, par2 = 13.2m, par3 = new { a = 34, par4 = new { b = 56 } } };

		/// <summary>
		/// Test property access.
		/// </summary>
		[Test]
		public void EvaluateProperty1()
		{
			EvaluationContext context = new EvaluationContext();
			decimal v = context.EvaluateExpression<int>("par1", obj1);
			Assert.AreEqual(12, v);
		}

		/// <summary>
		/// Test property access.
		/// </summary>
		[Test]
		public void EvaluateProperty2()
		{
			EvaluationContext context = new EvaluationContext();
			decimal v = context.EvaluateExpression<decimal>("par2", obj1);
			Assert.AreEqual(13.2m, v);
		}

		/// <summary>
		/// Test property access.
		/// </summary>
		[Test]
		public void EvaluateProperty3()
		{
			EvaluationContext context = new EvaluationContext();
			int v = context.EvaluateExpression<int>("par3.a", obj1);
			Assert.AreEqual(34, v);
		}

		/// <summary>
		/// Test property access.
		/// </summary>
		[Test]
		public void EvaluateProperty4()
		{
			EvaluationContext context = new EvaluationContext();
			int v = context.EvaluateExpression<int>("par3.par4.b", obj1);
			Assert.AreEqual(56, v);
		}

		/// <summary>
		/// Expression with static property and enum in System namespace
		/// </summary>
		[Test]
		public void EvaluateProperty5()
		{
			EvaluationContext context = new EvaluationContext();
			DayOfWeek v = context.EvaluateExpression<DayOfWeek>("DateTime.Today.DayOfWeek");
			Assert.AreEqual(DateTime.Today.DayOfWeek, v);
			v = context.EvaluateExpression<DayOfWeek>("System.DateTime.Today.DayOfWeek");
			Assert.AreEqual(DateTime.Today.DayOfWeek, v);
		}

		/// <summary>
		/// Expression with static property and enum in System namespace
		/// </summary>
		[Test]
		public void EvaluateProperty6()
		{
			EvaluationContext context = new EvaluationContext();
			bool v = context.EvaluateExpression<bool>("DateTime.Today.DayOfWeek == DayOfWeek.Friday");
			Assert.AreEqual(DateTime.Today.DayOfWeek == DayOfWeek.Friday, v);
		}
	}
}
