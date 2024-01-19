﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3217_CodeFixProvider)), Shared]
    public sealed class MiKo_3217_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3217_InvertIfInsideBlockWhenFollowedBySingleCodeLinesAnalyzer.Id;

        protected override string Title => Resources.MiKo_3217_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<IfStatementSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => syntax;

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            if (syntax is IfStatementSyntax ifStatement && syntax.Parent is BlockSyntax block)
            {
                var statements = block.Statements.ToList();

                var index = statements.IndexOf(ifStatement);

                if (index < statements.Count)
                {
                    var others = statements.Skip(index + 1).Select(_ => _.WithAdditionalLeadingSpaces(Constants.Indentation)).ToList(); // adjust spacing

                    if (others.Count > 0)
                    {
                        others[0] = others[0].WithoutLeadingEndOfLine();
                    }

                    var condition = ifStatement.Condition;
                    var newIf = ifStatement.WithCondition(InvertCondition(document, condition).WithTriviaFrom(condition))
                                           .WithStatement(SyntaxFactory.Block(others));

                    var comment = GetComment(ifStatement.Statement);

                    if (comment.Length > 0)
                    {
                        newIf = newIf.WithAdditionalLeadingTrivia(comment)
                                     .WithAdditionalLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                                     .WithAdditionalLeadingTriviaFrom(ifStatement);
                    }

                    return root.ReplaceNodes(statements.Skip(index), (original, rewritten) => original == ifStatement ? newIf : null);
                }
            }

            return root;
        }

        private static SyntaxTrivia[] GetComment(StatementSyntax statement)
        {
            if (statement is BlockSyntax block)
            {
                statement = block.Statements.FirstOrDefault();
            }

            if (statement is ContinueStatementSyntax cs)
            {
                if (cs.ContinueKeyword.HasComment())
                {
                    return cs.ContinueKeyword.GetComment();
                }

                if (cs.SemicolonToken.HasComment())
                {
                    return cs.SemicolonToken.GetComment();
                }
            }

            return Array.Empty<SyntaxTrivia>();
        }
    }
}