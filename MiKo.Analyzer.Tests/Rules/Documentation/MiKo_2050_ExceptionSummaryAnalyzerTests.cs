﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2050_ExceptionSummaryAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] StartingPhrases = [.. CreatePhrases().Except(Constants.Comments.ExceptionTypeSummaryStartingPhrase.Trim())];

        [Test]
        public void No_issue_is_reported_for_non_exception_class() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            /// <summary>
            /// Something...
            /// </summary>
            [Serializable]
            public sealed class BlaBla
            {
                /// <overloads>
                /// <summary>
                /// Ctor.
                /// </summary>
                /// </overloads>
                /// <summary>
                /// Ctor.
                /// </summary>
                public BlaBla()
                {
                }

                /// <summary>
                /// Ctor.
                /// </summary>
                /// <param name="message">
                /// whatever
                /// </param>
                public BlaBla(string message)
                    : base(message)
                {
                }

                /// <summary>
                /// Ctor.
                /// </summary>
                /// <param name="message">
                /// whatever
                /// </param>
                /// <param name="innerException">
                /// Some stuff.
                /// </param>
                public BlaBla(string message, Exception innerException) : base(message, innerException)
                {
                }

                /// <summary>
                /// Ctor.
                /// </summary>
                /// <param name="info">
                /// p1
                /// </param>
                /// <param name="context">
                /// p2
                /// </param>
                /// <remarks>
                /// Unimportant.
                /// </remarks>
                private BlaBla(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
            }
            """);

        [Test]
        public void No_issue_is_reported_for_correctly_documented_exception() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            /// <summary>
            /// The exception that is thrown when ...
            /// </summary>
            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <overloads>
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class.
                /// </summary>
                /// </overloads>
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class.
                /// </summary>
                public BlaBlaException()
                {
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message.
                /// </summary>
                /// <param name="message">
                /// The error message that explains the reason for the exception.
                /// </param>
                public BlaBlaException(string message)
                    : base(message)
                {
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
                /// </summary>
                /// <param name="message">
                /// The error message that explains the reason for the exception.
                /// </param>
                /// <param name="innerException">
                /// The exception that is the cause of the current exception.
                /// <para />
                /// If the <paramref name="innerException" /> parameter is not <see langword="null"/>, the current exception is raised in a <b>catch</b> block that handles the inner exception.
                /// </param>
                public BlaBlaException(string message, Exception innerException) : base(message, innerException)
                {
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with serialized data.
                /// </summary>
                /// <param name="info">
                /// The object that holds the serialized object data.
                /// </param>
                /// <param name="context">
                /// The contextual information about the source or destination.
                /// </param>
                /// <remarks>
                /// This constructor is invoked during deserialization to reconstitute the exception object transmitted over a stream.
                /// </remarks>
                private BlaBlaException(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
            }
            """);

        [Test]
        public void No_issue_is_reported_for_correctly_documented_exception_type() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            /// <summary>
            /// The exception that is thrown when ...
            /// </summary>
            [Serializable]
            public sealed class BlaBlaException : Exception
            {
            }
            """);

        [Test]
        public void No_issue_is_reported_for_non_documented_exception_type() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
            }
            """);

        [Test]
        public void No_issue_is_reported_for_correctly_documented_exception_ctor_without_params() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <overloads>
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class.
                /// </summary>
                /// </overloads>
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class.
                /// </summary>
                public BlaBlaException()
                {
                }
            }
            """);

        [Test]
        public void No_issue_is_reported_for_non_documented_exception_ctor_without_params() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                public BlaBlaException()
                {
                }
            }
            """);

        [Test]
        public void No_issue_is_reported_for_correctly_documented_but_missing_overloads_exception_ctor_without_params() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class.
                /// </summary>
                public BlaBlaException()
                {
                }
            }
            """);

        [Test]
        public void No_issue_is_reported_for_correctly_documented_exception_ctor_with_message_only_param() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message.
                /// </summary>
                /// <param name="message">
                /// The error message that explains the reason for the exception.
                /// </param>
                public BlaBlaException(string message)
                    : base(message)
                {
                }
            }
            """);

        [Test]
        public void No_issue_is_reported_for_non_documented_exception_ctor_with_message_only_param() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                public BlaBlaException(string message)
                    : base(message)
                {
                }
            }
            """);

        [Test]
        public void No_issue_is_reported_for_correctly_documented_exception_ctor_with_message_and_exception_param() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
                /// </summary>
                /// <param name="message">
                /// The error message that explains the reason for the exception.
                /// </param>
                /// <param name="innerException">
                /// The exception that is the cause of the current exception.
                /// <para />
                /// If the <paramref name="innerException" /> parameter is not <see langword="null"/>, the current exception is raised in a <b>catch</b> block that handles the inner exception.
                /// </param>
                public BlaBlaException(string message, Exception innerException) : base(message, innerException)
                {
                }
            }
            """);

        [Test]
        public void No_issue_is_reported_for_non_documented_exception_ctor_with_message_and_exception_param() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                public BlaBlaException(string message, Exception innerException) : base(message, innerException)
                {
                }
            }
            """);

        [Test]
        public void No_issue_is_reported_for_correctly_documented_exception_ctor_with_serialization_param() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with serialized data.
                /// </summary>
                /// <param name="info">
                /// The object that holds the serialized object data.
                /// </param>
                /// <param name="context">
                /// The contextual information about the source or destination.
                /// </param>
                /// <remarks>
                /// This constructor is invoked during deserialization to reconstitute the exception object transmitted over a stream.
                /// </remarks>
                private BlaBlaException(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
            }
            """);

        [Test]
        public void No_issue_is_reported_for_non_documented_exception_ctor_with_serialization_param() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                private BlaBlaException(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
            }
            """);

        [Test]
        public void No_issue_is_reported_for_correctly_documented_but_missing_remarks_exception_ctor_with_serialization_param() => No_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with serialized data.
                /// </summary>
                /// <param name="info">
                /// The object that holds the serialized object data.
                /// </param>
                /// <param name="context">
                /// The contextual information about the source or destination.
                /// </param>
                private BlaBlaException(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
            }
            """);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_exception_type() => An_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            /// <summary>
            /// Thrown ...
            /// </summary>
            [Serializable]
            public sealed class BlaBlaException : Exception
            {
            }
            """);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_overloads_on_exception_ctor_without_params() => An_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <overloads>
                /// <summary>
                /// Some overload.
                /// </summary>
                /// </overloads>
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class.
                /// </summary>
                public BlaBlaException()
                {
                }
            }
            """);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_summary_of_exception_ctor_with_message_only_param() => An_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class.
                /// </summary>
                /// <param name="message">
                /// The error message that explains the reason for the exception.
                /// </param>
                public BlaBlaException(string message)
                    : base(message)
                {
                }
            }
            """);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_message_param_of_exception_ctor_with_message_only_param() => An_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message.
                /// </summary>
                /// <param name="message">
                /// The error message.
                /// </param>
                public BlaBlaException(string message)
                    : base(message)
                {
                }
            }
            """);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_summary_of_exception_ctor_with_message_and_exception_param() => An_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class.
                /// </summary>
                /// <param name="message">
                /// The error message that explains the reason for the exception.
                /// </param>
                /// <param name="innerException">
                /// The exception that is the cause of the current exception.
                /// <para />
                /// If the <paramref name="innerException" /> parameter is not <see langword="null"/>, the current exception is raised in a <b>catch</b> block that handles the inner exception.
                /// </param>
                public BlaBlaException(string message, Exception innerException) : base(message, innerException)
                {
                }
            }
            """);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_message_param_of_exception_ctor_with_message_and_exception_param() => An_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
                /// </summary>
                /// <param name="message">
                /// The error message.
                /// </param>
                /// <param name="innerException">
                /// The exception that is the cause of the current exception.
                /// <para />
                /// If the <paramref name="innerException" /> parameter is not <see langword="null"/>, the current exception is raised in a <b>catch</b> block that handles the inner exception.
                /// </param>
                public BlaBlaException(string message, Exception innerException) : base(message, innerException)
                {
                }
            }
            """);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_exception_param_of_exception_ctor_with_message_and_exception_param() => An_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
                /// </summary>
                /// <param name="message">
                /// The error message that explains the reason for the exception.
                /// </param>
                /// <param name="innerException">
                /// The exception.
                /// <para />
                /// If the <paramref name="innerException" /> parameter is not <see langword="null"/>, the current exception is raised in a <b>catch</b> block that handles the inner exception.
                /// </param>
                public BlaBlaException(string message, Exception innerException) : base(message, innerException)
                {
                }
            }
            """);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_summary_of_exception_ctor_with_serialization_param() => An_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class.
                /// </summary>
                /// <param name="info">
                /// The object that holds the serialized object data.
                /// </param>
                /// <param name="context">
                /// The contextual information about the source or destination.
                /// </param>
                /// <remarks>
                /// This constructor is invoked during deserialization to reconstitute the exception object transmitted over a stream.
                /// </remarks>
                private BlaBlaException(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
                public BlaBlaException(string message, Exception innerException) : base(message, innerException)
                {
                }
            }
            """);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_SerializationInfo_param_of_exception_ctor_with_serialization_param() => An_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with serialized data.
                /// </summary>
                /// <param name="info">
                /// The serialization info.
                /// </param>
                /// <param name="context">
                /// The contextual information about the source or destination.
                /// </param>
                /// <remarks>
                /// This constructor is invoked during deserialization to reconstitute the exception object transmitted over a stream.
                /// </remarks>
                private BlaBlaException(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
            }
            """);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_StreamingContext_param_of_exception_ctor_with_serialization_param() => An_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with serialized data.
                /// </summary>
                /// <param name="info">
                /// The object that holds the serialized object data.
                /// </param>
                /// <param name="context">
                /// The streaming context.
                /// </param>
                /// <remarks>
                /// This constructor is invoked during deserialization to reconstitute the exception object transmitted over a stream.
                /// </remarks>
                private BlaBlaException(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
            }
            """);

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_remarks_on_exception_ctor_with_serialization_param() => An_issue_is_reported_for("""

            using System;
            using System.Runtime.Serialization;

            [Serializable]
            public sealed class BlaBlaException : Exception
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="BlaBlaException"/> class with serialized data.
                /// </summary>
                /// <param name="info">
                /// The object that holds the serialized object data.
                /// </param>
                /// <param name="context">
                /// The contextual information about the source or destination.
                /// </param>
                /// <remarks>
                /// Some remarks.
                /// </remarks>
                private BlaBlaException(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
            }
            """);

        [Test]
        public void Code_gets_fixed_for_exception_type_with_empty_summary_on_same_line()
        {
            const string OriginalCode = """

                using System;
                using System.Runtime.Serialization;

                /// <summary></summary>
                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                /// <summary>
                /// The exception that is thrown when 
                /// </summary>
                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                }
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_exception_type_with_empty_summary_on_different_lines()
        {
            const string OriginalCode = """

                using System;
                using System.Runtime.Serialization;

                /// <summary>
                /// </summary>
                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                /// <summary>
                /// The exception that is thrown when 
                /// </summary>
                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                }
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_exception_type_with_summary_on_different_lines()
        {
            const string OriginalCode = """

                using System;
                using System.Runtime.Serialization;

                /// <summary>
                /// Something is done.
                /// </summary>
                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                /// <summary>
                /// The exception that is thrown when something is done.
                /// </summary>
                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                }
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_exception_type_with_summary_on_different_lines_([ValueSource(nameof(StartingPhrases))] string message)
        {
            var originalCode = """

                using System;
                using System.Runtime.Serialization;

                /// <summary>
                /// 
                """ + message + """
                 something is done.
                /// </summary>
                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                /// <summary>
                /// The exception that is thrown when something is done.
                /// </summary>
                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                }
                """;

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_exception_type_with_a_filled_summary_on_same_line()
        {
            const string OriginalCode = """

                using System;
                using System.Runtime.Serialization;

                /// <summary>Something is done.</summary>
                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                /// <summary>
                /// The exception that is thrown when something is done.
                /// </summary>
                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                }
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_exception_type_with_a_filled_summary_on_same_line_([ValueSource(nameof(StartingPhrases))] string message)
        {
            var originalCode = @"
using System;
using System.Runtime.Serialization;

/// <summary>" + message + @" something is done.</summary>
[Serializable]
public sealed class BlaBlaException : Exception
{
}";

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                /// <summary>
                /// The exception that is thrown when something is done.
                /// </summary>
                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                }
                """;

            VerifyCSharpFix(originalCode, FixedCode);
        }

        [TestCase("Represents an exception that occurs during some operations", "The exception that is thrown when an error occurs during some operations")]
        [TestCase("Represents an exception which occurs during some operations", "The exception that is thrown when an error occurs during some operations")]
        [TestCase("An exception that occurs during some operations", "The exception that is thrown when an error occurs during some operations")]
        [TestCase("An exception which occurs during some operations", "The exception that is thrown when an error occurs during some operations")]
        [TestCase("The exception that occurs during some operations", "The exception that is thrown when an error occurs during some operations")]
        [TestCase("The exception which occurs during some operations", "The exception that is thrown when an error occurs during some operations")]
        [TestCase("Exception that occurs during some operations", "The exception that is thrown when an error occurs during some operations")]
        [TestCase("Exception which occurs during some operations", "The exception that is thrown when an error occurs during some operations")]
        public void Code_gets_fixed_for_exception_type_with_special_summary_on_same_line_(string originalMessage, string fixedMessage)
        {
            var originalCode = @"
using System;
using System.Runtime.Serialization;

/// <summary>" + originalMessage + @".</summary>
[Serializable]
public sealed class BlaBlaException : Exception
{
}";

            var fixedCode = @"
using System;
using System.Runtime.Serialization;

/// <summary>
/// " + fixedMessage + @".
/// </summary>
[Serializable]
public sealed class BlaBlaException : Exception
{
}";

            VerifyCSharpFix(originalCode, fixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_parameterless_ctor()
        {
            const string OriginalCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary></summary>
                    public BlaBlaException()
                    {
                    }
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary>
                    /// Initializes a new instance of the <see cref="BlaBlaException"/> class.
                    /// </summary>
                    public BlaBlaException()
                    {
                    }
                }
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_message_only_ctor_lower_case()
        {
            const string OriginalCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary></summary>
                    public BlaBlaException(string message)
                    {
                    }
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary>
                    /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message.
                    /// </summary>
                    /// <param name="message">
                    /// The error message that explains the reason for the exception.
                    /// </param>
                    public BlaBlaException(string message)
                    {
                    }
                }
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_message_only_ctor_upper_case()
        {
            const string OriginalCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary></summary>
                    public BlaBlaException(String message)
                    {
                    }
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary>
                    /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message.
                    /// </summary>
                    /// <param name="message">
                    /// The error message that explains the reason for the exception.
                    /// </param>
                    public BlaBlaException(String message)
                    {
                    }
                }
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nullable_message_only_ctor_lower_case()
        {
            const string OriginalCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary></summary>
                    public BlaBlaException(string? message)
                    {
                    }
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary>
                    /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message.
                    /// </summary>
                    /// <param name="message">
                    /// The error message that explains the reason for the exception.
                    /// </param>
                    public BlaBlaException(string? message)
                    {
                    }
                }
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nullable_message_only_ctor_upper_case()
        {
            const string OriginalCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary></summary>
                    public BlaBlaException(String? message)
                    {
                    }
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary>
                    /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message.
                    /// </summary>
                    /// <param name="message">
                    /// The error message that explains the reason for the exception.
                    /// </param>
                    public BlaBlaException(String? message)
                    {
                    }
                }
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_message_exception_ctor()
        {
            const string OriginalCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary></summary>
                    public BlaBlaException(string message, Exception innerException) : base(message, innerException)
                    {
                    }
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary>
                    /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
                    /// </summary>
                    /// <param name="message">
                    /// The error message that explains the reason for the exception.
                    /// </param>
                    /// <param name="innerException">
                    /// The exception that is the cause of the current exception.
                    /// <para/>
                    /// If the <paramref name="innerException"/> parameter is not <see langword="null"/>, the current exception is raised in a <b>catch</b> block that handles the inner exception.
                    /// </param>
                    public BlaBlaException(string message, Exception innerException) : base(message, innerException)
                    {
                    }
                }
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_nullable_message_exception_ctor()
        {
            const string OriginalCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary>
                    /// 
                    /// </summary>
                    public BlaBlaException(string? message, Exception? innerException) : base(message, innerException)
                    {
                    }
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary>
                    /// Initializes a new instance of the <see cref="BlaBlaException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
                    /// </summary>
                    /// <param name="message">
                    /// The error message that explains the reason for the exception.
                    /// </param>
                    /// <param name="innerException">
                    /// The exception that is the cause of the current exception.
                    /// <para/>
                    /// If the <paramref name="innerException"/> parameter is not <see langword="null"/>, the current exception is raised in a <b>catch</b> block that handles the inner exception.
                    /// </param>
                    public BlaBlaException(string? message, Exception? innerException) : base(message, innerException)
                    {
                    }
                }
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_serialization_ctor()
        {
            const string OriginalCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary></summary>
                    private BlaBlaException(SerializationInfo info, StreamingContext context)
                        : base(info, context)
                    {
                    }
                }
                """;

            const string FixedCode = """

                using System;
                using System.Runtime.Serialization;

                [Serializable]
                public sealed class BlaBlaException : Exception
                {
                    /// <summary>
                    /// Initializes a new instance of the <see cref="BlaBlaException"/> class with serialized data.
                    /// </summary>
                    /// <param name="info">
                    /// The object that holds the serialized object data.
                    /// </param>
                    /// <param name="context">
                    /// The contextual information about the source or destination.
                    /// </param>
                    /// <remarks>
                    /// This constructor is invoked during deserialization to reconstitute the exception object transmitted over a stream.
                    /// </remarks>
                    private BlaBlaException(SerializationInfo info, StreamingContext context)
                        : base(info, context)
                    {
                    }
                }
                """;

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2050_ExceptionSummaryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2050_ExceptionSummaryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2050_CodeFixProvider();

        [ExcludeFromCodeCoverage]
        private static HashSet<string> CreatePhrases()
        {
            string[] starts = [
                               "A exception", "An exception", "The exception", "This exception", "Exception",
                               "A general exception", "An general exception", "The general exception", "This general exception", "General exception",
                               "A most general exception", "An most general exception", "The most general exception", "This most general exception", "Most general exception",
                              ];
            string[] verbs = ["that is thrown", "which is thrown", "is thrown", "thrown", "thrown", "to throw", "that is fired", "which is fired", "fired", "to fire"];
            string[] conditions = ["if", "when", "in case"];

            var results = new HashSet<string>();

            foreach (var start in starts)
            {
                var lowerStart = start.ToLowerCaseAt(0);

                foreach (var verb in verbs)
                {
                    var middle = " " + verb + " ";

                    foreach (var condition in conditions)
                    {
                        var continuation = middle + condition;

                        results.Add(start + continuation);
                        results.Add("Represent " + lowerStart + continuation);
                        results.Add("Represents " + lowerStart + continuation);
                    }
                }

                results.Add(start + " used by ");
                results.Add(start + " is used by ");
                results.Add(start + " that is used by ");
                results.Add(start + " which is used by ");
                results.Add(start + " indicates that ");
                results.Add(start + " that indicates that ");
                results.Add(start + " which indicates that ");
                results.Add(start + " indicating that ");
            }

            foreach (var condition in conditions)
            {
                results.Add("Throw " + condition);
                results.Add("Thrown " + condition);

                results.Add("Fire " + condition);
                results.Add("Fired " + condition);

                results.Add("Occurs " + condition);

                results.Add("Indicates that " + condition);
            }

            return results;
        }
    }
}