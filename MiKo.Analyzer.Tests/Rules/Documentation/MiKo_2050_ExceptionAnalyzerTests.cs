using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2050_ExceptionAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_documented_exception() => No_issue_is_reported_for(@"
using System;
using System.Runtime.Serialization;

/// <summary>
/// The exception that is thrown ...
/// </summary>
[Serializable]
public sealed class BlaBlaException : Exception
{
    /// <overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref=""BlaBlaException""/> class.
    /// </summary>
    /// </overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref=""BlaBlaException""/> class.
    /// </summary>
    public BlaBlaException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref=""BlaBlaException""/> class with a specified error message.
    /// </summary>
    /// <param name=""message"">
    /// The error message that explains the reason for the exception.
    /// </param>
    public BlaBlaException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref=""BlaBlaException""/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name=""message"">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name=""innerException"">
    /// The exception that is the cause of the current exception.
    /// <para />
    /// If the <paramref name=""innerException"" /> parameter is not <see langword=""null""/>, the current exception is raised in a <b>catch</b> block that handles the inner exception.
    /// </param>
    public BlaBlaException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref=""BlaBlaException""/> class with serialized data.
    /// </summary>
    /// <param name=""info"">
    /// The object that holds the serialized object data.
    /// </param>
    /// <param name=""context"">
    /// The contextual information about the source or destination.
    /// </param>
    /// <remarks>
    /// This constructor is invoked during deserialization to reconstitute the exception object transmitted over a stream.
    /// </remarks>
    private BlaBlaException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}");

        protected override string GetDiagnosticId() => MiKo_2050_ExceptionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2050_ExceptionAnalyzer();
    }
}