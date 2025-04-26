﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MiKoSolutions.Analyzers.Rules.Spacing
{
    public abstract class SpacingAnalyzer : Analyzer
    {
        protected SpacingAnalyzer(string diagnosticId, in SymbolKind kind = SymbolKind.Method) : base(nameof(Spacing), diagnosticId, kind)
        {
        }

        protected static Pair[] CreateProposalForLinePosition(in LinePosition linePosition) => new[]
                                                                                                   {
                                                                                                       new Pair(Constants.AnalyzerCodeFixSharedData.LineNumber, linePosition.Line.ToString("D")),
                                                                                                       new Pair(Constants.AnalyzerCodeFixSharedData.CharacterPosition, linePosition.Character.ToString("D")),
                                                                                                   };

        protected static Pair[] CreateProposalForSpaces(in int spaces, in int additionalSpaces = 0) => new[]
                                                                                                           {
                                                                                                               new Pair(Constants.AnalyzerCodeFixSharedData.Spaces, spaces.ToString("D")),
                                                                                                               new Pair(Constants.AnalyzerCodeFixSharedData.AdditionalSpaces, additionalSpaces.ToString("D")),
                                                                                                           };
    }
}