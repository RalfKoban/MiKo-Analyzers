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
                                                   .SelectMany(_ => SentenceEndings, string.Concat)
                                                   .ToArray();

        public MiKo_2026_StillUsedParamPhraseAnalyzer() : base(Id)
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

            var issues = Analyze(context, method, methodBody, methodSymbol);

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

            var issues = Analyze(context, method, methodBody, methodSymbol);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> Analyze(SyntaxNodeAnalysisContext context, SyntaxNode method, SyntaxNode methodBody, IMethodSymbol methodSymbol)
        {
            var comments = method.GetDocumentationCommentTriviaSyntax();
            var commentsLength = comments.Length;

            if (commentsLength <= 0)
            {
                yield break;
            }

            var used = methodBody.GetAllUsedVariables(context.SemanticModel);

            if (used.Count <= 0)
            {
                yield break;
            }

            var parameters = methodSymbol.Parameters;
            var parametersLength = parameters.Length;

            if (parametersLength <= 0)
            {
                yield break;
            }

            for (var index = 0; index < parametersLength; index++)
            {
                var parameter = parameters[index];
                var parameterName = parameter.Name;

                if (used.Contains(parameterName))
                {
                    for (var i = 0; i < commentsLength; i++)
                    {
                        var parameterComment = comments[i].GetParameterComment(parameterName);

                        if (parameterComment is null)
                        {
                            continue;
                        }

                        if (parameterComment.GetTextTrimmed().EqualsAny(Phrases))
                        {
                            yield return Issue(parameterName, parameterComment.GetContentsLocation());
                        }
                    }
                }
            }
        }
    }
}