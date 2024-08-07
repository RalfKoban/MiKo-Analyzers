using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1083_CodeFixProvider)), Shared]
    public sealed class MiKo_1083_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1083";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<VariableDeclaratorSyntax>().FirstOrDefault();
    }
}