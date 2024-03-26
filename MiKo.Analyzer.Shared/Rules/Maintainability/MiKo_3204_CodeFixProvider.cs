using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3204_CodeFixProvider)), Shared]
    public sealed class MiKo_3204_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3204";

        protected override string Title => Resources.MiKo_3204_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<IfStatementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is IfStatementSyntax ifStatement && ifStatement.Else is ElseClauseSyntax elseClause)
            {
                var ifKeyword = ifStatement.IfKeyword;
                var elseKeyword = elseClause.ElseKeyword;
                var condition = ifStatement.Condition;
                var ifCloseParenthesis = ifStatement.CloseParenToken;

                var newIfStatement = ifStatement.WithCondition(InvertCondition(document, condition).WithTriviaFrom(condition)).WithStatement(elseClause.Statement);
                var newElseClause = elseClause.WithStatement(ifStatement.Statement);

                if (elseKeyword.HasTrailingComment())
                {
                    newIfStatement = newIfStatement.WithCloseParenToken(ifCloseParenthesis.WithTrailingTriviaFrom(elseKeyword));
                }

                if (ifCloseParenthesis.HasTrailingComment())
                {
                    newElseClause = newElseClause.WithElseKeyword(elseKeyword.WithTrailingTriviaFrom(ifCloseParenthesis));
                }

                if (elseKeyword.HasLeadingComment())
                {
                    if (ifKeyword.HasLeadingComment())
                    {
                        newIfStatement = newIfStatement.WithIfKeyword(ifKeyword.WithLeadingTriviaFrom(elseKeyword));
                        newElseClause = newElseClause.WithElseKeyword(elseKeyword.WithLeadingTriviaFrom(ifKeyword));
                    }
                    else
                    {
                        newIfStatement = newIfStatement.WithLeadingTriviaFrom(elseKeyword);
                        newElseClause = newElseClause.WithElseKeyword(elseKeyword.WithLeadingTriviaFrom(ifKeyword));
                    }
                }

                return newIfStatement.WithElse(newElseClause);
            }

            return syntax;
        }
    }
}