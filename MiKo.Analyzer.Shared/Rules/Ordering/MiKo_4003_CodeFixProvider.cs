using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4003_CodeFixProvider)), Shared]
    public sealed class MiKo_4003_CodeFixProvider : DisposeOrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzer.Id;

        protected override string Title => Resources.MiKo_4003_CodeFixTitle;

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var disposeMethod = (MethodDeclarationSyntax)syntax;

            var annotation = new SyntaxAnnotation(DisposeAnnotationKind);

            // remove method so that it can be added again
            var modifiedType = typeSyntax.RemoveNodeAndAdjustOpenCloseBraces(disposeMethod);

            var syntaxNode = FindLastCtorOrFinalizer(modifiedType);

            if (syntaxNode is null)
            {
                // none found, so insert method before first method
                var method = modifiedType.FirstChild<MethodDeclarationSyntax>();

                modifiedType = MoveRegionsWithDisposeAnnotation(modifiedType.InsertNodeBefore(method, disposeMethod.WithAnnotation(annotation)), disposeMethod);
            }
            else
            {
                // insert method after found ctor or finalizer
                modifiedType = MoveRegionsWithDisposeAnnotation(modifiedType.InsertNodeAfter(syntaxNode, disposeMethod.WithAnnotation(annotation)), disposeMethod);
            }

            return modifiedType.WithoutAnnotations(annotation);
        }

        private static SyntaxNode FindLastCtorOrFinalizer(SyntaxNode modifiedType)
        {
            SyntaxNode finalizer = modifiedType.LastChild<DestructorDeclarationSyntax>();

            return finalizer ?? modifiedType.LastChild<ConstructorDeclarationSyntax>();
        }
    }
}