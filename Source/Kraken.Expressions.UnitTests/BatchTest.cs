using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Reflection;
using NUnit.Framework;
using Kraken.Expressions;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Kraken.Expressions.UnitTests
{
	/// <summary>
	/// Unit tests.
	/// </summary>
	[TestFixture]
	public class BatchTest
	{
		/// <summary>
		/// File: Default.txt
		/// </summary>
		[Test]
		public void DefaultBatch()
			=> TestSingleFile("Default.txt");

		/// <summary>
		/// Test all files in Batch folder.
		/// </summary>
		[Test]
		public void TestAllBatches()
		{
			string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			foreach (string file in Directory.GetFiles(Path.Combine(dir, "Batch"), "*.txt"))
			{
				TestFile(file);
			}
		}

		private static void TestSingleFile(string file)
		{
			string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			string filePath = Path.Combine(dir, "Batch", file);
			if (File.Exists(filePath))
				TestFile(filePath);
		}

		private static void TestFile(string file)
		{
			using (var stream = new FileStream(file, FileMode.Open))
			{
				using (TextReader tx = new StreamReader(stream, Encoding.Default))
				{
					string line;
					long num = 0;
					while ((line = tx.ReadLine()) != null)
					{
						num++;
						if (string.IsNullOrWhiteSpace(line)
							|| line.TrimStart().StartsWith(";"))
							continue;
						try
						{
							Test(line);
						}
						catch (EvaluationException e)
						{
							if (e.Position != null)
								throw new TestFailedException($"Test {Path.GetFileNameWithoutExtension(file)} failed at line {num}, position {e.Position}\n{line}", e, num);
							else
								throw new TestFailedException($"Test {Path.GetFileNameWithoutExtension(file)} failed at line {num}\n{line}", e, num);
						}
						catch (Exception e)
						{
							throw new TestFailedException($"Test {Path.GetFileNameWithoutExtension(file)} failed at line {num}\n{line}", e, num);
						}
					}
				}
			}
		}

		private static Dictionary<string, Type> Exceptions = new Dictionary<string, Type>
		{
			{ "DivideByZeroException", typeof(DivideByZeroException) },
			{ "OverflowException", typeof(OverflowException) },
			{ "InvalidOperationException", typeof(InvalidOperationException) },
		};

		private static void Test(string line)
		{
			string[] l = line.Split(new[] { ';' }, 2);

			l[0] = l[0].Trim();
			Type t;
			EvaluationContext context = new EvaluationContext();
			if (context.TypeAliases.ContainsKey(l[0].ToLowerInvariant()))
			{
				t = context.TypeAliases[l[0].ToLowerInvariant()];
			}
			else
			{
				Assembly asm = typeof(int).Assembly;
				string testType = l[0][0].ToString().ToUpperInvariant() + l[0].Substring(1);
				if (!testType.StartsWith("System."))
					testType = "System." + testType;
				t = !testType.StartsWith("System.")
					? (asm.GetType("System." + testType) ?? asm.GetType(testType))
					: asm.GetType(testType);
				if (t == null)
					throw new Exception($"Type '{testType}' not found");
			}
			line = l[1];

			l = line.Split(new[] { ';' }, 2);
			object result;
			try
			{
				result = context.EvaluateExpression(l[1]);
				object expectedResult = ParseResult(t, l[0]);
				Assert.AreEqual(expectedResult, result);
			}
			catch (Exception e)
			{
				if (!Exceptions.ContainsKey(l[0]) || e.GetType() != Exceptions[l[0]])
				{
					throw;
				}
			}
		}

		private static readonly string[] DateFormats = new[]
		{
			"yyyy-MM-dd",
			"yyyy-MM-dd H:mm",
			"yyyy-MM-dd H:mm:ss",
		};

		private static object ParseResult(Type type, string value)
		{
			if (Exceptions.ContainsKey(value))
				return Exceptions[value];
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Boolean:
					if (!type.IsEnum)
						return bool.Parse(value);
					return Enum.Parse(type, value, true);
				case TypeCode.Byte:
					if (!type.IsEnum)
						return byte.Parse(value, CultureInfo.InvariantCulture);
					return Enum.Parse(type, value, true);
				case TypeCode.Char:
					return char.Parse(value);
				case TypeCode.DateTime:
					return DateTime.ParseExact(value, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);
				case TypeCode.Decimal:
					return decimal.Parse(value, CultureInfo.InvariantCulture);
				case TypeCode.Double:
					return double.Parse(value, CultureInfo.InvariantCulture);
				case TypeCode.Int16:
					if (!type.IsEnum)
						return short.Parse(value, CultureInfo.InvariantCulture);
					return Enum.Parse(type, value, true);
				case TypeCode.Int32:
					if (!type.IsEnum)
						return int.Parse(value, CultureInfo.InvariantCulture);
					return Enum.Parse(type, value, true);
				case TypeCode.Int64:
					if (!type.IsEnum)
						return long.Parse(value, CultureInfo.InvariantCulture);
					return Enum.Parse(type, value, true);
				case TypeCode.SByte:
					return sbyte.Parse(value, CultureInfo.InvariantCulture);
				case TypeCode.Single:
					return Single.Parse(value, CultureInfo.InvariantCulture);
				case TypeCode.String:
					return value;
				case TypeCode.UInt16:
					if (!type.IsEnum)
						return ushort.Parse(value, CultureInfo.InvariantCulture);
					return Enum.Parse(type, value, true);
				case TypeCode.UInt32:
					if (!type.IsEnum)
						return uint.Parse(value, CultureInfo.InvariantCulture);
					return Enum.Parse(type, value, true);
				case TypeCode.UInt64:
					if (!type.IsEnum)
						return ulong.Parse(value, CultureInfo.InvariantCulture);
					return Enum.Parse(type, value, true);
				default:
					if (type == typeof(float))
						return float.Parse(value, CultureInfo.InvariantCulture);
					if (type == typeof(System.Single))
						return Single.Parse(value, CultureInfo.InvariantCulture);
					throw new InvalidOperationException("Undefined type");
			}
		}
	}
}
