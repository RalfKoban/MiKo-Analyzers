using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public sealed partial class MiKo_3248_UseLiteralForThrowIfCheckAnalyzerTests
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_empty_with_expression_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public record Item(string Name);

public class TestMe
{
    public void DoSomething(Item value) => ArgumentNullException.ThrowIfNull(value with { });
}
", LanguageVersion.CSharp9);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_empty_with_expression_as_argument_and_paramName() => An_issue_is_reported_for(@"
using System;

public record Item(string Name);

public class TestMe
{
    public void DoSomething(Item value, string paramName) => ArgumentNullException.ThrowIfNull(value with { }, paramName);
}
", LanguageVersion.CSharp9);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_parenthesized_with_expression_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public record Item(string Name);

public class TestMe
{
    public void DoSomething(Item value) => ArgumentNullException.ThrowIfNull((value with { }));
}
", LanguageVersion.CSharp9);

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = Justifications.StyleCop.SA1116)]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = Justifications.StyleCop.SA1117)]
        [Test]
        public void An_issue_is_reported_for_ThrowIfNull_invocation_with_with_expression_on_struct_as_argument_and_no_paramName() => An_issue_is_reported_for(@"
using System;

public struct Item
{
    public string Name { get; set; }
}

public class TestMe
{
    public void DoSomething(Item value) => ArgumentNullException.ThrowIfNull(value with { });
}
", LanguageVersion.CSharp10);
    }
}
