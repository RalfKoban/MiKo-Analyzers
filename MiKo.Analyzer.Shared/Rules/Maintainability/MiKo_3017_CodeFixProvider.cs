using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3017_CodeFixProvider)), Shared]
    public sealed class MiKo_3017_CodeFixProvider : ObjectCreationExpressionMaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3017";

        protected override string Title => Resources.MiKo_3017_CodeFixTitle;

        protected override ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax) => syntax.ArgumentList; // there might be multiple nodes to replace, hence return original and do replacement in GetUpdatedSyntaxRoot

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            var o = (ObjectCreationExpressionSyntax)syntax;

            var problematicNode = o.GetExceptionSwallowingNode(() => GetSemanticModel(document));

            var replacements = CreateReplacements(o.ArgumentList, problematicNode);

            if (replacements.Any())
            {
                return root.ReplaceNodes(replacements.Keys, (original, rewritten) => replacements[rewritten]);
            }

            return root;
        }

        private static Dictionary<SyntaxNode, SyntaxNode> CreateReplacements(ArgumentListSyntax argumentList, SyntaxNode problematicNode)
        {
            var result = new Dictionary<SyntaxNode, SyntaxNode>();

            var errorMessage = GetUpdatedErrorMessage(argumentList);

            switch (problematicNode)
            {
                case CatchClauseSyntax catchClause: // maybe we need to fix the identifier for the catch block
                {
                    var newExceptionIdentifier = SyntaxFactory.Identifier(Constants.ExceptionIdentifier);
                    var newArgumentList = ArgumentList(errorMessage, Argument(Constants.ExceptionIdentifier));

                    var declaration = catchClause.Declaration;

                    if (declaration != null)
                    {
                        var identifier = declaration.Identifier.ValueText;

                        if (identifier.IsNullOrWhiteSpace())
                        {
                            // seems like a missing exception identifier, so we have to add the missing ones
                            result.Add(argumentList, newArgumentList);
                            result.Add(declaration, declaration.WithIdentifier(newExceptionIdentifier));
                        }
                        else
                        {
                            // available but unused exception
                            result.Add(argumentList, ArgumentList(errorMessage, Argument(identifier)));
                        }
                    }
                    else
                    {
                        // seems like there is no exception inside the catch clause, so we have to add the missing ones
                        var newCatchDeclaration = SyntaxFactory.CatchDeclaration(SyntaxFactory.ParseTypeName(nameof(Exception)), newExceptionIdentifier);
                        var newBlock = catchClause.Block.ReplaceNode(argumentList, newArgumentList);

                        result.Add(catchClause, SyntaxFactory.CatchClause(newCatchDeclaration, null, newBlock));
                    }

                    break;
                }

                case ParameterSyntax parameter:
                {
                    // seems like we found the exception on the method
                    result.Add(argumentList, ArgumentList(errorMessage, Argument(parameter)));
                    break;
                }

                case ExpressionSyntax expression:
                {
                    // seems like we found the exception on the method
                    result.Add(argumentList, ArgumentList(errorMessage, Argument(expression)));
                    break;
                }
            }

            return result;
        }
    }
}