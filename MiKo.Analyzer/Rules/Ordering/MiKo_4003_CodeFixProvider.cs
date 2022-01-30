﻿using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4003_CodeFixProvider)), Shared]
    public sealed class MiKo_4003_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzer.Id;

        protected override string Title => Resources.MiKo_4003_CodeFixTitle;

        protected override SyntaxNode GetUpdatedTypeSyntax(CodeFixContext context, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var disposeMethod = (MethodDeclarationSyntax)syntax;

            // remove method so that it can be added again
            var modifiedType = typeSyntax.RemoveNodeAndAdjustOpenCloseBraces(disposeMethod);

            var syntaxNode = FindLastCtorOrFinalizer(modifiedType);
            if (syntaxNode is null)
            {
                // none found, so insert method before first method
                var method = modifiedType.FirstChild<MethodDeclarationSyntax>();

                return modifiedType.InsertNodeBefore(method, disposeMethod);
            }

            // insert method after found ctor or finalizer
            return modifiedType.InsertNodeAfter(syntaxNode, disposeMethod);
        }

        private static SyntaxNode FindLastCtorOrFinalizer(SyntaxNode modifiedType)
        {
            SyntaxNode ctor = modifiedType.LastChild<ConstructorDeclarationSyntax>();
            SyntaxNode finalizer = modifiedType.LastChild<DestructorDeclarationSyntax>();

            return finalizer ?? ctor;
        }
    }
}