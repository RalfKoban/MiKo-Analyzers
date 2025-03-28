﻿using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3401_NamespaceDepthAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AllowedNamespaceNames =
                                                                 [
                                                                     "A",
                                                                     "A.B",
                                                                     "A.B.C",
                                                                     "A.B.C.D",
                                                                     "A.B.C.D.E",
                                                                     "A.B.C.D.E.F",
                                                                     "A.B.C.D.E.F.G",
                                                                 ];

        private static readonly string[] TooDeepNamespaceNames =
                                                                 [
                                                                     "A.B.C.D.E.F.G.H",
                                                                     "A.B.C.D.E.F.G.H.I",
                                                                     "A.B.C.D.E.F.G.H.I.J",
                                                                     "A.B.C.D.E.F.G.H.I.J.K",
                                                                     "A.B.C.D.E.F.G.H.I.J.K.L",
                                                                     "A.B.C.D.E.F.G.H.I.J.K.L.M",
                                                                 ];

        [Test]
        public void No_issue_is_reported_for_namespace_within_depth_([ValueSource(nameof(AllowedNamespaceNames))] string ns) => No_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_namespace_that_exceeds_depth_([ValueSource(nameof(TooDeepNamespaceNames))] string ns) => An_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_nested_namespace_that_just_exceeds_depth() => An_issue_is_reported_for(@"
namespace A
{
    namespace B
    {
        namespace C
        {
            namespace D
            {
                namespace E
                {
                    namespace F
                    {
                        namespace G
                        {
                            namespace H
                            {
                            }
                        }
                    }
                }
            }
        }
    }
}
");

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = Justifications.StyleCop.SA1118)]
        [Test]
        public void An_issue_is_reported_for_nested_namespace_that_exceeds_depth() => An_issue_is_reported_for(2, @"
namespace A
{
    namespace B
    {
        namespace C
        {
            namespace D
            {
                namespace E
                {
                    namespace F
                    {
                        namespace G
                        {
                            namespace H
                            {
                                namespace I
                                {
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_nested_combined_namespace_that_exceeds_depth() => An_issue_is_reported_for(@"
namespace A.B.C.D
{
    namespace E.F.G.H
    {
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3401_NamespaceDepthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3401_NamespaceDepthAnalyzer();
    }
}
