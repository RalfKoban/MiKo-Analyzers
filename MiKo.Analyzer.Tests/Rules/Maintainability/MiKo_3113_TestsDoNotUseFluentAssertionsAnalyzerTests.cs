using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3113_TestsDoNotUseFluentAssertionsAnalyzerTests : CodeFixVerifier
    {
        private static readonly object[] ReplacementsForFluentAssertions =
                                                                           {
                                                                               new object[] { "element1.Equals(element2).Should().BeTrue()", "Assert.That(element1.Equals(element2), Is.True)" },
                                                                               new object[] { @"element1.Equals(element2).Should().BeTrue(""some message"")", @"Assert.That(element1.Equals(element2), Is.True, ""some message"")" },
                                                                               new object[] { "element1.Equals(element2).Should().BeFalse()", "Assert.That(element1.Equals(element2), Is.False)" },
                                                                               new object[] { @"element1.Equals(element2).Should().BeFalse(""some message"")", @"Assert.That(element1.Equals(element2), Is.False, ""some message"")" },
                                                                               new object[] { "element1.Should().BeNull()", "Assert.That(element1, Is.Null)" },
                                                                               new object[] { @"element1.Should().BeNull(""some message"")", @"Assert.That(element1, Is.Null, ""some message"")" },
                                                                               new object[] { "element1.Should().Be(null)", "Assert.That(element1, Is.Null)" },
                                                                               new object[] { @"element1.Should().Be(null, ""some message"")", @"Assert.That(element1, Is.Null, ""some message"")" },
                                                                               new object[] { "element1.Should().NotBeNull()", "Assert.That(element1, Is.Not.Null)" },
                                                                               new object[] { @"element1.Should().NotBeNull(""some message"")", @"Assert.That(element1, Is.Not.Null, ""some message"")" },
                                                                               new object[] { "element1.Should().NotBe(null)", "Assert.That(element1, Is.Not.Null)" },
                                                                               new object[] { @"element1.Should().NotBe(null, ""some message"")", @"Assert.That(element1, Is.Not.Null, ""some message"")" },
                                                                               new object[] { "element1.Should().BeEmpty()", "Assert.That(element1, Is.Empty)" },
                                                                               new object[] { @"element1.Should().BeEmpty(""some message"")", @"Assert.That(element1, Is.Empty, ""some message"")" },
                                                                               new object[] { "element1.Should().NotBeEmpty()", "Assert.That(element1, Is.Not.Empty)" },
                                                                               new object[] { @"element1.Should().NotBeEmpty(""some message"")", @"Assert.That(element1, Is.Not.Empty, ""some message"")" },
                                                                               new object[] { "element1.Should().BeNullOrEmpty()", "Assert.That(element1, Is.Null.Or.Empty)" },
                                                                               new object[] { @"element1.Should().BeNullOrEmpty(""some message"")", @"Assert.That(element1, Is.Null.Or.Empty, ""some message"")" },
                                                                               new object[] { "element1.Should().NotBeNullOrEmpty()", "Assert.That(element1, Is.Not.Null.And.Not.Empty)" },
                                                                               new object[] { @"element1.Should().NotBeNullOrEmpty(""some message"")", @"Assert.That(element1, Is.Not.Null.And.Not.Empty, ""some message"")" },
                                                                               new object[] { "element1.Count.Should().Be(5)", "Assert.That(element1.Count, Is.EqualTo(5))" },
                                                                               new object[] { @"element1.Count.Should().Be(5, ""some message"")", @"Assert.That(element1.Count, Is.EqualTo(5), ""some message"")" },
                                                                               new object[] { @"element1.Count.Should().Be(5, reason: ""some message"")", @"Assert.That(element1.Count, Is.EqualTo(5), ""some message"")" },
                                                                               new object[] { "element1.Count.Should().NotBe(5)", "Assert.That(element1.Count, Is.Not.EqualTo(5))" },
                                                                               new object[] { @"element1.Count.Should().NotBe(5, ""some message"")", @"Assert.That(element1.Count, Is.Not.EqualTo(5), ""some message"")" },
                                                                               new object[] { @"element1.Count.Should().NotBe(5, reason: ""some message"")", @"Assert.That(element1.Count, Is.Not.EqualTo(5), ""some message"")" },
                                                                               new object[] { "element1.Should().Contain(element2)", "Assert.That(element1, Is.SupersetOf(element2))" },
                                                                               new object[] { @"element1.Should().Contain(element2, ""some message"")", @"Assert.That(element1, Is.SupersetOf(element2), ""some message"")" },
                                                                               new object[] { @"element1.Should().Contain(element2, reason: ""some message"")", @"Assert.That(element1, Is.SupersetOf(element2), ""some message"")" },
                                                                               new object[] { "element1.Should().Contain(5)", "Assert.That(element1, Does.Contain(5))" },
                                                                               new object[] { @"element1.Should().Contain(5, ""some message"")", @"Assert.That(element1, Does.Contain(5), ""some message"")" },
                                                                               new object[] { @"element1.Should().Contain(5, reason: ""some message"")", @"Assert.That(element1, Does.Contain(5), ""some message"")" },
                                                                               new object[] { @"element1.Should().Contain(_ => _ == ""abc"")", @"Assert.That(element1, Has.Some.Matches<string>(_ => _ == ""abc""))" },
                                                                               new object[] { @"element1.Should().Contain(_ => _ == ""abc"", ""some message"")", @"Assert.That(element1, Has.Some.Matches<string>(_ => _ == ""abc""), ""some message"")" },
                                                                               new object[] { @"element1.Should().Contain(_ => _ == ""abc"", reason: ""some message"")", @"Assert.That(element1, Has.Some.Matches<string>(_ => _ == ""abc""), ""some message"")" },
                                                                               new object[] { @"element1.Should().Contain(_ => _ == ""abc"", ""some {0} message"", 1)", @"Assert.That(element1, Has.Some.Matches<string>(_ => _ == ""abc""), ""some {0} message"", 1)" },
                                                                               new object[] { "element1.Should().ContainSingle()", "Assert.That(element1, Has.Exactly(1).Items)" },
                                                                               new object[] { @"element1.Should().ContainSingle(_ => _ == ""abc"")", @"Assert.That(element1, Has.One.Matches<string>(_ => _ == ""abc""))" },
                                                                               new object[] { @"element1.Should().ContainSingle(_ => _ == ""abc"", ""some message"")", @"Assert.That(element1, Has.One.Matches<string>(_ => _ == ""abc""), ""some message"")" },
                                                                               new object[] { @"element1.Should().ContainSingle(_ => _ == ""abc"", reason: ""some message"")", @"Assert.That(element1, Has.One.Matches<string>(_ => _ == ""abc""), ""some message"")" },
                                                                               new object[] { @"element1.Should().ContainSingle(_ => _ == ""abc"", ""some {0} message"", 1)", @"Assert.That(element1, Has.One.Matches<string>(_ => _ == ""abc""), ""some {0} message"", 1)" },
                                                                               new object[] { @"element1.Should().OnlyContain(_ => _ == ""abc"")", @"Assert.That(element1, Has.All.Matches<string>(_ => _ == ""abc""))" },
                                                                               new object[] { @"element1.Should().OnlyContain(_ => _ == ""abc"", ""some message"")", @"Assert.That(element1, Has.All.Matches<string>(_ => _ == ""abc""), ""some message"")" },
                                                                               new object[] { @"element1.Should().OnlyContain(_ => _ == ""abc"", reason: ""some message"")", @"Assert.That(element1, Has.All.Matches<string>(_ => _ == ""abc""), ""some message"")" },
                                                                               new object[] { "element1.Should().NotContain(5)", "Assert.That(element1, Does.Not.Contain(5))" },
                                                                               new object[] { @"element1.Should().NotContain(5, ""some message"")", @"Assert.That(element1, Does.Not.Contain(5), ""some message"")" },
                                                                               new object[] { @"element1.Should().NotContain(5, reason: ""some message"")", @"Assert.That(element1, Does.Not.Contain(5), ""some message"")" },
                                                                               new object[] { @"element1.Should().NotContain(_ => _ == ""abc"")", @"Assert.That(element1, Has.None.Matches<string>(_ => _ == ""abc""))" },
                                                                               new object[] { @"element1.Should().NotContain(_ => _ == ""abc"", ""some message"")", @"Assert.That(element1, Has.None.Matches<string>(_ => _ == ""abc""), ""some message"")" },
                                                                               new object[] { @"element1.Should().NotContain(_ => _ == ""abc"", reason: ""some message"")", @"Assert.That(element1, Has.None.Matches<string>(_ => _ == ""abc""), ""some message"")" },
                                                                               new object[] { @"element1.Should().NotContainEquivalentOf(""5"")", @"Assert.That(element1, Does.Not.Contain(""5"").IgnoreCase)" },
                                                                               new object[] { @"element1.Should().NotContainEquivalentOf(""5"", ""some message"")", @"Assert.That(element1, Does.Not.Contain(""5"").IgnoreCase, ""some message"")" },
                                                                               new object[] { @"element1.Should().NotContainEquivalentOf(""5"", reason: ""some message"")", @"Assert.That(element1, Does.Not.Contain(""5"").IgnoreCase, ""some message"")" },
                                                                               new object[] { "element1.Count.Should().BeGreaterThan(5)", "Assert.That(element1.Count, Is.GreaterThan(5))" },
                                                                               new object[] { @"element1.Count.Should().BeGreaterThan(5, ""some message"")", @"Assert.That(element1.Count, Is.GreaterThan(5), ""some message"")" },
                                                                               new object[] { @"element1.Count.Should().BeGreaterThan(5, reason: ""some message"")", @"Assert.That(element1.Count, Is.GreaterThan(5), ""some message"")" },
                                                                               new object[] { "element1.Count.Should().BeGreaterThanOrEqualTo(5)", "Assert.That(element1.Count, Is.GreaterThanOrEqualTo(5))" },
                                                                               new object[] { @"element1.Count.Should().BeGreaterThanOrEqualTo(5, ""some message"")", @"Assert.That(element1.Count, Is.GreaterThanOrEqualTo(5), ""some message"")" },
                                                                               new object[] { @"element1.Count.Should().BeGreaterThanOrEqualTo(5, reason: ""some message"")", @"Assert.That(element1.Count, Is.GreaterThanOrEqualTo(5), ""some message"")" },
                                                                               new object[] { "element1.Count.Should().BeGreaterOrEqualTo(5)", "Assert.That(element1.Count, Is.GreaterThanOrEqualTo(5))" },
                                                                               new object[] { @"element1.Count.Should().BeGreaterOrEqualTo(5, ""some message"")", @"Assert.That(element1.Count, Is.GreaterThanOrEqualTo(5), ""some message"")" },
                                                                               new object[] { @"element1.Count.Should().BeGreaterOrEqualTo(5, reason: ""some message"")", @"Assert.That(element1.Count, Is.GreaterThanOrEqualTo(5), ""some message"")" },
                                                                               new object[] { "element1.Count.Should().BeLessThan(5)", "Assert.That(element1.Count, Is.LessThan(5))" },
                                                                               new object[] { @"element1.Count.Should().BeLessThan(5, ""some message"")", @"Assert.That(element1.Count, Is.LessThan(5), ""some message"")" },
                                                                               new object[] { @"element1.Count.Should().BeLessThan(5, reason: ""some message"")", @"Assert.That(element1.Count, Is.LessThan(5), ""some message"")" },
                                                                               new object[] { "element1.Count.Should().BeLessThanOrEqualTo(5)", "Assert.That(element1.Count, Is.LessThanOrEqualTo(5))" },
                                                                               new object[] { @"element1.Count.Should().BeLessThanOrEqualTo(5, ""some message"")", @"Assert.That(element1.Count, Is.LessThanOrEqualTo(5), ""some message"")" },
                                                                               new object[] { @"element1.Count.Should().BeLessThanOrEqualTo(5, reason: ""some message"")", @"Assert.That(element1.Count, Is.LessThanOrEqualTo(5), ""some message"")" },
                                                                               new object[] { "element1.Count.Should().BeLessOrEqualTo(5)", "Assert.That(element1.Count, Is.LessThanOrEqualTo(5))" },
                                                                               new object[] { @"element1.Count.Should().BeLessOrEqualTo(5, ""some message"")", @"Assert.That(element1.Count, Is.LessThanOrEqualTo(5), ""some message"")" },
                                                                               new object[] { @"element1.Count.Should().BeLessOrEqualTo(5, reason: ""some message"")", @"Assert.That(element1.Count, Is.LessThanOrEqualTo(5), ""some message"")" },
                                                                               new object[] { "element1.Count.Should().BePositive()", "Assert.That(element1.Count, Is.Positive)" },
                                                                               new object[] { @"element1.Count.Should().BePositive(""some message"")", @"Assert.That(element1.Count, Is.Positive, ""some message"")" },
                                                                               new object[] { "element1.Count.Should().BeNegative()", "Assert.That(element1.Count, Is.Negative)" },
                                                                               new object[] { @"element1.Count.Should().BeNegative(""some message"")", @"Assert.That(element1.Count, Is.Negative, ""some message"")" },
                                                                               new object[] { "element1.Should().Equal(element2)", "Assert.That(element1, Is.EqualTo(element2))" },
                                                                               new object[] { @"element1.Should().Equal(element2, ""some message"")", @"Assert.That(element1, Is.EqualTo(element2), ""some message"")" },
                                                                               new object[] { @"element1.Should().Equal(element2, reason: ""some message"")", @"Assert.That(element1, Is.EqualTo(element2), ""some message"")" },
                                                                               new object[] { "element1.Should().BeEquivalentTo(element2)", "Assert.That(element1, Is.EquivalentTo(element2))" },
                                                                               new object[] { @"element1.Should().BeEquivalentTo(element2, ""some message"")", @"Assert.That(element1, Is.EquivalentTo(element2), ""some message"")" },
                                                                               new object[] { @"element1.Should().BeEquivalentTo(element2, reason: ""some message"")", @"Assert.That(element1, Is.EquivalentTo(element2), ""some message"")" },
                                                                               new object[] { "element1.Should().BeEquivalentTo(1, 2)", "Assert.That(element1, Is.EquivalentTo(new[] { 1, 2 }))" },
                                                                               new object[] { @"element1.Should().BeEquivalentTo(""a"", ""b"")", @"Assert.That(element1, Is.EquivalentTo(new[] { ""a"", ""b"" }))" },
                                                                               new object[] { @"element1.Should().BeEquivalentTo(""a"", ""b"", ""c"")", @"Assert.That(element1, Is.EquivalentTo(new[] { ""a"", ""b"", ""c"" }))" },
                                                                               new object[] { @"element1.Should().BeEquivalentTo(new[] { ""a"", ""b"", ""c"" }, ""some message"")", @"Assert.That(element1, Is.EquivalentTo(new[] { ""a"", ""b"", ""c"" }), ""some message"")" },
                                                                               new object[] { @"element1[0].Should().BeEquivalentTo(""a"")", @"Assert.That(element1[0], Is.EqualTo(""a"").IgnoreCase)" },
                                                                               new object[] { @"element1[0].Should().NotBeEquivalentTo(""a"")", @"Assert.That(element1[0], Is.Not.EqualTo(""a"").IgnoreCase)" },
                                                                               new object[] { "element1.Should().NotBeEquivalentTo(element2)", "Assert.That(element1, Is.Not.EquivalentTo(element2))" },
                                                                               new object[] { @"element1.Should().NotBeEquivalentTo(element2, ""some message"")", @"Assert.That(element1, Is.Not.EquivalentTo(element2), ""some message"")" },
                                                                               new object[] { @"element1.Should().NotBeEquivalentTo(element2, reason: ""some message"")", @"Assert.That(element1, Is.Not.EquivalentTo(element2), ""some message"")" },
                                                                               new object[] { "element1.Count.Should().BeInRange(1, 10)", "Assert.That(element1.Count, Is.InRange(1, 10))" },
                                                                               new object[] { @"element1.Count.Should().BeInRange(1, 10, ""some message"")", @"Assert.That(element1.Count, Is.InRange(1, 10), ""some message"")" },
                                                                               new object[] { @"element1.Count.Should().BeInRange(1, 10, reason: ""some message"")", @"Assert.That(element1.Count, Is.InRange(1, 10), ""some message"")" },
                                                                               new object[] { "element1.Count.Should().NotBeInRange(1, 10)", "Assert.That(element1.Count, Is.Not.InRange(1, 10))" },
                                                                               new object[] { @"element1.Count.Should().NotBeInRange(1, 10, ""some message"")", @"Assert.That(element1.Count, Is.Not.InRange(1, 10), ""some message"")" },
                                                                               new object[] { @"element1.Count.Should().NotBeInRange(1, 10, reason: ""some message"")", @"Assert.That(element1.Count, Is.Not.InRange(1, 10), ""some message"")" },
                                                                               new object[] { "element1.Should().HaveCount(2)", "Assert.That(element1, Has.Count.EqualTo(2))" },
                                                                               new object[] { @"element1.Should().HaveCount(2, ""some message"")", @"Assert.That(element1, Has.Count.EqualTo(2), ""some message"")" },
                                                                               new object[] { @"element1.Should().HaveCount(2, reason: ""some message"")", @"Assert.That(element1, Has.Count.EqualTo(2), ""some message"")" },
                                                                               new object[] { "element1.Should().HaveCountGreaterThan(2)", "Assert.That(element1, Has.Count.GreaterThan(2))" },
                                                                               new object[] { @"element1.Should().HaveCountGreaterThan(2, ""some message"")", @"Assert.That(element1, Has.Count.GreaterThan(2), ""some message"")" },
                                                                               new object[] { @"element1.Should().HaveCountGreaterThan(2, reason: ""some message"")", @"Assert.That(element1, Has.Count.GreaterThan(2), ""some message"")" },
                                                                               new object[] { "element1.Should().HaveCountGreaterThanOrEqualTo(2)", "Assert.That(element1, Has.Count.GreaterThanOrEqualTo(2))" },
                                                                               new object[] { @"element1.Should().HaveCountGreaterThanOrEqualTo(2, ""some message"")", @"Assert.That(element1, Has.Count.GreaterThanOrEqualTo(2), ""some message"")" },
                                                                               new object[] { @"element1.Should().HaveCountGreaterThanOrEqualTo(2, reason: ""some message"")", @"Assert.That(element1, Has.Count.GreaterThanOrEqualTo(2), ""some message"")" },
                                                                               new object[] { "element1.Should().HaveCountLessThan(2)", "Assert.That(element1, Has.Count.LessThan(2))" },
                                                                               new object[] { @"element1.Should().HaveCountLessThan(2, ""some message"")", @"Assert.That(element1, Has.Count.LessThan(2), ""some message"")" },
                                                                               new object[] { @"element1.Should().HaveCountLessThan(2, reason: ""some message"")", @"Assert.That(element1, Has.Count.LessThan(2), ""some message"")" },
                                                                               new object[] { "element1.Should().HaveCountLessThanOrEqualTo(2)", "Assert.That(element1, Has.Count.LessThanOrEqualTo(2))" },
                                                                               new object[] { @"element1.Should().HaveCountLessThanOrEqualTo(2, ""some message"")", @"Assert.That(element1, Has.Count.LessThanOrEqualTo(2), ""some message"")" },
                                                                               new object[] { @"element1.Should().HaveCountLessThanOrEqualTo(2, reason: ""some message"")", @"Assert.That(element1, Has.Count.LessThanOrEqualTo(2), ""some message"")" },
                                                                               new object[] { "element1.Should().NotHaveCount(2)", "Assert.That(element1, Has.Count.Not.EqualTo(2))" },
                                                                               new object[] { @"element1.Should().NotHaveCount(2, ""some message"")", @"Assert.That(element1, Has.Count.Not.EqualTo(2), ""some message"")" },
                                                                               new object[] { @"element1.Should().NotHaveCount(2, reason: ""some message"")", @"Assert.That(element1, Has.Count.Not.EqualTo(2), ""some message"")" },
                                                                               new object[] { "element1.Should().BeSubsetOf(element2)", "Assert.That(element1, Is.SubsetOf(element2))" },
                                                                               new object[] { @"element1.Should().BeSubsetOf(element2, ""some message"")", @"Assert.That(element1, Is.SubsetOf(element2), ""some message"")" },
                                                                               new object[] { @"element1.Should().BeSubsetOf(element2, reason: ""some message"")", @"Assert.That(element1, Is.SubsetOf(element2), ""some message"")" },
                                                                               new object[] { "element1.Should().NotBeSubsetOf(element2)", "Assert.That(element1, Is.Not.SubsetOf(element2))" },
                                                                               new object[] { @"element1.Should().NotBeSubsetOf(element2, ""some message"")", @"Assert.That(element1, Is.Not.SubsetOf(element2), ""some message"")" },
                                                                               new object[] { @"element1.Should().NotBeSubsetOf(element2, reason: ""some message"")", @"Assert.That(element1, Is.Not.SubsetOf(element2), ""some message"")" },
                                                                               new object[] { "element1.Should().BeSameAs(element2)", "Assert.That(element1, Is.SameAs(element2))" },
                                                                               new object[] { @"element1.Should().BeSameAs(element2, ""some message"")", @"Assert.That(element1, Is.SameAs(element2), ""some message"")" },
                                                                               new object[] { @"element1.Should().BeSameAs(element2, reason: ""some message"")", @"Assert.That(element1, Is.SameAs(element2), ""some message"")" },
                                                                               new object[] { "element1.Should().NotBeSameAs(element2)", "Assert.That(element1, Is.Not.SameAs(element2))" },
                                                                               new object[] { @"element1.Should().NotBeSameAs(element2, ""some message"")", @"Assert.That(element1, Is.Not.SameAs(element2), ""some message"")" },
                                                                               new object[] { @"element1.Should().NotBeSameAs(element2, reason: ""some message"")", @"Assert.That(element1, Is.Not.SameAs(element2), ""some message"")" },
                                                                               new object[] { "element1.Should().HaveValue()", "Assert.That(element1, Is.Not.Null)" },
                                                                               new object[] { @"element1.Should().HaveValue(""some message"")", @"Assert.That(element1, Is.Not.Null, ""some message"")" },
                                                                               new object[] { "element1.Should().NotHaveValue()", "Assert.That(element1, Is.Null)" },
                                                                               new object[] { @"element1.Should().NotHaveValue(""some message"")", @"Assert.That(element1, Is.Null, ""some message"")" },
                                                                               new object[] { @"element1.Count.ToString().Should().StartWith(""xyz"")", @"Assert.That(element1.Count.ToString(), Does.StartWith(""xyz""))" },
                                                                               new object[] { @"element1.Count.ToString().Should().StartWith(""xyz"", ""some message"")", @"Assert.That(element1.Count.ToString(), Does.StartWith(""xyz""), ""some message"")" },
                                                                               new object[] { @"element1.Count.ToString().Should().NotStartWith(""xyz"")", @"Assert.That(element1.Count.ToString(), Does.Not.StartWith(""xyz""))" },
                                                                               new object[] { @"element1.Count.ToString().Should().NotStartWith(""xyz"", ""some message"")", @"Assert.That(element1.Count.ToString(), Does.Not.StartWith(""xyz""), ""some message"")" },
                                                                               new object[] { @"element1.Count.ToString().Should().StartWithEquivalentOf(""xyz"")", @"Assert.That(element1.Count.ToString(), Does.StartWith(""xyz"").IgnoreCase)" },
                                                                               new object[] { @"element1.Count.ToString().Should().StartWithEquivalentOf(""xyz"", ""some message"")", @"Assert.That(element1.Count.ToString(), Does.StartWith(""xyz"").IgnoreCase, ""some message"")" },
                                                                               new object[] { @"element1.Count.ToString().Should().NotStartWithEquivalentOf(""xyz"")", @"Assert.That(element1.Count.ToString(), Does.Not.StartWith(""xyz"").IgnoreCase)" },
                                                                               new object[] { @"element1.Count.ToString().Should().NotStartWithEquivalentOf(""xyz"", ""some message"")", @"Assert.That(element1.Count.ToString(), Does.Not.StartWith(""xyz"").IgnoreCase, ""some message"")" },
                                                                               new object[] { @"element1.Count.ToString().Should().EndWith(""xyz"")", @"Assert.That(element1.Count.ToString(), Does.EndWith(""xyz""))" },
                                                                               new object[] { @"element1.Count.ToString().Should().EndWith(""xyz"", ""some message"")", @"Assert.That(element1.Count.ToString(), Does.EndWith(""xyz""), ""some message"")" },
                                                                               new object[] { @"element1.Count.ToString().Should().NotEndWith(""xyz"")", @"Assert.That(element1.Count.ToString(), Does.Not.EndWith(""xyz""))" },
                                                                               new object[] { @"element1.Count.ToString().Should().NotEndWith(""xyz"", ""some message"")", @"Assert.That(element1.Count.ToString(), Does.Not.EndWith(""xyz""), ""some message"")" },
                                                                               new object[] { @"element1.Count.ToString().Should().EndWithEquivalentOf(""xyz"")", @"Assert.That(element1.Count.ToString(), Does.EndWith(""xyz"").IgnoreCase)" },
                                                                               new object[] { @"element1.Count.ToString().Should().EndWithEquivalentOf(""xyz"", ""some message"")", @"Assert.That(element1.Count.ToString(), Does.EndWith(""xyz"").IgnoreCase, ""some message"")" },
                                                                               new object[] { @"element1.Count.ToString().Should().NotEndWithEquivalentOf(""xyz"")", @"Assert.That(element1.Count.ToString(), Does.Not.EndWith(""xyz"").IgnoreCase)" },
                                                                               new object[] { @"element1.Count.ToString().Should().NotEndWithEquivalentOf(""xyz"", ""some message"")", @"Assert.That(element1.Count.ToString(), Does.Not.EndWith(""xyz"").IgnoreCase, ""some message"")" },
                                                                               new object[] { "element1.Count.Should().BeApproximately(3.14F, 0.01F)", "Assert.That(element1.Count, Is.EqualTo(3.14F).Within(0.01F))" },
                                                                               new object[] { "element1.Count.Should().NotBeApproximately(3.14F, 0.01F)", "Assert.That(element1.Count, Is.Not.EqualTo(3.14F).Within(0.01F))" },
                                                                               new object[] { "element1.Should().BeOfType<List<string>>()", "Assert.That(element1, Is.TypeOf<List<string>>())" },
                                                                               new object[] { @"element1.Should().BeOfType<List<string>>(""some message"")", @"Assert.That(element1, Is.TypeOf<List<string>>(), ""some message"")" },
                                                                               new object[] { "element1.Should().BeOfType(typeof(List<string>))", "Assert.That(element1, Is.TypeOf(typeof(List<string>)))" },
                                                                               new object[] { @"element1.Should().BeOfType(typeof(List<string>), ""some message"")", @"Assert.That(element1, Is.TypeOf(typeof(List<string>)), ""some message"")" },
                                                                               new object[] { "element1.Should().BeXmlSerializable()", "Assert.That(element1, Is.XmlSerializable)" },
                                                                               new object[] { "element1.Should().BeBinarySerializable()", "Assert.That(element1, Is.BinarySerializable)" },
                                                                               new object[] { "element1.Should().OnlyHaveUniqueItems()", "Assert.That(element1, Is.Unique)" },
                                                                               new object[] { @"element1.Should().OnlyHaveUniqueItems(""some message"")", @"Assert.That(element1, Is.Unique, ""some message"")" },
                                                                               new object[] { "element1.Should().BeOneOf(element2)", "Assert.That(element1, Is.AnyOf(element2))" },
                                                                               new object[] { @"element1.Should().BeOneOf(element2, ""some message"")", @"Assert.That(element1, Is.AnyOf(element2), ""some message"")" },
                                                                               new object[] { "Guid.NewGuid().Should().BeEmpty()", "Assert.That(Guid.NewGuid(), Is.EqualTo(Guid.Empty))" },
                                                                               new object[] { "Guid.NewGuid().Should().NotBeEmpty()", "Assert.That(Guid.NewGuid(), Is.Not.EqualTo(Guid.Empty))" },
                                                                               new object[] { "element1.Should().BeInAscendingOrder()", "Assert.That(element1, Is.Ordered.Ascending)" },
                                                                               new object[] { @"element1.Should().BeInAscendingOrder(""some message"")", @"Assert.That(element1, Is.Ordered.Ascending, ""some message"")" },
                                                                               new object[] { "element1.Should().NotBeInAscendingOrder()", "Assert.That(element1, Is.Not.Ordered.Ascending)" },
                                                                               new object[] { @"element1.Should().NotBeInAscendingOrder(""some message"")", @"Assert.That(element1, Is.Not.Ordered.Ascending, ""some message"")" },
                                                                               new object[] { "element1.Should().BeInDescendingOrder()", "Assert.That(element1, Is.Ordered.Descending)" },
                                                                               new object[] { @"element1.Should().BeInDescendingOrder(""some message"")", @"Assert.That(element1, Is.Ordered.Descending, ""some message"")" },
                                                                               new object[] { "element1.Should().NotBeInDescendingOrder()", "Assert.That(element1, Is.Not.Ordered.Descending)" },
                                                                               new object[] { @"element1.Should().NotBeInDescendingOrder(""some message"")", @"Assert.That(element1, Is.Not.Ordered.Descending, ""some message"")" },
                                                                           };

        // for yet unknown reasons those elements cannot be fixed when available multiple times
        private static readonly object[] ReplacementsForStringFluentAssertions =
                                                                                 {
                                                                                     new object[] { "element1[0].Should().BeBlank()", "Assert.That(String.IsNullOrWhiteSpace(element1[0]), Is.True)" },
                                                                                     new object[] { "element1[0].Should().BeNullOrWhiteSpace()", "Assert.That(String.IsNullOrWhiteSpace(element1[0]), Is.True)" },
                                                                                     new object[] { "element1[0].Should().NotBeBlank()", "Assert.That(String.IsNullOrWhiteSpace(element1[0]), Is.False)" },
                                                                                     new object[] { "element1[0].Should().NotBeNullOrWhiteSpace()", "Assert.That(String.IsNullOrWhiteSpace(element1[0]), Is.False)" },
                                                                                 };

        [Test]
        public void No_issue_is_reported_for_Assert_That() => No_issue_is_reported_for(@"
using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        var element1 = new object();
        var element2 = new object();

        Assert.That(element1.Equals(element2), Is.False);
    }
}
");

        [Test]
        public void An_issue_is_reported_for_FluentAssertions_Should() => An_issue_is_reported_for(@"
using FluentAssertions;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        var element1 = new object();
        var element2 = new object();

        element1.Equals(element2).Should().BeFalse();
    }
}
");

        [TestCaseSource(nameof(ReplacementsForFluentAssertions))]
        [TestCaseSource(nameof(ReplacementsForStringFluentAssertions))]
        public void Code_gets_fixed_for_FluentAssertions_(string originalCode, string fixedCode)
        {
            const string OriginalTemplate = @"
using System;
using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        var element1 = new List<string>();
        var element2 = new List<string>();

        ###;
    }
}
";

            const string FixedTemplate = @"
using System;
using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        var element1 = new List<string>();
        var element2 = new List<string>();

        ###;
    }
}
";

            VerifyCSharpFix(OriginalTemplate.Replace("###", originalCode), FixedTemplate.Replace("###", fixedCode));
        }

        [TestCaseSource(nameof(ReplacementsForFluentAssertions))]
        public void Code_gets_fixed_for_multiple_FluentAssertions_(string originalCode, string fixedCode)
        {
            const string OriginalTemplate = @"
using System;
using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        var element1 = new List<string>();
        var element2 = new List<string>();

        ###;

        ###;

        ###;
    }
}
";

            const string FixedTemplate = @"
using System;
using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

[TestFixture]
public class TestMe
{
    [Test]
    public void DoSomething()
    {
        var element1 = new List<string>();
        var element2 = new List<string>();

        ###;

        ###;

        ###;
    }
}
";

            VerifyCSharpFix(OriginalTemplate.Replace("###", originalCode), FixedTemplate.Replace("###", fixedCode));
        }

        [Test]
        public void Code_gets_fixed_for_FluentAssertions_and_updates_using_directive()
        {
            const string OriginalCode = @"
using System;
using FluentAssertions;

public class TestMe
{
    public void DoSomething()
    {
        var element1 = 5;

        element1.Should().Be(5);
    }
}
";

            const string FixedCode = @"
using System;
using FluentAssertions;
using NUnit.Framework;

public class TestMe
{
    public void DoSomething()
    {
        var element1 = 5;

        Assert.That(element1, Is.EqualTo(5));
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_FluentAssertions_inside_simple_lambda()
        {
            const string OriginalCode = @"
using System;
using FluentAssertions;
using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        Callback<int>(x => x.Should().Be(5));
    }

    private void Callback<T>(Action<T> action)
    {
    }
}
";

            const string FixedCode = @"
using System;
using FluentAssertions;
using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        Callback<int>(x => Assert.That(x, Is.EqualTo(5)));
    }

    private void Callback<T>(Action<T> action)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_FluentAssertions_inside_singleline_parenthesized_lambda()
        {
            const string OriginalCode = @"
using System;
using FluentAssertions;
using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        Callback<int>((x) => x.Should().Be(5));
    }

    private void Callback<T>(Action<T> action)
    {
    }
}
";

            const string FixedCode = @"
using System;
using FluentAssertions;
using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        Callback<int>((x) => Assert.That(x, Is.EqualTo(5)));
    }

    private void Callback<T>(Action<T> action)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_FluentAssertions_inside_multiline_parenthesized_lambda()
        {
            const string OriginalCode = @"
using System;
using FluentAssertions;
using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        Callback<int>((x) =>
                            {
                                x = 5;
                                x.Should().Be(5);
                            });
    }

    private void Callback<T>(Action<T> action)
    {
    }
}
";

            const string FixedCode = @"
using System;
using FluentAssertions;
using NUnit.Framework;

public class TestMe
{
    [Test]
    public void DoSomething()
    {
        Callback<int>((x) =>
                            {
                                x = 5;
                                Assert.That(x, Is.EqualTo(5));
                            });
    }

    private void Callback<T>(Action<T> action)
    {
    }
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3113_TestsDoNotUseFluentAssertionsAnalyzer();

        protected override string GetDiagnosticId() => MiKo_3113_TestsDoNotUseFluentAssertionsAnalyzer.Id;

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_3113_CodeFixProvider();
    }
}