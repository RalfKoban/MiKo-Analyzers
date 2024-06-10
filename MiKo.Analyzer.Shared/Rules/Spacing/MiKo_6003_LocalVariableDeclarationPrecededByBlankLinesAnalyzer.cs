using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6003_LocalVariableDeclarationPrecededByBlankLinesAnalyzer : SurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6003";

        public MiKo_6003_LocalVariableDeclarationPrecededByBlankLinesAnalyzer() : base(Id)
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

        private Diagnostic AnalyzeLocalDeclarationStatement(LocalDeclarationStatementSyntax declaration)
        {
            if (IsDeclaration(declaration))
            {
                foreach (var ancestor in declaration.Ancestors())
                {
                    switch (ancestor.Kind())
                    {
                        case SyntaxKind.Block:
                            return AnalyzeLocalDeclarationStatement(((BlockSyntax)ancestor).Statements, declaration);

                        case SyntaxKind.SwitchSection:
                            return AnalyzeLocalDeclarationStatement(((SwitchSectionSyntax)ancestor).Statements, declaration);

                        case SyntaxKind.IfStatement:
                        case SyntaxKind.ElseClause:
                            return null; // stop lookup as there is no valid ancestor anymore

                        // base methods
                        case SyntaxKind.ConversionOperatorDeclaration:
                        case SyntaxKind.ConstructorDeclaration:
                        case SyntaxKind.DestructorDeclaration:
                        case SyntaxKind.MethodDeclaration:
                        case SyntaxKind.OperatorDeclaration:
                            return null; // stop lookup as there is no valid ancestor anymore

                        // base types
                        case SyntaxKind.RecordDeclaration:
                        case SyntaxKind.ClassDeclaration:
                        case SyntaxKind.InterfaceDeclaration:
                        case SyntaxKind.StructDeclaration:
                            return null; // stop lookup as there is no valid ancestor anymore
                    }
                }
            }

            return null;
        }

        private Diagnostic AnalyzeLocalDeclarationStatement(SyntaxList<StatementSyntax> statements, LocalDeclarationStatementSyntax declaration)
        {
            var callLineSpan = declaration.GetLocation().GetLineSpan();

            var noBlankLinesBefore = statements
                                             .Where(_ => HasNoBlankLinesBefore(callLineSpan, _))
                                             .Any(_ => IsDeclaration(_) is false);

            if (noBlankLinesBefore)
            {
                return Issue(declaration.Declaration.Type, true, false);
            }

            return null;
        }
    }
}