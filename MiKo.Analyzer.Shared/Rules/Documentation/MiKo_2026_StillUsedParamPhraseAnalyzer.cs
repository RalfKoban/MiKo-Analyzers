using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2026_StillUsedParamPhraseAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2026";

        private static readonly string[] SentenceEndings = { ".", string.Empty };
        private static readonly string[] Phrases = new[]
                                                       {
                                                           "Unused",
                                                           "Not used",
                                                           "Not in use",
                                                           "No use",
                                                           "No usage",
                                                           "Ignore",
                                                           "Ignored",
                                                           "This parameter is not used",
                                                           "This parameter is ignored",
                                                           "The parameter is not used",
                                                           "The parameter is ignored",
                                                           "Parameter is not used",
                                                           "Parameter is ignored",
                                                       }
                                                   .SelectMany(_ => SentenceEndings, (phrase, end) => phrase + end)
                                                   .ToArray();

        public MiKo_2026_StillUsedParamPhraseAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConstructor, SyntaxKind.ConstructorDeclaration);
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            var parameters = method.ParameterList.Parameters;

            if (parameters.Count == 0)
            {
                return;
            }

            var commentTrivia = method.GetDocumentationCommentTriviaSyntax();

            if (commentTrivia is null)
            {
                return; // no comment available
            }

            var methodBody = method.Body ?? (SyntaxNode)method.ExpressionBody?.Expression;

            if (methodBody is null)
            {
                return; // unfinished code or code that has no body (such as interface methods or abstract methods)
            }

            var methodSymbol = context.GetEnclosingMethod();

            if (methodSymbol is null)
            {
                return;
            }

            var issues = Analyze(context, methodBody, methodSymbol);

            ReportDiagnostics(context, issues);
        }

        private void AnalyzeConstructor(SyntaxNodeAnalysisContext context)
        {
            var method = (ConstructorDeclarationSyntax)context.Node;

            var parameters = method.ParameterList.Parameters;

            if (parameters.Count == 0)
            {
                return;
            }

            var commentTrivia = method.GetDocumentationCommentTriviaSyntax();

            if (commentTrivia is null)
            {
                return; // no comment available
            }

            var methodBody = method.Body ?? (SyntaxNode)method.ExpressionBody?.Expression;

            if (methodBody is null)
            {
                return; // unfinished code or code that has no body (such as interface methods or abstract methods)
            }

            var methodSymbol = context.GetEnclosingMethod();

            if (methodSymbol is null)
            {
                return;
            }

            var issues = Analyze(context, methodBody, methodSymbol);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> Analyze(SyntaxNodeAnalysisContext context, SyntaxNode methodBody, IMethodSymbol method)
        {
            var used = methodBody.GetAllUsedVariables(context.SemanticModel);

            if (used.Any())
            {
                var commentXml = method.GetDocumentationCommentXml();

                foreach (var parameter in method.Parameters.Where(_ => used.Contains(_.Name)))
                {
                    var comment = parameter.GetComment(commentXml);

                    if (comment.EqualsAny(Phrases))
                    {
                        yield return Issue(parameter);
                    }
                }
            }
        }
    }
}