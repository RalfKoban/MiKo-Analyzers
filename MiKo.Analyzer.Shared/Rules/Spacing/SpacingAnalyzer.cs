using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    /// <summary>
    /// Provides a base class for analyzers that enforce spacing rules.
    /// </summary>
    public abstract class SpacingAnalyzer : Analyzer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpacingAnalyzer"/> class with the unique identifier of the diagnostic and the kind of symbol to analyze.
        /// </summary>
        /// <param name="diagnosticId">
        /// The unique identifier of the diagnostic.
        /// </param>
        /// <param name="symbolKind">
        /// One of the enumeration members that specifies the kind of symbol to analyze.
        /// The default is <see cref="SymbolKind.Method"/>.
        /// </param>
        protected SpacingAnalyzer(string diagnosticId, in SymbolKind symbolKind = SymbolKind.Method) : base(nameof(Spacing), diagnosticId, symbolKind)
        {
        }

        /// <summary>
        /// Creates an array of key-value pairs containing the line position information.
        /// </summary>
        /// <param name="linePosition">
        /// The line position to create the proposal for.
        /// </param>
        /// <returns>
        /// An array of key-value pairs containing the line number and character position.
        /// </returns>
        protected static Pair[] CreateProposalForLinePosition(in LinePosition linePosition) => new[]
                                                                                                   {
                                                                                                       new Pair(Constants.AnalyzerCodeFixSharedData.LineNumber, linePosition.Line.ToString("D")),
                                                                                                       new Pair(Constants.AnalyzerCodeFixSharedData.CharacterPosition, linePosition.Character.ToString("D")),
                                                                                                   };

        /// <summary>
        /// Creates an array of key-value pairs containing the spacing information.
        /// </summary>
        /// <param name="spaces">
        /// The number of spaces.
        /// </param>
        /// <param name="additionalSpaces">
        /// The number of additional spaces.
        /// The default is <c>0</c>.
        /// </param>
        /// <returns>
        /// An array of key-value pairs containing the number of spaces and additional spaces.
        /// </returns>
        protected static Pair[] CreateProposalForSpaces(in int spaces, in int additionalSpaces = 0) => new[]
                                                                                                           {
                                                                                                               new Pair(Constants.AnalyzerCodeFixSharedData.Spaces, spaces.ToString("D")),
                                                                                                               new Pair(Constants.AnalyzerCodeFixSharedData.AdditionalSpaces, additionalSpaces.ToString("D")),
                                                                                                           };
    }
}