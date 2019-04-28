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

        private static HashSet<string> GetAllUsedVariables(SyntaxNodeAnalysisContext context, SyntaxNode statementOrExpression)
        {
            var dataFlow = context.SemanticModel.AnalyzeDataFlow(statementOrExpression);

            // do not use the declared ones as we are interested in parameters, not unused variables
            // var variablesDeclared = dataFlow.VariablesDeclared;

            var variablesRead = dataFlow.ReadInside.Union(dataFlow.ReadOutside);

            // do not include the ones that are written outside as those are the ones that are not used at all
            var variablesWritten = dataFlow.WrittenInside;

            var used = variablesRead.Union(variablesWritten).Select(_ => _.Name).ToHashSet();
            return used;
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            var parameters = method.ParameterList.Parameters;
            if (parameters.Count == 0)
                return;

            var methodBody = method.Body ?? (SyntaxNode)method.ExpressionBody?.Expression;
            if (methodBody is null)
                return; // unfinished code or code that has no body (such as interface methods or abstract methods)

            var methodSymbol = method.GetEnclosingMethod(context.SemanticModel);
            if (methodSymbol is null)
                return;

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
                return;

            var methodBody = method.Body ?? (SyntaxNode)method.ExpressionBody?.Expression;
            if (methodBody is null)
                return; // unfinished code or code that has no body (such as interface methods or abstract methods)

            var methodSymbol = method.GetEnclosingMethod(context.SemanticModel);
            if (methodSymbol is null)
                return;

            var results = Analyze(context, methodBody, methodSymbol);
            foreach (var diagnostic in results)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private IEnumerable<Diagnostic> Analyze(SyntaxNodeAnalysisContext context, SyntaxNode methodBody, IMethodSymbol method)
        {
            var used = GetAllUsedVariables(context, methodBody);

            List<Diagnostic> results = null;
            foreach (var parameter in method.Parameters)
            {
                if (!used.Contains(parameter.Name))
                    continue;

                // check comment
                var commentXml = method.GetDocumentationCommentXml();
                var comment = parameter.GetComment(commentXml);

                if (comment.EqualsAny(Phrases, StringComparison.OrdinalIgnoreCase))
                {
                    if (results == null)
                        results = new List<Diagnostic>();

                    var diagnostic = Issue(parameter);
                    results.Add(diagnostic);
                }
            }

            return results ?? Enumerable.Empty<Diagnostic>();
        }
    }
}