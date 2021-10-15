using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3033_CodeFixProvider)), Shared]
    public sealed class MiKo_3033_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3033_PropertyChangeEventArgsCtorUsingNameofAnalyzer.Id;

        protected override string Title => Resources.MiKo_3033_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<LiteralExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic) => NameOf((LiteralExpressionSyntax)syntax);
    }
}