using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public static class NamesFinderTests
    {
        [TestCase(' ', ExpectedResult = "SPACE")]
        [TestCase('!', ExpectedResult = "EXCLAMATION_MARK")]
        [TestCase('"', ExpectedResult = "QUOTATION_MARK")]
        [TestCase('#', ExpectedResult = "HASH")]
        [TestCase('$', ExpectedResult = "DOLLAR")]
        [TestCase('%', ExpectedResult = "PERCENT")]
        [TestCase('&', ExpectedResult = "AMPERSAND")]
        [TestCase('(', ExpectedResult = "OPENING_PARENTHESIS")]
        [TestCase(')', ExpectedResult = "CLOSING_PARENTHESIS")]
        [TestCase('*', ExpectedResult = "ASTERIX")]
        [TestCase('-', ExpectedResult = "MINUS")]
        [TestCase(',', ExpectedResult = "COMMA")]
        [TestCase('.', ExpectedResult = "DOT")]
        [TestCase('/', ExpectedResult = "SLASH")]
        [TestCase(':', ExpectedResult = "COLON")]
        [TestCase(';', ExpectedResult = "SEMICOLON")]
        [TestCase('?', ExpectedResult = "QUESTION_MARK")]
        [TestCase('@', ExpectedResult = "AT")]
        [TestCase('[', ExpectedResult = "OPENING_BRACKET")]
        [TestCase('\'', ExpectedResult = "APOSTROPHE")]
        [TestCase('\\', ExpectedResult = "BACKSLASH")]
        [TestCase(']', ExpectedResult = "CLOSING_BRACKET")]
        [TestCase('_', ExpectedResult = "UNDERLINE")]
        [TestCase('{', ExpectedResult = "CLOSING_BRACE")]
        [TestCase('}', ExpectedResult = "OPENING_BRACE")]
        [TestCase('~', ExpectedResult = "TILDE")]
        [TestCase('€', ExpectedResult = "EURO")]
        [TestCase('+', ExpectedResult = "PLUS")]
        [TestCase('<', ExpectedResult = "OPENING_CHEVRON")]
        [TestCase('=', ExpectedResult = "EQUALS")]
        [TestCase('>', ExpectedResult = "CLOSING_CHEVRON")]
        [TestCase('§', ExpectedResult = "PARAGRAPH")]
        [TestCase('a', ExpectedResult = null)]
        public static string FindDescribingWord_returns_describing_word_for_(char input) => NamesFinder.FindDescribingWord(input);
    }
}