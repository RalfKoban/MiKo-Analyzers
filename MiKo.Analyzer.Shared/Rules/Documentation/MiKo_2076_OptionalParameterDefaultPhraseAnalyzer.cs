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

        private const string Phrase = Constants.Comments.DefaultStartingPhrase;

        private static readonly string[] Phrases =
                                                   {
                                                       "Default value is",
                                                       "Default is",
                                                       "Defaults to",
                                                   };

        public MiKo_2076_OptionalParameterDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Parameters.Any(_ => _.HasExplicitDefaultValue) && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter) => parameter.HasExplicitDefaultValue;

        protected override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            if (parameterComment.GetTextTrimmed().ContainsAny(Phrases, StringComparison.OrdinalIgnoreCase))
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

            yield return Issue(parameter.Name, parameterComment.GetContentsLocation(), Phrase, new Dictionary<string, string> { { data.Key, data.Value } });
        }

        private static KeyValuePair<string, string> CreatePropertyData(IParameterSymbol parameter)
        {
            var defaultValue = parameter.GetSyntax().Default?.Value;

            var parameterType = parameter.Type;

            switch (defaultValue)
            {
                case IdentifierNameSyntax _:
                    return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, defaultValue.ToString()); // seems like some field or property, so simply use that one

                case PrefixUnaryExpressionSyntax _:
                    return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultCodeValue, defaultValue.ToString()); // seems like some hardcoded value negative value

                case DefaultExpressionSyntax _:
                    return CreatePropertyDataForDefault(parameterType); // seems like we have some 'default(Xyz)' value

                case LiteralExpressionSyntax literal:
                {
                    // seems like some hardcoded value
                    switch (literal.Kind())
                    {
                        case SyntaxKind.TrueLiteralExpression:
                        case SyntaxKind.FalseLiteralExpression:
                        case SyntaxKind.NullLiteralExpression:
                            return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultSeeLangwordValue, defaultValue.ToString());

                        case SyntaxKind.NumericLiteralExpression:
                        case SyntaxKind.StringLiteralExpression:
                            return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultCodeValue, defaultValue.ToString());

                        case SyntaxKind.DefaultLiteralExpression:
                            return CreatePropertyDataForDefault(parameterType);

                        default:
                            return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, defaultValue.ToString());
                    }
                }
            }

            if (parameterType.IsEnum())
            {
                return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, defaultValue?.ToString());
            }

            return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, parameterType.MinimalTypeName());
        }

        private static KeyValuePair<string, string> CreatePropertyDataForDefault(ITypeSymbol parameterType)
        {
            switch (parameterType.SpecialType)
            {
                case SpecialType.System_Boolean:
                    return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultSeeLangwordValue, "false");

                case SpecialType.System_Char:
                    return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultCodeValue, @"'\0' (U+0000)");

                case SpecialType.System_DateTime:
                    return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, "DateTime.MinValue");

                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                    return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultCodeValue, "0");

                default:
                {
                    var defaultValue = parameterType.Name;

                    if (parameterType.TypeKind == TypeKind.Struct)
                    {
                        return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, defaultValue);
                    }

                    if (parameterType.IsEnum())
                    {
                        var defaultFieldValue = parameterType.GetFields().FirstOrDefault(_ => _.ConstantValue?.ToString() == "0");

                        if (defaultFieldValue != null)
                        {
                            defaultValue = defaultValue + "." + defaultFieldValue.Name;
                        }

                        return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, defaultValue);
                    }

                    return new KeyValuePair<string, string>(Constants.AnalyzerCodeFixSharedData.DefaultSeeLangwordValue, "null");
                }
            }
        }
    }
}