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
    public sealed class MiKo_2076_OptionalParameterDefaultPhraseAnalyzer : ParamDocumentationAnalyzer
    {
        public const string Id = "MiKo_2076";

        internal const string DefaultSeeLangwordValue = "SeeLangword";
        internal const string DefaultSeeCrefValue = "SeeCref";
        internal const string DefaultCodeValue = "CodeValue";

        public MiKo_2076_OptionalParameterDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Parameters.Any(_ => _.HasExplicitDefaultValue) && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            const string Phrase = Constants.Comments.DefaultStartingPhrase;

            if (parameterComment.GetTextTrimmed().Contains(Phrase, StringComparison.Ordinal))
            {
                // seems like there is a default parameter mentioned
                yield break;
            }

            if (parameter.HasAttributeApplied("System.Runtime.CompilerServices.CallerMemberNameAttribute"))
            {
                // nothing to report as that attribute indicates that the value gets automatically set
                yield break;
            }

            var data = CreatePropertyData(parameter);

            var properties = new Dictionary<string, string>();
            properties.Add(data.Key, data.Value);

            yield return Issue(parameter.Name, parameterComment.GetContentsLocation(), Phrase, properties);
        }

        private static KeyValuePair<string, string> CreatePropertyData(IParameterSymbol parameter)
        {
            var defaultValue = parameter.GetSyntax().Default?.Value;

            switch (defaultValue)
            {
                case IdentifierNameSyntax _:
                {
                    // seems like some field or property, so simply use that one
                    return new KeyValuePair<string, string>(DefaultSeeCrefValue, defaultValue.ToString());
                }

                case PrefixUnaryExpressionSyntax _:
                {
                    // seems like some hardcoded value negative value
                    return new KeyValuePair<string, string>(DefaultCodeValue, defaultValue.ToString());
                }

                case LiteralExpressionSyntax literal:
                {
                    // seems like some hardcoded value
                    switch (literal.Kind())
                    {
                        case SyntaxKind.TrueLiteralExpression:
                        case SyntaxKind.FalseLiteralExpression:
                        case SyntaxKind.NullLiteralExpression:
                            return new KeyValuePair<string, string>(DefaultSeeLangwordValue, defaultValue.ToString());

                        case SyntaxKind.NumericLiteralExpression:
                            return new KeyValuePair<string, string>(DefaultCodeValue, defaultValue.ToString());

                        default:
                            return new KeyValuePair<string, string>(DefaultSeeCrefValue, defaultValue.ToString());
                    }
                }
            }

            var parameterType = parameter.Type;

            if (parameterType.IsEnum())
            {
                return new KeyValuePair<string, string>(DefaultSeeCrefValue, defaultValue?.ToString());
            }

            return new KeyValuePair<string, string>(DefaultSeeCrefValue, parameterType.MinimalTypeName());
        }
    }
}