﻿using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace MiKoSolutions.Analyzers
{
    internal static class LocationExtensions
    {
        internal static T GetEnclosing<T>(this Location value, SemanticModel semanticModel) where T : class, ISymbol
        {
            var node = value.SourceTree?.GetRoot().FindNode(value.SourceSpan);

            return node.GetEnclosingSymbol(semanticModel) as T;
        }

        internal static LinePosition GetStartPosition(this Location value) => value.GetLineSpan().StartLinePosition;

        internal static LinePosition GetEndPosition(this Location value) => value.GetLineSpan().EndLinePosition;

        internal static int GetPositionWithinStartLine(this Location value) => value.GetStartPosition().Character;

        internal static int GetPositionWithinEndLine(this Location value) => value.GetEndPosition().Character;

        internal static int GetStartingLine(this Location value) => value.GetStartPosition().Line;

        internal static int GetEndingLine(this Location value) => value.GetEndPosition().Line;

        internal static string GetText(this Location value) => value.SourceTree?.GetText().ToString(value.SourceSpan);

        internal static string GetText(this Location value, in int length)
        {
            var tree = value.SourceTree;

            if (tree is null)
            {
                return null;
            }

            var start = value.SourceSpan.Start;

            return tree.GetText().ToString(TextSpan.FromBounds(start, start + length));
        }

        internal static string GetSurroundingWord(this Location value)
        {
            var tree = value.SourceTree;

            if (tree is null)
            {
                return null;
            }

            var sourceText = tree.GetText();
            var text = sourceText.ToString(TextSpan.FromBounds(0, value.SourceSpan.End));

            var lastIndexOfFirstSpace = text.LastIndexOfAny(Constants.WhiteSpaceCharacters);

            if (lastIndexOfFirstSpace is -1)
            {
                return null;
            }

            var followUpText = sourceText.GetSubText(value.SourceSpan.End).ToString();

            var firstIndexOfNextSpace = followUpText.StartsWith('<') // seems like the comment finished
                                        ? 0
                                        : followUpText.IndexOfAny(Constants.WhiteSpaceCharacters);

            if (firstIndexOfNextSpace is -1)
            {
                return null;
            }

            return sourceText.ToString(TextSpan.FromBounds(lastIndexOfFirstSpace + 1, text.Length + firstIndexOfNextSpace));
        }

        internal static bool Contains(this Location value, Location other)
        {
            var valueSpan = value.SourceSpan;
            var otherSpan = other.SourceSpan;

            return valueSpan.Start <= otherSpan.Start && otherSpan.End <= valueSpan.End;
        }

        internal static bool IntersectsWith(this Location value, Location other)
        {
            var valueSpan = value.SourceSpan;
            var otherSpan = other.SourceSpan;

            return valueSpan.Start < otherSpan.End && otherSpan.Start < valueSpan.End;
        }
    }
}
