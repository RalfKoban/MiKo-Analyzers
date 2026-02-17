using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4105_CodeFixProvider)), Shared]
    public sealed class MiKo_4105_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_4105";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<FieldDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is FieldDeclarationSyntax field)
            {
                var fields = typeSyntax.ChildNodes<FieldDeclarationSyntax>().ToList();

                if (fields.Any(_ => _.IsConst()))
                {
                    var modifiedType = typeSyntax.RemoveNodeAndAdjustOpenCloseBraces(field);
                    var nonConstField = modifiedType.ChildNodes<FieldDeclarationSyntax>().SkipWhile(_ => _.IsConst()).First();

                    return modifiedType.InsertNodeBefore(nonConstField, field);
                }

                if (field.IsStatic() && field.IsReadOnly() is false)
                {
                    if (fields.Any(_ => _.IsStatic() && _.IsReadOnly()))
                    {
                        var modifiedType = typeSyntax.RemoveNodeAndAdjustOpenCloseBraces(field);
                        var nonStaticReadOnlyField = modifiedType.ChildNodes<FieldDeclarationSyntax>().SkipWhile(_ => _.IsStatic() && _.IsReadOnly()).First();

                        return modifiedType.InsertNodeBefore(nonStaticReadOnlyField, field);
                    }
                }
            }

            return PlaceFirst<FieldDeclarationSyntax>(syntax, typeSyntax);
        }
    }
}