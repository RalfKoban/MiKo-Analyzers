using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer : ReturnTypeDefaultPhraseAnalyzer
    {
        public const string Id = "MiKo_2032";

        public MiKo_2032_BooleanReturnTypeDefaultPhraseAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeReturnType(ISymbol owningSymbol, ITypeSymbol returnType, DocumentationCommentTriviaSyntax comment, string commentXml, string xmlTag)
        {
            var startingPhrases = GetStartingPhrases(owningSymbol, returnType);
            var endingPhrases = GetEndingPhrases(returnType);

            const StringComparison Comparison = StringComparison.Ordinal;

            if (commentXml.StartsWithAny(startingPhrases, Comparison) && commentXml.ContainsAny(endingPhrases, Comparison))
            {
                // nothing to do here
                return Enumerable.Empty<Diagnostic>();
            }

            var syntaxNode = comment.FirstChild<XmlElementSyntax>(_ => _.GetName() == xmlTag);

            if (syntaxNode is null)
            {
                // seems like returns is inside the summary tag
                syntaxNode = comment.FirstDescendant<XmlElementSyntax>(_ => _.GetName() == xmlTag);
            }

            return new[] { Issue(owningSymbol.Name, syntaxNode.GetContentsLocation(), xmlTag, startingPhrases[0], endingPhrases[0]) };
        }

        // ReSharper disable once RedundantNameQualifier
        protected override bool IsAcceptedType(ITypeSymbol returnType) => returnType.IsBoolean();

        protected override string[] GetStartingPhrases(ISymbol owningSymbol, ITypeSymbol returnType)
        {
            if (IsAcceptedType(returnType))
            {
                var hasPropertySetter = owningSymbol is IPropertySymbol property && property.IsReadOnly is false;

                return hasPropertySetter
                       ? Constants.Comments.BooleanPropertySetterStartingPhrase
                       : Constants.Comments.BooleanReturnTypeStartingPhrase;
            }

            return Constants.Comments.BooleanTaskReturnTypeStartingPhrase;
        }

        private string[] GetEndingPhrases(ITypeSymbol returnType) => IsAcceptedType(returnType)
                                                                     ? Constants.Comments.BooleanReturnTypeEndingPhrase
                                                                     : Constants.Comments.BooleanTaskReturnTypeEndingPhrase;
    }
}