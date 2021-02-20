using System;

//// ReSharper disable once CheckNamespace
namespace NCrunch.Framework
{
    /// <summary>
    /// Specifies that the test marked with this attribute prevents all parallel execution of all other NCrunch tests while the specific test or fixture is
    /// being executed.
    /// </summary>
    /// <remarks>
    /// Tests and fixtures marked with this attribute will not be run concurrently with any other test in the solution.
    /// This is a very broad way to prevent the concurrent execution of tests that do not support it.
    /// <para />
    /// This attribute can be applied at assembly level, in which case all tests within the assembly will be run without parallel execution.
    /// </remarks>
    public class SerialAttribute : Attribute
    {
    }
}