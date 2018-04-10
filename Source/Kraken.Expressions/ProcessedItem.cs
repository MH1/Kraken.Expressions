using Kraken.Expressions.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kraken.Expressions
{
	/// <summary>
	/// The result expression part. The pre-compiled source.
	/// </summary>
	public class ProcessedItem
	{
		/// <summary>
		/// Create the instance of <see cref="ProcessedItem"/> identified by parser.
		/// </summary>
		/// <param name="itemType"><see cref="ExpressionPart"/> type of the block.</param>
		/// <param name="content">String part of the expression.</param>
		public ProcessedItem(Type itemType, string content)
		{
			this.ItemType = itemType;
			this.Content = content;
		}

		/// <summary>
		/// Create the instance of <see cref="ProcessedItem"/> with a compiled part of the expression.
		/// </summary>
		/// <param name="expression">Expression</param>
		public ProcessedItem(Expression expression)
		{
			this.Expression = expression;
		}

		/// <summary>
		/// Create the instance of <see cref="ProcessedItem"/> with a compiled part of type cast.
		/// </summary>
		/// <param name="itemType"><see cref="ExpressionPart"/> type of the block.</param>
		/// <param name="typeToCast">Type to cast</param>
		public ProcessedItem(Type itemType, Type typeToCast)
		{
			this.ItemType = itemType;
			this.TypeToCast = typeToCast;
		}

		/// <summary>
		/// <see cref="ExpressionPart"/> type of block.
		/// </summary>
		public Type ItemType { get; private set; }

		/// <summary>
		/// String part of the expression.
		/// </summary>
		public string Content { get; private set; }

		/// <summary>
		/// The compiled part of the expression.
		/// </summary>
		public Expression Expression { get; private set; }

		/// <summary>
		/// Type cast
		/// </summary>
		public Type TypeToCast { get; private set; }


		/// <summary>
		/// Get the instance of <see cref="ProcessedItem"/> identified by parser.
		/// </summary>
		/// <param name="itemType"><see cref="ExpressionPart"/> type of the block.</param>
		/// <param name="content">String part of the expression.</param>
		/// <returns>Instance of the <see cref="ProcessedItem"/></returns>
		public static ProcessedItem Create(Type itemType, string content)
		{
			return new ProcessedItem(itemType, content);
		}

		/// <summary>
		/// Get the instance of <see cref="ProcessedItem"/> with a compiled part of the expression.
		/// </summary>
		/// <param name="expression">Expression</param>
		/// <returns>Instance of the <see cref="ProcessedItem"/></returns>
		public static ProcessedItem Create(Expression expression)
		{
			return new ProcessedItem(expression);
		}

		/// <summary>
		/// Get the instance of <see cref="ProcessedItem"/> with a type to cast.
		/// </summary>
		/// <param name="typeToCast">Type to cast</param>
		/// <returns>Instance of the <see cref="ProcessedItem"/></returns>
		public static ProcessedItem CreateCast(Type typeToCast)
		{
			return new ProcessedItem(typeof(ETypeCast), typeToCast);
		}

		/// <summary>
		/// Sub-items defined by the brackets.
		/// </summary>
		internal IList<ProcessedItem> Items { get; set; } = new List<ProcessedItem>();

		/// <summary>
		/// Sub-items defined by the brackets.
		/// Indexed access for the <see cref="Items"/>.
		/// </summary>
		/// <param name="index">Index of the sub-item</param>
		/// <returns><see cref="ProcessedItem"/></returns>
		internal ProcessedItem this[int index]
		{
			get => Items[index];
			set => Items[index] = value;
		}

#if DEBUG
		/// <summary>
		/// The overrided method which returns the string which describes the expression part content.
		/// </summary>
		/// <returns>The string which describes the expression part content</returns>
		public override string ToString()
		{
			return Items.Count > 0 ?
				// print the list of sub-items
				string.Join(" ", Items
#if NET35
				.Select(o => o.ToString()).ToArray()
#endif
				) :
				Expression != null ?
				// print the compiled expression
				$"{{{Expression}}}" :
				// default
				base.ToString();
		}
#endif

	}
}
