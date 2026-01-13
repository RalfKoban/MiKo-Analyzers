using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2208_CodeFixProvider)), Shared]
    public sealed class MiKo_2208_CodeFixProvider : XmlTextDocumentationCodeFixProvider
    {
//// ncrunch: rdi off

        private static readonly Pair[] ReplacementMap = CreateReplacementMap();

//// ncrunch: rdi default

        public override string FixableDiagnosticId => "MiKo_2208";

        protected override XmlTextSyntax GetUpdatedSyntax(Document document, XmlTextSyntax syntax, Diagnostic issue) => GetUpdatedSyntax(syntax, issue, ReplacementMap);

//// ncrunch: rdi off

        private static Pair[] CreateReplacementMap()
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var phrase in Constants.Comments.InstanceOfPhrases)
            {
                var upperCase = phrase[0].IsUpperCase();

                dictionary.Add(phrase + "a ", upperCase ? "A " : "a ");
                dictionary.Add(phrase + "an ", upperCase ? "An " : "an ");
                dictionary.Add(phrase, phrase.FirstWord() + " ");
            }

            var map = dictionary.Select(_ => new Pair(_.Key, _.Value)).ToList();

            map.Add(new Pair("A the", "The"));
            map.Add(new Pair("An the", "The"));
            map.Add(new Pair("The the", "The"));
            map.Add(new Pair(" a the", " the"));
            map.Add(new Pair(" an the", " the"));
            map.Add(new Pair(" the the", " the"));

            map.Add(new Pair("A this", "This"));
            map.Add(new Pair("An this", "This"));
            map.Add(new Pair("The this", "This"));
            map.Add(new Pair(" a this", " this"));
            map.Add(new Pair(" an this", " this"));
            map.Add(new Pair(" the this", " this"));

            return map.ToArray();
        }

//// ncrunch: rdi default
    }
}