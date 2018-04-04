using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Kraken.Expressions.Parser
{
	/// <summary>
	/// The <see cref="ExpressionPart"/> implementation of the identifiers (parameters and variables).
	/// </summary>
	public class EIdentifier : ExpressionPart
	{
		/// <summary>
		/// The list of <see cref="ExpressionPart"/> types which follows after the identifier.
		/// </summary>
		public override Type[] ExpectedParts => new[]
		{
			typeof(EExpressionEnd),
			typeof(EBinaryOperator),
			typeof(EConditionalOperator),
		};

		/// <summary>
		/// The regular expression to match the identifier.
		/// </summary>
		public override string Regexp => Constants.REGEX_Identifier;

		/// <summary>
		/// Method to convert the identifier from string to <see cref="Expression"/>.
		/// </summary>
		/// <param name="context">The context of evaluation</param>
		/// <param name="message">The message to convert</param>
		/// <returns>The result expression</returns>
		public override Expression GetExpression(EvaluationContext context, string message)
		{
			string valObj = message.ToLowerInvariant();
			if (valObj == "true")
				return Expression.Constant(true, typeof(bool));
			if (valObj == "false")
				return Expression.Constant(false, typeof(bool));

			string[] mem = message.Split(new[] { '.' }, 2);
			if (context.TypeAliases.ContainsKey(mem[0]))
			{
				Type t = context.TypeAliases[mem[0]];
				MemberInfo[] me = t.GetMember(mem[1]);
				return Expression.MakeMemberAccess(null, me[0]);
			}
			else
			{
				foreach (string ns in context.Namespaces)
				{
					Assembly[] asm;
#if NETSTANDARD1_6
					asm = NetStandard.AppDomain.CurrentDomain.GetAssemblies();
#else
					asm = AppDomain.CurrentDomain.GetAssemblies();
#endif
					Type t = asm.Select(o => o.GetType($"{ns}.{mem[0]}", false))
						.FirstOrDefault(o => o != null);
					if (t != null)
					{
						MemberInfo[] me = t.GetMember(mem[1]);
						return Expression.MakeMemberAccess(null, me[0]);
					}
				}
			}
			return TranslateMemberAccess(context, message);
		}

		private Expression TranslateMemberAccess(EvaluationContext context, string expression)
		{
			MatchCollection m = Regex.Matches(expression, "^" + Constants.REGEX_Identifier, RegexOptions.IgnoreCase);
			string[] ma = m[0].Value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

			Expression exp = context.ParamListExpressions.FirstOrDefault(p => p.Name == ma[0]);
			if (exp == null)
			{
				throw new EvaluationException($"Unable to evaluate identifier {ma[0]}");
			}
			Type t = exp.Type, t2 = exp.Type;
			for (int i = 1; i < ma.Length; i++)
			{
				if (string.IsNullOrEmpty(ma[i]))
					continue;
#if FEATURE_TYPE_INFO
				t2 = t.GetTypeInfo().GetProperty(ma[i])?.PropertyType;
#else
				t2 = t.GetProperty(ma[i])?.PropertyType;
#endif
				if (t == null)
				{
					throw new EvaluationException($"Unable to find property {ma[i]} on type {t.Name}.");
				}

#if FEATURE_TYPE_INFO
				if (exp.Type.GetTypeInfo().IsAssignableFrom((Type)null))
#else
				if (exp.Type.IsAssignableFrom((Type)null))
#endif
				{
					// checked
					exp = Expression.Coalesce(
						Expression.PropertyOrField(exp, ma[i]),
						Expression.Constant(null, exp.Type));
				}
				else
				{
					// unchecked
					exp = Expression.PropertyOrField(
						exp,
						ma[i]);
				}
			}
			//exp = Expression.Convert(exp, exp.Type);
			return exp;
		}
	}
}
