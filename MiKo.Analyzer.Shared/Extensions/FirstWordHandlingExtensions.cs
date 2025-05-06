using System.Runtime.CompilerServices;

using MiKoSolutions.Analyzers.Linguistics;

//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace System
{
    internal static class FirstWordHandlingExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasSet(this FirstWordHandling value, in FirstWordHandling flag) => (value & flag) == flag;
    }
}