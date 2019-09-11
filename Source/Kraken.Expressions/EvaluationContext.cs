using Kraken.Expressions.Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kraken.Expressions
{
	/// <summary>
	/// The context of evaluation. Defines the parser engine, compiler, environment...
	/// </summary>
	public class EvaluationContext
	{
		/// <summary>
		/// The evaluation engine.
		/// </summary>
		public readonly EvaluationEngine Engine;

		/// <summary>
		/// Processing parser method with the entry <see cref="ExpressionPart"/>.
		/// </summary>
		public Processing<ExpressionBlock> Parser = new Processing<ExpressionBlock>();

		/// <summary>
		/// Create the instance of <see cref="EvaluationContext"/> with default <see cref="EvaluationEngine"/>.
		/// </summary>
		public EvaluationContext() : this(new EvaluationEngine()) { }

		/// <summary>
		/// Create the instance of <see cref="EvaluationContext"/> with custom <see cref="EvaluationEngine"/>.
		/// </summary>
		/// <param name="engine">Custom <see cref="EvaluationEngine"/>.</param>
		public EvaluationContext(EvaluationEngine engine)
		{
			this.Engine = engine;
		}

		/// <summary>
		/// The type namespaces where to search types.
		/// </summary>
		public string[] Namespaces = new[] { "System" };

		/// <summary>
		/// The aliases for <seealso cref="Type"/>s to translate.
		/// </summary>
		public Dictionary<string, Type> TypeAliases = new Dictionary<string, Type>
		{
			{ "float", typeof(System.Single) },
			{ "bool", typeof(System.Boolean) },
			{ "int", typeof(System.Int32) },
			{ "short", typeof(System.Int16) },
			{ "long", typeof(System.Int64) },
			{ "uint", typeof(System.UInt32) },
			{ "ushort", typeof(System.UInt16) },
			{ "ulong", typeof(System.UInt64) },
			{ "sbyte", typeof(System.SByte) },
			{ "byte", typeof(System.Byte) },
		};

		/// <summary>
		/// The ignored characters between <see cref="ExpressionPart"/>s to escape.
		/// </summary>
		public char[] Whitespaces = new[] { ' ', '\t', '\r', '\n', '\xa0' };

		/// <summary>
		/// The signed integer types.
		/// </summary>
		public IList<Type> SignedIntTypes = new List<Type>
		{
			typeof(sbyte),
			typeof(short),
			typeof(int),
			typeof(long),
		};

		/// <summary>
		/// The unsigned integer types.
		/// </summary>
		public IList<Type> UnsignedIntTypes = new List<Type>
		{
			typeof(byte),
			typeof(ushort),
			typeof(uint),
			typeof(ulong),
		};

		/// <summary>
		/// The types with floating point.
		/// </summary>
		public IList<Type> FloatingPointTypes = new List<Type>
		{
			typeof(float),
			typeof(Single),
			typeof(double),
			typeof(decimal),
		};

		//internal static readonly string[] Types = new[]
		//{
		//	"int", "uint",
		//	"long", "ulong",
		//	"short", "ushort",
		//	"byte",
		//	"decimal", "double", "float",
		//	"bool",
		//	"string",
		//	"char",
		//};

		/// <summary>
		/// The binary operators priority.
		/// </summary>
		internal string[][] Priority = new[]
		{
			//new[] { "!", "NOT", "~"  }, // unary operators - EUnaryOperator
			new[] { "*", "/", "%", "MOD" },
			new[] { "+", "-" },
			new[] { "<<", ">>", "SRL", "SHR" },
			new[] { "<", ">", "<=", ">=" },
			new[] { "==", "!=", "<>" },
			new[] { "&" },
			new[] { "^", "XOR" },
			new[] { "|" },
			new[] { "&&", "AND" },
			new[] { "||", "OR" },
			new[] { "??" },
			//new[] { "?", ":" }, // ?: - EConditionalOperator
			// assignment
		};

		/// <summary>
		/// The unary expression translation table.
		/// The ~ is not supported for .NET 3.5.
		/// </summary>
		internal Dictionary<string, ExpressionType> UnaryNodeTypes = new Dictionary<string, ExpressionType>
		{
			{"!", ExpressionType.Not },
			{"NOT", ExpressionType.Not },
#if !NET35
			{"~", ExpressionType.OnesComplement },
#endif
			//{"-", ExpressionType.Negate },
			//{"+", ExpressionType.UnaryPlus },
		};

		/// <summary>
		/// The binary expression translation table.
		/// </summary>
		internal Dictionary<string, ExpressionType> BinaryNodeTypes = new Dictionary<string, ExpressionType>(StringComparer.OrdinalIgnoreCase)
		{
			// logical
			{"|", ExpressionType.Or},
			{"||", ExpressionType.OrElse},
			{"OR", ExpressionType.OrElse},
			{"&", ExpressionType.And},
			{"&&", ExpressionType.AndAlso},
			{"AND", ExpressionType.AndAlso},
			{"^", ExpressionType.ExclusiveOr},
			{"XOR", ExpressionType.ExclusiveOr},

			// comparison
			{"!=", ExpressionType.NotEqual},
			{"<>", ExpressionType.NotEqual},
			{"==", ExpressionType.Equal},
			{">", ExpressionType.GreaterThan},
			{">=", ExpressionType.GreaterThanOrEqual},
			{"<", ExpressionType.LessThan},
			{"<=", ExpressionType.LessThanOrEqual},

			// mathematical functions
			{"+", ExpressionType.Add},
			{"-", ExpressionType.Subtract},
			{"*", ExpressionType.Multiply},
			{"/", ExpressionType.Divide},
			{"%", ExpressionType.Modulo},
			{"MOD", ExpressionType.Modulo},
			{"??", ExpressionType.Coalesce},
			{"<<", ExpressionType.LeftShift},
			{"SHL", ExpressionType.LeftShift},
			{">>", ExpressionType.RightShift},
			{"SHR", ExpressionType.RightShift},

			// mathematical functions with assignment
			//{"+=", ExpressionType.AddAssign},
			////{"+=", ExpressionType.AddAssignChecked },
			//{"-=", ExpressionType.SubtractAssign},
			////{"-=", ExpressionType.SubtractAssignChecked },
			//{"*=", ExpressionType.MultiplyAssign},
			////{"*=", ExpressionType.MultiplyAssignChecked },
			//{"/=", ExpressionType.DivideAssign},
			//{"%=", ExpressionType.ModuloAssign},
			//{"<<=", ExpressionType.LeftShiftAssign},
			//{">>=", ExpressionType.RightShiftAssign},
		};

		/// <summary>
		/// The list of parameter expressions.
		/// </summary>
		public IList<ParameterExpression> ParamListExpressions = new List<ParameterExpression>();

		/// <summary>
		/// The list of parameter content.
		/// </summary>
		public IList<object> ParamListValues = new List<object>();

		/// <summary>
		/// Supported Date/DateTime formats.
		/// </summary>
		public string[] DateTimeFormats =
			new[] { "y/d/M", "y-M-d", "d.M.y" }
				.Join(new[] { "yy", "yyyy" }, o => 1, o => 1, (a, b) => a.Replace("y", b))
				.Join(new[] { "", "H:m", "H:m:s" }, o => 1, o => 1, (a, b) => string.Join(" ", new[] { a, b }).TrimEnd())
				.Select(o => $"#{o}#")
				.ToArray();

		/// <summary>
		/// Evaluate the <paramref name="expression"/> with parameters in parameter object <paramref name="par"/>
		/// and returns the result of evaluation.
		/// </summary>
		/// <param name="expression">Source expression in string.</param>
		/// <param name="par">The parameter object.</param>
		/// <returns>Result value of evaluation</returns>
		public virtual object EvaluateExpression(string expression, object par = null)
		{
			this.ParseParameters(par);
			return this.Engine.EvaluateExpression(this, expression);
		}

		/// <summary>
		/// Evaluate the <paramref name="expression"/> with parameters in parameter object <paramref name="par"/>
		/// and returns the result of evaluation.
		/// </summary>
		/// <typeparam name="T">Type of the result.</typeparam>
		/// <param name="expression">Source expression in string.</param>
		/// <param name="par">The parameter object.</param>
		/// <returns>Result value of evaluation</returns>
		public virtual T EvaluateExpression<T>(string expression, object par = null)
		{
			this.ParseParameters(par);
			return this.Engine.EvaluateExpression<T>(this, expression);
		}

		/// <summary>
		/// Translate <paramref name="expression"/> into the expression.
		/// </summary>
		/// <param name="expression">Source expression in string.</param>
		/// <param name="par">The parameter object.</param>
		/// <returns>The result expression.</returns>
		public virtual Expression TranslateExpression(string expression, object par = null)
		{
			this.ParseParameters(par);
			return this.Engine.TranslateExpression(this, expression);
		}

		/// <summary>
		/// Add parameters definition.
		/// </summary>
		/// <param name="parameterNames">The parameter names.</param>
		/// <param name="parameterTypes">The parameter types.</param>
		public void AddParameters(string[] parameterNames, Type[] parameterTypes)
		{
			if (parameterNames.Length != parameterTypes.Length)
				throw new InvalidOperationException("Parameter count mismatch.");
			for (int i = 0; i < parameterNames.Length; i++)
			{
				this.ParamListExpressions.Add(Expression.Parameter(parameterTypes[i], parameterNames[i]));
			}
		}

		/// <summary>
		/// Parse parameter object <paramref name="par"/> into <seealso cref="ParamListExpressions"/>
		/// and <seealso cref="ParamListValues"/> used by compiler.
		/// </summary>
		/// <param name="par">The parameter object.</param>
		public virtual void ParseParameters(object par)
		{
			if (par == null)
				return;
			this.ParamListExpressions.Clear();
			this.ParamListValues.Clear();
			Type t = par?.GetType();
			if (t != null)
			{
#if NETSTANDARD
				foreach (PropertyInfo prop in t.GetTypeInfo().GetProperties())
#else
				foreach (PropertyInfo prop in t.GetProperties())
#endif
				{
					if (prop.CanRead)
					{
						this.ParamListExpressions.Add(Expression.Parameter(prop.PropertyType, prop.Name));
#if NET40 || NET35
						this.ParamListValues.Add(prop.GetValue(par, null));
#else
						this.ParamListValues.Add(prop.GetValue(par));
#endif
					}
				}
			}
		}

		/// <summary>
		/// Method to consolidate the result type used for the binary expressions.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="t1">The first parameter type</param>
		/// <param name="t2">The second parameter type</param>
		/// <returns>The result type</returns>
		public Type ConsolidateBinaryTypes(EvaluationContext context, Type t1, Type t2)
		{
			if (t1 == t2)
				return t1;

			if (t1.IsAssignableFrom(t2))
				return t1;

			if (t2.IsAssignableFrom(t1))
				return t2;

			if (
#if NETSTANDARD1_6
				t1.GetTypeInfo().IsEnum &&
#else
				t1.IsEnum &&
#endif
				(context.SignedIntTypes.Contains(t2)
				|| context.UnsignedIntTypes.Contains(t2)))
			{
				return ConsolidateBinaryTypes(context, GetBaseTypeFromEnum(t1), t2);
			}

			if (
#if NETSTANDARD1_6
				t2.GetTypeInfo().IsEnum &&
#else
				t2.IsEnum &&
#endif
				(context.SignedIntTypes.Contains(t1)
				|| context.UnsignedIntTypes.Contains(t1)))
			{
				return ConsolidateBinaryTypes(context, t1, GetBaseTypeFromEnum(t2));
			}

			if (context.SignedIntTypes.Contains(t1)
				|| context.UnsignedIntTypes.Contains(t1)
				|| context.FloatingPointTypes.Contains(t1))
				return t1;

			if ((t1 == typeof(string) && t2 == typeof(char)) ||
				(t2 == typeof(string) && t1 == typeof(char)))
				return typeof(string);

			throw new EvaluationException($"Unknown binary operation with {t1.Name} and {t2.Name}");
		}

		private Type GetBaseTypeFromEnum(Type type)
		{
			FieldInfo mi = type.GetField("value__");
			return mi.FieldType;
		}

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression Precompile(string expression)
			=> new PrecompiledExpression(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1> Precompile<T1>(string expression)
			=> new PrecompiledExpression<T1>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2> Precompile<T1, T2>(string expression)
			=> new PrecompiledExpression<T1, T2>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3> Precompile<T1, T2, T3>(string expression)
			=> new PrecompiledExpression<T1, T2, T3>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4> Precompile<T1, T2, T3, T4>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4>(this, expression);
#if !NET35
		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4, T5> Precompile<T1, T2, T3, T4, T5>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4, T5>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4, T5, T6> Precompile<T1, T2, T3, T4, T5, T6>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4, T5, T6>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7> Precompile<T1, T2, T3, T4, T5, T6, T7>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8> Precompile<T1, T2, T3, T4, T5, T6, T7, T8>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9> Precompile<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Precompile<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Precompile<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Precompile<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Precompile<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Precompile<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Precompile<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this, expression);

		/// <summary>
		/// Precompile string expression.
		/// </summary>
		/// <param name="expression">The string expression.</param>
		/// <returns>Precompiled object which can invoke to get the result.</returns>
		public PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Precompile<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string expression)
			=> new PrecompiledExpression<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this, expression);
#endif
	}
}
