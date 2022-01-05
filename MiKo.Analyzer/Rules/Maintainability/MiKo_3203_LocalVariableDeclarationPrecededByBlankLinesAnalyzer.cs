using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3203_LocalVariableDeclarationPrecededByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_3203";

        public MiKo_3203_LocalVariableDeclarationPrecededByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
        }

        private static bool IsDeclaration(StatementSyntax statement) => statement is LocalDeclarationStatementSyntax;

        private void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (LocalDeclarationStatementSyntax)context.Node;
            var issue = AnalyzeLocalDeclarationStatement(node);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeLocalDeclarationStatement(StatementSyntax declaration)
        {
            if (IsDeclaration(declaration))
            {
                foreach (var ancestor in declaration.Ancestors())
                {
                    switch (ancestor)
                    {
                        case BlockSyntax block:
                            return AnalyzeLocalDeclarationStatement(block.Statements, declaration);

                        case SwitchSectionSyntax section:
                            return AnalyzeLocalDeclarationStatement(section.Statements, declaration);

                        case IfStatementSyntax _:
                        case ElseClauseSyntax _:
                        case MethodDeclarationSyntax _:
                        case ClassDeclarationSyntax _:
                            return null; // stop lookup as there is no valid ancestor anymore
                    }
                }
            }

            return null;
        }

        private Diagnostic AnalyzeLocalDeclarationStatement(SyntaxList<StatementSyntax> statements, CSharpSyntaxNode declaration)
        {
            var callLineSpan = declaration.GetLocation().GetLineSpan();

            var noBlankLinesBefore = statements
                                     .Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                     .Any(_ => IsDeclaration(_) is false);

            if (noBlankLinesBefore)
            {
                return Issue(declaration, true, false);
            }

            return null;
        }
    }
}