using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3201_CodeFixProvider)), Shared]
    public sealed class MiKo_3201_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3201";

        protected override string Title => Resources.MiKo_3201_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<IfStatementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is IfStatementSyntax ifStatement && syntax.Parent is BlockSyntax block)
            {
                var statements = block.Statements.ToArray();

                var index = statements.IndexOf(ifStatement);

                if (index < statements.Length)
                {
                    var condition = ifStatement.Condition;
                    var newIf = ifStatement.WithCondition(InvertCondition(document, condition).WithTriviaFrom(condition));

                    var others = statements.Skip(index + 1).ToList();

                    if (others.Count > 0)
                    {
                        others[0] = others[0].WithoutLeadingEndOfLine();
                        var spaces = others[0].GetPositionWithinStartLine();

                        newIf = newIf.WithStatement(GetUpdatedBlock(SyntaxFactory.Block(others), spaces)); // adjust spacing
                    }

                    return root.ReplaceNodes(statements.Skip(index), (original, rewritten) => original == ifStatement ? newIf : null);
                }
            }

            return root;
        }
    }
}