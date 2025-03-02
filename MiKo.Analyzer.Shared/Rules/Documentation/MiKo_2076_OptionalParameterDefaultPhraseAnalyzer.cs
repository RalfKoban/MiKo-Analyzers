﻿using System;
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

        public MiKo_2076_OptionalParameterDefaultPhraseAnalyzer() : base(Id) => IgnoreEmptyParameters = false;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Parameters.Any(_ => _.HasExplicitDefaultValue) && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyzeParameter(IParameterSymbol parameter) => parameter.HasExplicitDefaultValue;

        protected override Diagnostic[] AnalyzeParameter(IParameterSymbol parameter, XmlElementSyntax parameterComment, string comment)
        {
            if (parameterComment.GetTextTrimmed().ContainsAny(Phrases, StringComparison.OrdinalIgnoreCase))
            {
                // seems like there is a default parameter mentioned
                return Array.Empty<Diagnostic>();
            }

            if (parameter.HasAttribute("System.Runtime.CompilerServices.CallerMemberNameAttribute"))
            {
                // nothing to report as that attribute indicates that the value gets automatically set
                return Array.Empty<Diagnostic>();
            }

            var data = CreatePropertyData(parameter);

            return new[] { Issue(parameter.Name, parameterComment.GetContentsLocation(), Phrase, new[] { data }) };
        }

        private static Pair CreatePropertyData(IParameterSymbol parameter)
        {
            var defaultValue = parameter.GetSyntax().Default?.Value;

            var parameterType = parameter.Type;

            switch (defaultValue?.Kind())
            {
                case SyntaxKind.IdentifierName:
                    return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, defaultValue.ToString()); // seems like some field or property, so simply use that one

                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.AddressOfExpression:
                case SyntaxKind.PointerIndirectionExpression:
                case SyntaxKind.IndexExpression:
                    return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultCodeValue, defaultValue.ToString()); // seems like some hardcoded value negative value

                case SyntaxKind.DefaultExpression:
                    return CreatePropertyDataForDefault(parameterType); // seems like we have some 'default(Xyz)' value

                // seems like some hardcoded value
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                    return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultSeeLangwordValue, defaultValue.ToString());

                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
                    return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultCodeValue, defaultValue.ToString());

                case SyntaxKind.DefaultLiteralExpression:
                    return CreatePropertyDataForDefault(parameterType);

                case SyntaxKind.ArgListExpression:
                case SyntaxKind.CharacterLiteralExpression:
                    return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, defaultValue.ToString());
            }

            if (parameterType.IsEnum())
            {
                return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, defaultValue?.ToString());
            }

            return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, parameterType.MinimalTypeName());
        }

        private static Pair CreatePropertyDataForDefault(ITypeSymbol parameterType)
        {
            switch (parameterType.SpecialType)
            {
                case SpecialType.System_Boolean:
                    return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultSeeLangwordValue, "false");

                case SpecialType.System_Char:
                    return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultCodeValue, @"'\0' (U+0000)");

                case SpecialType.System_DateTime:
                    return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, "DateTime.MinValue");

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
                    return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultCodeValue, "0");

                default:
                    return CreatePropertyDataForDefaultNonSpecial(parameterType);
            }
        }

        private static Pair CreatePropertyDataForDefaultNonSpecial(ITypeSymbol parameterType)
        {
            switch (parameterType.TypeKind)
            {
                case TypeKind.Struct:
                {
                    var defaultStructValue = parameterType.GetProperties().FirstOrDefault(_ => _.IsStatic)?.Name
                                          ?? parameterType.GetFields().FirstOrDefault(_ => _.IsStatic)?.Name;

                    var defaultValue = defaultStructValue != null
                                       ? parameterType.Name + "." + defaultStructValue
                                       : parameterType.Name;

                    return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, defaultValue);
                }

                case TypeKind.Enum:
                {
                    var defaultFieldValue = parameterType.GetFields().FirstOrDefault(_ => _.ConstantValue is null || _.ConstantValue.ToString() == "0")?.Name;

                    var defaultValue = defaultFieldValue != null
                                       ? parameterType.Name + "." + defaultFieldValue
                                       : parameterType.Name;

                    return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultSeeCrefValue, defaultValue);
                }

                default:
                    return new Pair(Constants.AnalyzerCodeFixSharedData.DefaultSeeLangwordValue, "null");
            }
        }
    }
}