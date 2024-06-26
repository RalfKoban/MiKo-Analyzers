﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1073_BooleanFieldNamedAsQuestionAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] CorrectNames =
                                                        [
                                                            "AreConnected",
                                                            "IsConnected",
                                                            "Connected",
                                                            "HasConnectionEstablished",
                                                            "CanBeConnected",
                                                            "InDesign",
                                                            "InDesignMode",
                                                            "IsInDesign",
                                                            "IsInDesignMode",
                                                            "IsInDesignerMode",
                                                        ];

        private static readonly string[] WrongNames =
                                                      [
                                                          "IsConnectionPossible",
                                                          "AreDevicesConnected",
                                                      ];

        private static readonly string[] Prefixes =
                                                    [
                                                        "m_",
                                                        "s_",
                                                        "t_",
                                                        "_",
                                                    ];

        [Test]
        public void No_issue_is_reported_for_empty_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_non_Boolean_field_([ValueSource(nameof(Prefixes))] string prefix) => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    private int  " + prefix + @"IsDoingSomething;
}
");

        [Test, Combinatorial]
        public void No_issue_is_reported_for_Boolean_field_with_correct_name_([ValueSource(nameof(CorrectNames))] string name, [ValueSource(nameof(Prefixes))] string prefix) => No_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    private bool " + prefix + name + @";
}
");

        [Test, Combinatorial]
        public void An_issue_is_reported_for_Boolean_field_with_incorrect_name_([ValueSource(nameof(WrongNames))] string name, [ValueSource(nameof(Prefixes))] string prefix) => An_issue_is_reported_for(@"
using System;
using System.Threading;

public class TestMe
{
    private bool " + prefix + name + @";
}
");

        protected override string GetDiagnosticId() => MiKo_1073_BooleanFieldNamedAsQuestionAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1073_BooleanFieldNamedAsQuestionAnalyzer();
    }
}