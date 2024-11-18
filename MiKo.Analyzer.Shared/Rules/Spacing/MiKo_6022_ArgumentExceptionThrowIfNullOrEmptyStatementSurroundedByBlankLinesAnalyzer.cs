using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6022_ArgumentExceptionThrowIfNullOrEmptyStatementSurroundedByBlankLinesAnalyzer : CallSurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6022";

        public MiKo_6022_ArgumentExceptionThrowIfNullOrEmptyStatementSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override bool IsApplicable(CompilationStartAnalysisContext context) => context.Compilation.GetTypeByMetadataName("System.ArgumentException") != null;

        // it may happen that in some broken code Roslyn is unable to detect a type (e.g. due to missing code paths), hence 'type' could be null here
        protected override bool IsCall(ITypeSymbol type) => type?.Name == nameof(ArgumentException);

        protected override bool IsCall(MemberAccessExpressionSyntax syntax, SemanticModel semanticModel) => syntax.GetName() == "ThrowIfNullOrEmpty" && base.IsCall(syntax, semanticModel);

        protected override bool IsAlsoCall(MemberAccessExpressionSyntax syntax, SemanticModel semanticModel) => syntax.GetName()?.StartsWith("ThrowIf", StringComparison.Ordinal) is true;
    }
}