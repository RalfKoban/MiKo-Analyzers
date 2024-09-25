//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Rules
{
    public sealed class TestCaseData
    {
        public string Wrong { get; init; }

        public string Fixed { get; init; }

        public override string ToString() => $"Wrong: {Wrong}   -   Fixed: {Fixed}";
    }
}