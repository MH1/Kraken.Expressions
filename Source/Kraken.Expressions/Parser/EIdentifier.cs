using Kraken.Expressions.NetStandard;
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

			string[] me = message.Split(new[] { '.' }, 2);

			// parameter first
			Expression exp = context.ParamListExpressions.FirstOrDefault(p => p.Name == me[0]);
			Type t = null;
			string rest = null;

			// found the parameter
			if (exp != null)
			{
				rest = me.Length > 1 ? me[1] : null;
				if (string.IsNullOrEmpty(rest))
					return exp;
			}

			// check the defined type aliases
			if (exp == null && context.TypeAliases.ContainsKey(me[0]))
			{
				t = context.TypeAliases[me[0]];
				rest = me.Length > 1 ? me[1] : null;
				//MemberInfo[] mi = t.GetMember(me[1]);
				//return Expression.MakeMemberAccess(null, me[0]);
			}

			if (exp == null && t == null)
			{
				Assembly[] asm = Reflect.GetAssemblies();
				// find class in included namespaces
				me = message.Split(new[] { '.' });
				foreach (var ns in new[] { string.Empty }.Union(context.Namespaces))
				{
					// search nested namespaces
					for (int i = 0; i < me.Length && t == null; i++)
					{
						Type t0 = asm.Select(o => o.GetType($"{ns}.{string.Join(".", me.Take(i + 1).ToArray())}", false))
							.FirstOrDefault(o => o != null);
						if (t0 != null)
						{
							t = t0;
							rest = string.Join(".", me.Skip(i + 1).ToArray());
						}
					}
					if (t != null)
						break;
				}
			}

			if (exp == null && t == null)
				throw new EvaluationException($"Unable to evaluate {message}");

			if (string.IsNullOrEmpty(rest))
			{
				// TODO type
			}
			else
			{
				me = rest.Split(new[] { '.' });
				Type t2;
				foreach (string m in me)
				{
					t = t ?? exp?.Type;
					if (exp == null || t == null)
					{
						MemberInfo[] mi = t.GetMember(m);
						if (me.Length > 0)
						{
							exp = Expression.MakeMemberAccess(exp, mi[0]);
							t = t2 = exp.Type;
							continue;
						}
					}

#if FEATURE_TYPE_INFO
					t2 = t.GetTypeInfo().GetProperty(m)?.PropertyType;
#else
					t2 = t.GetProperty(m)?.PropertyType;
#endif
					if (t == null)
					{
						throw new EvaluationException($"Unable to find property {m} on type {t.Name}.");
					}

#if FEATURE_TYPE_INFO
					if (exp.Type.GetTypeInfo().IsAssignableFrom((Type)null))
#else
					if (exp.Type.IsAssignableFrom((Type)null))
#endif
					{
						// checked
						exp = Expression.Coalesce(
							Expression.PropertyOrField(exp, m),
							Expression.Constant(null, exp.Type));
					}
					else
					{
						// unchecked
						exp = Expression.PropertyOrField(
							exp,
							m);
					}
				}
			}

			if (exp != null)
				return exp;

			if (t == null)
				throw new EvaluationException($"Unable to evaluate {rest} on type {t.FullName}");

			return null;
		}
	}
}
