using System.Buffers.Text;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    public abstract class DocumentationCodeFixProvider : MiKoCodeFixProvider
    {
        protected DocumentationCodeFixProvider() : base(false)
        {
        }
    }
}