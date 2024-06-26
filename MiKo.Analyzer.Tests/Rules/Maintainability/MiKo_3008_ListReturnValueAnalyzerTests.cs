﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3008_ListReturnValueAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] AllowedTypes =
                                                        [
                                                            "string",
                                                            "byte[]",
                                                            "Byte[]",
                                                            "int",
                                                            "IReadOnlyCollection<string>",
                                                            "IReadOnlyList<string>",
                                                            "IReadOnlyDictionary<string, string>",
                                                        ];

        private static readonly string[] ForbiddenTypes =
                                                          [
                                                              "ICollection<string>",
                                                              "IList<string>",
                                                              "List<string>",
                                                              "IDictionary<string, string>",
                                                              "Dictionary<string, string>",
                                                          ];

        [Test]
        public void No_issue_is_reported_for_allowed_type_([ValueSource(nameof(AllowedTypes))] string returnValue) => No_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public interface TestMe
{
    public " + returnValue + @" GetSomething();
}
");

        [Test]
        public void An_issue_is_reported_for_forbidden_type_([ValueSource(nameof(ForbiddenTypes))] string returnValue) => An_issue_is_reported_for(@"
using System;
using System.Collections;
using System.Collections.Generic;

public interface TestMe
{
    public " + returnValue + @" GetSomething();
}
");

        protected override string GetDiagnosticId() => MiKo_3008_ListReturnValueAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3008_ListReturnValueAnalyzer();
    }
}