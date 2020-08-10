namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    public abstract class MaintainabilityCodeFixProvider : MiKoCodeFixProvider
    {
        protected MaintainabilityCodeFixProvider() : base(false)
        {
        }
    }
}