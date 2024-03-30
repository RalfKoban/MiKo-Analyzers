using NUnit.Framework;

#if NCRUNCH

[assembly: Timeout(45 * 1000)] // default timeout of 45 seconds

#else

[assembly: Timeout(60 * 1000)] // default timeout of 60 seconds
[assembly: FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[assembly: Parallelizable(ParallelScope.All)]

#endif