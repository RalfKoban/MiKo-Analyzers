using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class OverallDocumentationAnalyzer : DocumentationAnalyzer
    {
        protected OverallDocumentationAnalyzer(string id) : base(id, (SymbolKind)(-1))
        {
        }

        protected static string ConstructComment(SyntaxNode comment)
        {
            var builder = new StringBuilder();

            foreach (var text in comment.DescendantNodes(_ => _.IsCode() is false, true).OfType<XmlTextSyntax>())
            {
                builder.Append(' ').Append(text.WithoutXmlCommentExterior()).Append(' ');
            }

            var result = builder.ToString().Trim();

            return result;
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field, SymbolKind.TypeParameter);
    }
}