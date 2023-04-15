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
        private static readonly HashSet<string> EnumerableMethods = typeof(Enumerable).GetMethods().Select(_ => _.Name)
                                                                                      .Except(typeof(object).GetMethods().Select(_ => _.Name)) // get rid of GetHashCode() or Equals()
                                                                                      .Except("Contains") // special handling
                                                                                      .ToHashSet();

        public override string FixableDiagnosticId => MiKo_3109_TestAssertsHaveMessageAnalyzer.Id;

        protected override string Title => Resources.MiKo_3109_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

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

            string suffix = null;

            var finalText = text.AsSpan()
                                .Words()
                                .Select(_ => _.ToLowerCaseAt(0))
                                .Select(_ =>
                                            {
                                                switch (_)
                                                {
                                                    case "add":
                                                    case "get":
                                                    case "has":
                                                    case "is":
                                                    case "remove":
                                                    case "set":
                                                        return string.Empty;

                                                    case "contains":
                                                        return "value";

                                                    case "id":
                                                        return "identifier";

                                                    case "can":
                                                    {
                                                        suffix = "state";
                                                        return string.Empty;
                                                    }

                                                    case "execute":
                                                        return "executable";

                                                    default:
                                                        return _;
                                                }
                                            })
                                .Where(_ => string.IsNullOrEmpty(_) is false)
                                .ToList();

            var firstWord = GetStartingWord(arguments);

            finalText.Insert(0, firstWord);

            if (suffix != null)
            {
                finalText.Add(suffix);
            }

            return args.WithArguments(arguments.Add(Argument(StringLiteral(finalText.ConcatenatedWith(" ")))));
        }

        private static string GetText(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case MemberAccessExpressionSyntax member:
                {
                    return member.GetName();
                }

                case InvocationExpressionSyntax method:
                {
                    var name = method.Expression.GetName();

                    if (name == "Parse")
                    {
                        return method.ArgumentList?.Arguments.FirstOrDefault()?.GetName();
                    }

                    if (EnumerableMethods.Contains(name))
                    {
                        return method.GetName();
                    }

                    return name;
                }

                case ObjectCreationExpressionSyntax creation:
                {
                    return creation.Type.GetNameOnlyPart();
                }

                default:
                {
                    return expression.GetName();
                }
            }
        }

        // let's see if we have the special case 'Is.Not.Null'
        private static string GetStartingWord(SeparatedSyntaxList<ArgumentSyntax> arguments) => arguments.Count > 1 && arguments[1].Expression.ToString() == "Is.Not.Null"
                                                                                                    ? "missing"
                                                                                                    : "wrong";
    }
}