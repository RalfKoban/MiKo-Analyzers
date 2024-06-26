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
        private static readonly HashSet<string> EnumerableMethods = typeof(Enumerable).GetMethods().ToHashSet(_ => _.Name)
                                                                                      .Except(typeof(object).GetMethods().Select(_ => _.Name)) // get rid of GetHashCode() or Equals()
                                                                                      .Except("Contains"); // special handling

        public override string FixableDiagnosticId => "MiKo_3109";

        protected override string Title => Resources.MiKo_3109_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var original = (InvocationExpressionSyntax)syntax;

            if (original.Expression is MemberAccessExpressionSyntax maes && maes.Expression is IdentifierNameSyntax type)
            {
                var typeName = type.GetName();

                switch (typeName)
                {
                    case "Assert":
                    case "CollectionAssert":
                    case "StringAssert":
                    {
                        var args = original.ArgumentList;
                        var fixedArgs = UpdatedSyntax(maes, args);

                        if (fixedArgs != args)
                        {
                            return original.ReplaceNode(args, fixedArgs);
                        }

                        break;
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
                case "Fail":
                case "Inconclusive":
                case "Ignore":
                case "Multiple":
                    return args; // do not adjust

                case "Less":
                case "LessOrEqual":
                case "Greater":
                case "GreaterOrEqual":
                    return OldAssertFixer.Fix(args, 0);

                case "That":
                    return AssertThatFixer.FixThat(args);

                default:
                    return OldAssertFixer.Fix(args, args.Arguments.Count - 1);
            }
        }

        private static string GetText(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case MemberAccessExpressionSyntax m: return m.GetName();
                case ObjectCreationExpressionSyntax o: return o.Type.GetNameOnlyPart();

                case InvocationExpressionSyntax method:
                {
                    var name = method.Expression.GetName();

                    if (name == "Parse")
                    {
                        return method.ArgumentList?.Arguments.FirstOrDefault()?.GetName();
                    }

                    return EnumerableMethods.Contains(name)
                           ? method.GetName()
                           : name;
                }

                default:
                    return expression.GetName();
            }
        }

        private static List<string> GetFinalText(SeparatedSyntaxList<ArgumentSyntax> arguments, int index, out string suffix)
        {
            var foundSuffix = string.Empty;

            var text = GetText(arguments[index].Expression);
            var finalText = text.AsSpan()
                                .WordsAsSpan()
                                .Select(_ => _.Text.Trim(Constants.Underscores).ToLowerCaseAt(0))
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
                                                        foundSuffix = "state";

                                                        return string.Empty;
                                                    }

                                                    case "execute":
                                                        return "executable";

                                                    default:
                                                        return _;
                                                }
                                            })
                                .Where(_ => _.HasCharacters())
                                .ToList();

            suffix = foundSuffix;

            return finalText;
        }

        private static class AssertThatFixer
        {
            internal static ArgumentListSyntax FixThat(ArgumentListSyntax args)
            {
                var arguments = args.Arguments;

                var finalText = GetFinalText(arguments, 0, out var suffix);
                var firstWord = GetStartingWord(arguments);

                finalText.Insert(0, firstWord);

                if (suffix.HasCharacters())
                {
                    finalText.Add(suffix);
                }

                return args.WithArguments(arguments.Add(Argument(StringLiteral(finalText.ConcatenatedWith(" ")))));
            }

            // let's see if we have the special case 'Is.Not.Null'
            private static string GetStartingWord(SeparatedSyntaxList<ArgumentSyntax> arguments)
            {
                if (arguments.Count > 1 && arguments[1].Expression.ToString() == "Is.Not.Null")
                {
                    return "missing";
                }

                return "wrong";
            }
        }

        private static class OldAssertFixer
        {
            internal static ArgumentListSyntax Fix(ArgumentListSyntax args, int index)
            {
                var arguments = args.Arguments;

                var finalText = GetFinalText(arguments, index, out var suffix);
                finalText.Insert(0, "wrong"); // TODO RKN: Let's see if we have to distinguish based on the call itself

                if (suffix.HasCharacters())
                {
                    finalText.Add(suffix);
                }

                return args.WithArguments(arguments.Add(Argument(StringLiteral(finalText.ConcatenatedWith(" ")))));
            }
        }
    }
}