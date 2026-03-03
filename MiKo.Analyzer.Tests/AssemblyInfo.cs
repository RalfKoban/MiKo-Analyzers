using NUnit.Framework;

#if NCRUNCH

// NUnit 4.5 no longer supports the TimeoutAttribute
// [assembly: Timeout(90 * 1000)] // default timeout of 90 seconds

#else

// NUnit 4.5 no longer supports the TimeoutAttribute
// [assembly: Timeout(60 * 1000)] // default timeout of 60 seconds
[assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[assembly: Parallelizable(ParallelScope.All)]

#endif