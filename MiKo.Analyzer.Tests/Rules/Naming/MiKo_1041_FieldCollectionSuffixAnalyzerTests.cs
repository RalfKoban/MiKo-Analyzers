﻿using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1041_FieldCollectionSuffixAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_field() => No_issue_is_reported_for(@"

public class TestMe
{
    private string bla;
}
");

        [TestCase("blaList")]
        [TestCase("blaCollection")]
        [TestCase("blaObservableCollection")]
        [TestCase("blaArray")]
        [TestCase("blaHashSet")]
        public void No_issue_is_reported_for_incorrectly_named_field_in_enum_(string field) => No_issue_is_reported_for(@"

public enum TestMe
{
    " + field + @",
}
");

        [TestCase("string blaList")]
        [TestCase("string blaCollection")]
        [TestCase("string blaObservableCollection")]
        [TestCase("string blaArray")]
        [TestCase("string blaHashSet")]
        public void An_issue_is_reported_for_incorrectly_named_field_(string field) => An_issue_is_reported_for(@"

public class TestMe
{
    private " + field + @";
}
");

        protected override string GetDiagnosticId() => MiKo_1041_FieldCollectionSuffixAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1041_FieldCollectionSuffixAnalyzer();
    }
}