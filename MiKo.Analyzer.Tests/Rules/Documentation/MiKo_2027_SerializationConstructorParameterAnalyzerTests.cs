﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2027_SerializationConstructorParameterAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_method_that_has_no_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        public int DoSomething()
        {
            var j = 42;
            return j;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_that_has_no_parameter() => No_issue_is_reported_for(@"
using System;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Ctor.</summary>
        public TestMe()
        {
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_method_that_has_serialization_parameters() => No_issue_is_reported_for(@"
using System;
using System.Runtime.Serialization;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""info"">blah</param>
        /// <param name=""context"">blubb</param>
        public int DoSomething(SerializationInfo info, StreamingContext context)
        {
            var j = 42;
            return j;
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_ctor_that_has_correctly_documented_serialization_parameter() => No_issue_is_reported_for(@"
using System;
using System.Runtime.Serialization;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""info"">The object that holds the serialized object data.</param>
        /// <param name=""context"">The contextual information about the source or destination.</param>
        public TestMe(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_that_has_incorrectly_documented_SerializationInfo_parameter() => An_issue_is_reported_for(@"
using System;
using System.Runtime.Serialization;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""info"">blah</param>
        /// <param name=""context"">The contextual information about the source or destination.</param>
        public TestMe(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_ctor_that_has_incorrectly_documented_StreamingContext_parameter() => An_issue_is_reported_for(@"
using System;
using System.Runtime.Serialization;

namespace Bla
{
    public class TestMe
    {
        /// <summary>Does something.</summary>
        /// <param name=""info"">The object that holds the serialized object data.</param>
        /// <param name=""context"">blah</param>
        public TestMe(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_2027_SerializationConstructorParameterAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2027_SerializationConstructorParameterAnalyzer();
    }
}