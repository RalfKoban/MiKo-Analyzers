using System.Linq;

using TestHelper;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingLengthAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] All =
                                               {
                                                   "A",
                                                   "Ab",
                                                   "Abc",
                                                   "Abcd",
                                                   "Abcde",
                                                   "Abcdef",
                                                   "Abcdefg",
                                                   "Abcdefgh",
                                                   "Abcdefghi",
                                                   "Abcdefghij",
                                                   "Abcdefghijk",
                                                   "Abcdefghijkl",
                                                   "Abcdefghijklm",
                                                   "Abcdefghijklmn",
                                                   "Abcdefghijklmno",
                                                   "Abcdefghijklmnop",
                                                   "Abcdefghijklmnopq",
                                                   "Abcdefghijklmnopqr",
                                                   "Abcdefghijklmnopqrs",
                                                   "Abcdefghijklmnopqrst",
                                                   "Abcdefghijklmnopqrstu",
                                                   "Abcdefghijklmnopqrstuv",
                                                   "Abcdefghijklmnopqrstuvw",
                                                   "Abcdefghijklmnopqrstuvwx",
                                                   "Abcdefghijklmnopqrstuvwxy",
                                                   "Abcdefghijklmnopqrstuvwxyz",
                                                   "Abcdefghijklmnopqrstuvwxyz01234567890äöü",
                                                   "Abcdefghijklmnopqrstuvwxyz01234567890äöüß",
                                               };

        protected static string[] GetAllWithMaxLengthOf(int length) => All.Where(_ => _.Length <= length).ToArray();

        protected static string[] GetAllAboveLengthOf(int length) => All.Where(_ => _.Length > length).ToArray();
    }
}