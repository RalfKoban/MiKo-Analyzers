using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3109_CodeFixProvider)), Shared]
    public sealed class MiKo_3109_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3109_TestAssertsHaveMessageAnalyzer.Id;

        protected override string Title => Resources.MiKo_3109_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var original = (InvocationExpressionSyntax)syntax;

            if (original.Expression is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax type)
            {
                var typeName = type.GetName();

                if (typeName == "Assert")
                {
                    var args = original.ArgumentList;
                    var fixedArgs = UpdatedSyntax(maes, args);
                    if (fixedArgs != args)
                    {
                        return original.ReplaceNode(args, fixedArgs);
                    }
                }
            }

            return original;
        }

        private static ArgumentListSyntax UpdatedSyntax(MemberAccessExpressionSyntax syntax, ArgumentListSyntax args)
        {
            var methodName = syntax.GetName();

            switch (methodName)
            {
                case "That": return FixThat(args);
                default: return args;
            }
        }

        private static ArgumentListSyntax FixThat(ArgumentListSyntax args)
        {
            var arguments = args.Arguments;
            var text = GetText(arguments[0].Expression);

            var finalText = text.Words()
                                .Select(_ => _.ToLowerCaseAt(0))
                                .Select(_ =>
                                            {
                                                switch (_)
                                                {
                                                    case "add":
                                                    case "can":
                                                    case "get":
                                                    case "has":
                                                    case "is":
                                                    case "remove":
                                                    case "set":
                                                        return string.Empty;

                                                    case "contains":
                                                        return "containment";

                                                    case "id":
                                                        return "identifier";

                                                    default:
                                                        return _;
                                                }
                                            })
                                .Where(_ => string.IsNullOrEmpty(_) is false)
                                .ToList();

            var firstWord = GetStartingWord(arguments);

            finalText.Insert(0, firstWord);

            return args.WithArguments(arguments.Add(Argument(StringLiteral(finalText.ConcatenatedWith(" ")))));
        }

        private static string GetText(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case MemberAccessExpressionSyntax member: return member.GetName();
                case InvocationExpressionSyntax method: return method.Expression.GetName();
                default: return expression.GetName();
            }
        }

        // let's see if we have the special case 'Is.Not.Null'
        private static string GetStartingWord(SeparatedSyntaxList<ArgumentSyntax> arguments) => arguments[1].Expression.ToString() == "Is.Not.Null"
                                                                                                    ? "missing"
                                                                                                    : "wrong";
    }
}