using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using System.Linq.Expressions;
using System.Diagnostics;

namespace Kraken.Expressions.Tests
{
	/// <summary>
	/// Performance tests.
	/// </summary>
	[TestFixture]
	public class PerfTest
	{
		private const long MAX_I = 1000;
		private const long MAX_J = 1000;

		/// <summary>
		/// Performance test.
		/// </summary>
		[Test]
		public void PerformanceTest1()
		{
			EvaluationContext context = new EvaluationContext();
			context.AddParameters(new[] { "x", "y" }, new[] { typeof(int), typeof(int) });
			//context.ParseParameters(new { x = 1, y = 1 });
			var expr1 = context.Precompile<int, int>("(x ^ 2) % y");
			var expr2 = context.Precompile<int, int>("y % x");
			var expr3 = context.Precompile<int, int>("x % y");

			Stopwatch sw = Stopwatch.StartNew();
			for (int j = 1; j < MAX_J; j++)
				for (int i = 1; i < MAX_I; i++)
				{
					var a = expr1.Invoke(i, j);
					var b = expr2.Invoke(i, j);
					var c = expr3.Invoke(i, j);
				}
			sw.Stop();

			double result = (MAX_J * MAX_I * 3) / sw.Elapsed.TotalMilliseconds * 1000;
		}
	}
}
