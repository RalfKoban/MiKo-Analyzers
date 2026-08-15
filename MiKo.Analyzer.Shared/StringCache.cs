using System.Collections.Concurrent;

//// ncrunch: rdi off
//// ncrunch: no coverage start
namespace MiKoSolutions.Analyzers
{
    internal static class StringCache
    {
        private static readonly ConcurrentDictionary<string, string> Cache = new ConcurrentDictionary<string, string>();

        internal static string Intern(string text) => text is null ? null : Cache.GetOrAdd(text, text);
    }
}