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
        /// Determines whether two <see cref="LinePosition"/> values are not vertically aligned.
        /// </summary>
        /// <param name="left">
        /// The first <see cref="LinePosition"/> to compare.
        /// </param>
        /// <param name="right">
        /// The second <see cref="LinePosition"/> to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> differ in both line number and character position;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        protected static bool NotVerticallyAligned(in LinePosition left, in LinePosition right) => left.Line != right.Line && left.Character != right.Character;

        /// <summary>
        /// Determines whether a given <see cref="SyntaxToken"/> is not vertically aligned based on a given <see cref="LinePosition"/>.
        /// </summary>
        /// <param name="token">
        /// The <see cref="SyntaxToken"/> to compare.
        /// </param>
        /// <param name="position">
        /// The <see cref="LinePosition"/> to compare with.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="LinePosition"/> of <paramref name="token"/> and <paramref name="position"/> differ in both line number and character position;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        protected static bool NotVerticallyAligned(in SyntaxToken token, in LinePosition position) => NotVerticallyAligned(position, token.GetStartPosition());
    }
}