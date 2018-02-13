﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1011_DoMethodsAnalyzerTests : CodeFixVerifier
    {
        [TestCase("CanDock")]
        [TestCase("CanDouble")]
        [TestCase("CanDown")]
        [TestCase("CanDownload")]
        [TestCase("Dock")]
        [TestCase("Dot")]
        [TestCase("Double")]
        [TestCase("Down")]
        [TestCase("Download")]
        [TestCase("IsDown")]
        [TestCase("Whatever")]
        [TestCase("CallsDownloadWorkflowForMultipleParameterDownloadDevices")]
        public void No_issue_is_reported_for_correctly_named_method(string methodName) => No_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        [TestCase("CanDo")]
        [TestCase("CanDoWhatever")]
        [TestCase("CanDoDock")]
        [TestCase("CanDoDouble")]
        [TestCase("CanDoDown")]
        [TestCase("CanDoDownload")]
        [TestCase("Do")]
        [TestCase("DoDock")]
        [TestCase("DoDot")]
        [TestCase("DoDouble")]
        [TestCase("DoDown")]
        [TestCase("DoDownload")]
        [TestCase("DoWhatever")]
        [TestCase("IsDoDown")]
        public void An_issue_is_reported_for_wrong_named_method(string methodName) => An_issue_is_reported_for(@"
public class TestMe
{
    public void " + methodName + @"() { }
}
");

        protected override string GetDiagnosticId() => MiKo_1011_DoMethodsAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1011_DoMethodsAnalyzer();
    }
}