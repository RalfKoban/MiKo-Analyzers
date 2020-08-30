﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3020_CodeFixProvider)), Shared]
    public sealed class MiKo_3020_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3020_CompletedTaskAnalyzer.Id;

        protected override string Title => "Use '" + nameof(Task) + "." + nameof(Task.CompletedTask) + "'";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax) => CreateSimpleMemberAccessExpressionSyntax(nameof(Task), nameof(Task.CompletedTask));
    }
}