using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class SpacingAnalyzer : Analyzer
    {
        protected SpacingAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method) : base(nameof(Spacing), diagnosticId, kind)
        {
        }

        protected static Dictionary<string, string> CreateProposalForLinePosition(LinePosition linePosition) => new Dictionary<string, string>
                                                                                                                    {
                                                                                                                        { Constants.AnalyzerCodeFixSharedData.LineNumber, linePosition.Line.ToString("D") },
                                                                                                                        { Constants.AnalyzerCodeFixSharedData.CharacterPosition, linePosition.Character.ToString("D") },
                                                                                                                    };

        protected static Dictionary<string, string> CreateProposalForSpaces(int spaces, int additionalSpaces = 0) => new Dictionary<string, string>
                                                                                                                         {
                                                                                                                             { Constants.AnalyzerCodeFixSharedData.Spaces, spaces.ToString("D") },
                                                                                                                             { Constants.AnalyzerCodeFixSharedData.AdditionalSpaces, additionalSpaces.ToString("D") },
                                                                                                                         };
    }
}