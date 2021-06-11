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

        protected override void InitializeCore(AnalysisContext context)
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

            var results = Analyze(context, methodBody, methodSymbol);
            foreach (var diagnostic in results)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private void AnalyzeConstructor(SyntaxNodeAnalysisContext context)
        {
            var method = (ConstructorDeclarationSyntax)context.Node;

            var parameters = method.ParameterList.Parameters;
            if (parameters.Count == 0)
            {
                return;
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

            var results = Analyze(context, methodBody, methodSymbol);
            foreach (var diagnostic in results)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private IEnumerable<Diagnostic> Analyze(SyntaxNodeAnalysisContext context, SyntaxNode methodBody, IMethodSymbol method)
        {
            var used = methodBody.GetAllUsedVariables(context.SemanticModel);

            return from parameter in method.Parameters
                   where used.Contains(parameter.Name)
                   let commentXml = method.GetDocumentationCommentXml()
                   let comment = parameter.GetComment(commentXml)
                   where comment.EqualsAny(Phrases, StringComparison.OrdinalIgnoreCase)
                   select Issue(parameter);
        }
    }
}