using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_6001_LogStatementSurroundedByBlankLinesAnalyzer : CallSurroundedByBlankLinesAnalyzer
    {
        public const string Id = "MiKo_6001";

        public MiKo_6001_LogStatementSurroundedByBlankLinesAnalyzer() : base(Id)
        {
        }

        protected override bool IsApplicable(CompilationStartAnalysisContext context)
        {
            if (context.Compilation.GetTypeByMetadataName(Constants.ILog.FullTypeName) != null)
            {
                return true;
            }

            if (context.Compilation.GetTypeByMetadataName(Constants.SeriLog.FullTypeName) != null)
            {
                return true;
            }

            if (context.Compilation.GetTypeByMetadataName(Constants.MicrosoftLogging.FullTypeName) != null)
            {
                return true;
            }

            return false;
        }

        // it may happen that in some broken code Roslyn is unable to detect a type (e.g. due to missing code paths), hence 'type' could be null here
        protected override bool IsCall(ITypeSymbol type)
        {
            switch (type?.Name)
            {
                case Constants.ILog.TypeName when type.ContainingNamespace.Name == Constants.ILog.NamespaceName:
                case Constants.SeriLog.TypeName when type.ContainingNamespace.Name == Constants.SeriLog.NamespaceName:
                case Constants.MicrosoftLogging.TypeName when type.ContainingNamespace.FullyQualifiedName() == Constants.MicrosoftLogging.NamespaceName:
                    return true;

                default:
                    return false;
            }
        }
    }
}