﻿using System;
using System.Collections.Generic;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static class StringSplitExtensionsTests
    {
        private const string Line1 = "This is a test to see if everything is working";
        private const string Line2 = "Another line. Some more text.";

        private const string Text = $"{Line1}\r\n{Line2}";

        [Test]
        public static void SplitBy_splits_text_by_words()
        {
            var words = new List<string>();

            foreach (ReadOnlySpan<char> line in Text.AsSpan().SplitBy(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (var item in line.SplitBy(" "))
                {
                    words.Add(item.ToString());
                }
            }

            Assert.That(words, Is.EqualTo(new[] { "This", "is", "a", "test", "to", "see", "if", "everything", "is", "working", "Another", "line.", "Some", "more", "text." }));
        }

        [Test]
        public static void SplitBy_splits_text_by_lines_and_keeps_empty_lines()
        {
            var lines = Text.AsSpan().SplitBy(Environment.NewLine);
            var count = lines.Count();

            Assert.That(count, Is.EqualTo(3)); // additional 'empty' line between both lines

            var foundLines = new List<string>();

            foreach (var line in lines)
            {
                foundLines.Add(line.ToString());
            }

            Assert.That(foundLines.Count, Is.EqualTo(3));
            Assert.That(foundLines[0], Is.EqualTo(Line1));
            Assert.That(foundLines[1], Is.EqualTo(string.Empty)); // line between \r and \n
            Assert.That(foundLines[2], Is.EqualTo(Line2));
        }

        [Test]
        public static void SplitBy_splits_text_by_lines_and_removes_empty_lines()
        {
            var lines = Text.AsSpan().SplitBy(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            var count = lines.Count();

            Assert.That(count, Is.EqualTo(2));

            var foundLines = new List<string>();

            foreach (var line in lines)
            {
                foundLines.Add(line.ToString());
            }

            Assert.That(foundLines.Count, Is.EqualTo(2));
            Assert.That(foundLines[0], Is.EqualTo(Line1));
            Assert.That(foundLines[1], Is.EqualTo(Line2));
        }

        [Test]
        public static void SplitBy_splits_text_by_lines_and_removes_empty_lines_MultiFindings()
        {
            var lines = "something text to split".AsSpan().SplitBy([" text "], options: StringSplitOptions.RemoveEmptyEntries);

            Assert.That(lines.Count, Is.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("something"));
            Assert.That(lines[1], Is.EqualTo("to split"));
        }
    }
}