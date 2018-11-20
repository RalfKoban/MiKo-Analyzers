using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3401_NamespaceDepthAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_namespace_within_depth([ValueSource(nameof(AllowedNamespaceNames))]string ns) => No_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        [Test]
        public void An_issue_is_reported_for_namespace_within_depth([ValueSource(nameof(TooDeepNamespaceNames))]string ns) => An_issue_is_reported_for(@"
namespace " + ns + @"
{
}
");

        protected override string GetDiagnosticId() => MiKo_3401_NamespaceDepthAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3401_NamespaceDepthAnalyzer();

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> AllowedNamespaceNames() => new HashSet<string>
                                                                            {
                                                                                "A",
                                                                                "A.B",
                                                                                "A.B.C",
                                                                                "A.B.C.D",
                                                                                "A.B.C.D.E",
                                                                                "A.B.C.D.E.F",
                                                                                "A.B.C.D.E.F.G",
                                                                            };

        [ExcludeFromCodeCoverage]
        private static IEnumerable<string> TooDeepNamespaceNames() => new HashSet<string>
                                                                            {
                                                                                "A.B.C.D.E.F.G.H",
                                                                                "A.B.C.D.E.F.G.H.I",
                                                                                "A.B.C.D.E.F.G.H.I.J",
                                                                                "A.B.C.D.E.F.G.H.I.J.K",
                                                                                "A.B.C.D.E.F.G.H.I.J.K.L",
                                                                                "A.B.C.D.E.F.G.H.I.J.K.L.M",
                                                                            };
    }
}