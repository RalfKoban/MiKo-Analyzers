using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Extensions
{
    [TestFixture]
    public static class LocationExtensionsTests
    {
        [Test]
        public static void IntersectsWith_returns_true_for_intersecting_locations()
        {
            var result = SyntaxFactory.ParseName("some class with a very long name here");

            var location1 = Location.Create(result.SyntaxTree, TextSpan.FromBounds(11, 22));
            var location2 = Location.Create(result.SyntaxTree, TextSpan.FromBounds(13, 25));
            var location3 = Location.Create(result.SyntaxTree, TextSpan.FromBounds(15, 17));
            var location4 = Location.Create(result.SyntaxTree, TextSpan.FromBounds(24, 28));

            Assert.Multiple(() =>
                                 {
                                     Assert.That(location1.IntersectsWith(location1), Is.True);
                                     Assert.That(location1.IntersectsWith(location2), Is.True);
                                     Assert.That(location1.IntersectsWith(location3), Is.True);

                                     Assert.That(location2.IntersectsWith(location1), Is.True);
                                     Assert.That(location2.IntersectsWith(location2), Is.True);
                                     Assert.That(location2.IntersectsWith(location3), Is.True);
                                     Assert.That(location2.IntersectsWith(location4), Is.True);

                                     Assert.That(location3.IntersectsWith(location1), Is.True);
                                     Assert.That(location3.IntersectsWith(location2), Is.True);
                                     Assert.That(location3.IntersectsWith(location3), Is.True);

                                     Assert.That(location4.IntersectsWith(location2), Is.True);
                                 });
        }

        [Test]
        public static void IntersectsWith_returns_false_for_non_intersecting_locations()
        {
            var result = SyntaxFactory.ParseName("some class with a very long name here");

            var location1 = Location.Create(result.SyntaxTree, TextSpan.FromBounds(11, 13));
            var location2 = Location.Create(result.SyntaxTree, TextSpan.FromBounds(22, 25));
            var location3 = Location.Create(result.SyntaxTree, TextSpan.FromBounds(31, 37));

            var location4 = Location.Create(result.SyntaxTree, TextSpan.FromBounds(0, 5));
            var location5 = Location.Create(result.SyntaxTree, TextSpan.FromBounds(5, 10));

            Assert.Multiple(() =>
                                 {
                                     Assert.That(location1.IntersectsWith(location2), Is.False);
                                     Assert.That(location1.IntersectsWith(location3), Is.False);

                                     Assert.That(location2.IntersectsWith(location1), Is.False);
                                     Assert.That(location2.IntersectsWith(location3), Is.False);

                                     Assert.That(location3.IntersectsWith(location1), Is.False);
                                     Assert.That(location3.IntersectsWith(location2), Is.False);

                                     Assert.That(location4.IntersectsWith(location5), Is.False);
                                     Assert.That(location5.IntersectsWith(location4), Is.False);
                                 });
        }

        [Test]
        public static void GetSurroundingWord_returns_correct_word_when_location_is_beyond_200_characters_([Values(0, 19, 20, 40)] int repeatCount)
        {
            const string Word = "SomeWord?param=value";

            var prefix = string.Concat(Enumerable.Repeat("1234567890", repeatCount));
            var source = prefix + " " + Word + " some more suffix at the end";

            var questionMarkIndex = source.IndexOf('?');

            var tree = CSharpSyntaxTree.ParseText(source);
            var location = Location.Create(tree, TextSpan.FromBounds(questionMarkIndex, questionMarkIndex + 1));

            var result = location.GetSurroundingWord();

            Assert.That(result, Is.EqualTo(Word));
        }
    }
}