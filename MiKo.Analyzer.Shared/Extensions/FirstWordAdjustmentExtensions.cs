using System.Runtime.CompilerServices;

using MiKoSolutions.Analyzers.Linguistics;

//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace System
{
    internal static class FirstWordAdjustmentExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasSet(this FirstWordAdjustment value, in FirstWordAdjustment flag) => (value & flag) == flag;
    }
}