using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6023_ObjectDisposedExceptionThrowIfStatementSurroundedByBlankLinesAnalyzer : CallSurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6023";

        public MiKo_6023_ObjectDisposedExceptionThrowIfStatementSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override bool IsApplicable(CompilationStartAnalysisContext context) => context.Compilation.GetTypeByMetadataName("System.ObjectDisposedException") != null;

        // it may happen that in some broken code Roslyn is unable to detect a type (eg. due to missing code paths), hence 'type' could be null here
        protected override bool IsCall(ITypeSymbol type) => type?.Name == nameof(ObjectDisposedException);

        protected override bool IsCall(MemberAccessExpressionSyntax call, SemanticModel semanticModel) => call.GetName() == "ThrowIf" && base.IsCall(call, semanticModel);
    }
}