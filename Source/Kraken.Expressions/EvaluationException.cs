using System;
using System.Runtime.Serialization;

namespace Kraken.Expressions
{
	/// <summary>
	/// Represents errors that occur during expression evaluation.
	/// </summary>
#if !NETSTANDARD1_6
	[Serializable]
#endif
	public class EvaluationException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EvaluationException"/> class.
		/// </summary>
		public EvaluationException() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EvaluationException"/> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public EvaluationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EvaluationException"/> class with a specified error message and
		/// a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception,
		/// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public EvaluationException(string message, Exception innerException) : base(message, innerException)
		{
		}

#if !NETSTANDARD1_6
		/// <summary>
		/// Initializes a new instance of the <see cref="EvaluationException"/> class with serialized data.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized
		/// object data about the exception being thrown.</param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information
		/// about the source or destination.</param>
		protected EvaluationException(SerializationInfo info, StreamingContext context) :base(info, context)
		{
		}
#endif

		/// <summary>
		/// The source expression that is the cause of this exception.
		/// </summary>
		public string SourceExpression
		{
			get => Data[nameof(SourceExpression)] as string;
			set => Data[nameof(SourceExpression)] = value;
		}

		/// <summary>
		/// The position at which this exception has occured.
		/// </summary>
		public int? Position
		{
			get => Data[nameof(Position)] as int?;
			set => Data[nameof(Position)] = value;
		}
	}
}
