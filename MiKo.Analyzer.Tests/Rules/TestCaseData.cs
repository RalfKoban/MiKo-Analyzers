namespace MiKoSolutions.Analyzers.Rules
{
    public sealed class TestCaseData
    {
        public string Wrong { get; set; }

        public string Fixed { get; set; }

        public override string ToString() => $"Wrong: {Wrong}   -   Fixed: {Fixed}";
    }
}