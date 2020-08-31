﻿using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4102_CodeFixProvider)), Shared]
    public sealed class MiKo_4102_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_4102_TestTearDownMethodOrderingAnalyzer.Id;

        protected override string Title => Resources.MiKo_4102_MessageFormat;

        protected override SyntaxNode GetUpdatedTypeSyntax(BaseTypeDeclarationSyntax syntax)
        {
            var method = syntax.ChildNodes().OfType<MethodDeclarationSyntax>().First(_ => _.IsTestTearDownMethod());

            // to avoid line-ends before the first node, we simply create a new open brace without the problematic trivia
            var modifiedType = syntax.RemoveNodeAndAdjustOpenCloseBraces(method);

            var otherMethods = modifiedType.ChildNodes().OfType<MethodDeclarationSyntax>().ToList();

            var setup = otherMethods.FirstOrDefault(_ => _.IsTestSetUpMethod());
            var oneTimeSetup = otherMethods.FirstOrDefault(_ => _.IsTestOneTimeSetUpMethod());
            var oneTimeTearDowns = otherMethods.FirstOrDefault(_ => _.IsTestOneTimeTearDownMethod());

            var precedingNode = setup ?? oneTimeTearDowns ?? oneTimeSetup;
            if (precedingNode is null)
            {
                // place before all other nodes as there is no set-up or one-time method
                return modifiedType.InsertNodeBefore(otherMethods.First(), method);
            }

            // and we have to add the trivia to the method (as the original one belonged to the open brace token which we removed above)
            return modifiedType.InsertNodeAfter(precedingNode, method.WithLeadingEndOfLine());
        }
    }
}