using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

using NUnit.Framework;

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
            var location4 = Location.Create(result.SyntaxTree, TextSpan.FromBounds(25, 28));

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

            Assert.Multiple(() =>
                                    {
                                        Assert.That(location1.IntersectsWith(location2), Is.False);
                                        Assert.That(location1.IntersectsWith(location3), Is.False);

                                        Assert.That(location2.IntersectsWith(location1), Is.False);
                                        Assert.That(location2.IntersectsWith(location3), Is.False);

                                        Assert.That(location3.IntersectsWith(location1), Is.False);
                                        Assert.That(location3.IntersectsWith(location2), Is.False);
                                    });
        }
    }
}