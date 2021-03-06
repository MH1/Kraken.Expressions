﻿using System;
using System.Runtime.Serialization;

namespace Kraken.Expressions.Tests
{
	/// <summary>
	/// Represents errors that occur during unit tests.
	/// </summary>
	[Serializable]
	internal class TestFailedException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TestFailedException"/> class.
		/// </summary>
		/// <param name="line">The number of line in source file.</param>
		public TestFailedException(long? line = null) : base()
		{
			Line = line;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TestFailedException"/> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="line">The number of line in source file.</param>
		public TestFailedException(string message, long? line = null) : base(message)
		{
			Line = line;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EvaluationException"/> class with a specified error message and
		/// a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception,
		/// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		/// <param name="line">The number of line in source file.</param>
		public TestFailedException(string message, Exception innerException, long? line = null) : base(message, innerException)
		{
			Line = line;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TestFailedException"/> class with serialized data.
		/// </summary>
		/// <param name="info">
		/// The System.Runtime.Serialization.SerializationInfo that holds the serialized
		/// object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The System.Runtime.Serialization.StreamingContext that contains contextual information
		/// about the source or destination.
		/// </param>
		/// <param name="line">The number of line in source file.</param>
		protected TestFailedException(SerializationInfo info, StreamingContext context, long? line = null) : base(info, context)
		{
			Line = line;
		}

		public long? Line
		{
			get => Data[nameof(Line)] as long?;
			set => Data[nameof(Line)] = value;
		}
	}
}