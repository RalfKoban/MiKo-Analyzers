using NUnit.Framework;

#if NCRUNCH

[assembly: Timeout(90 * 1000)] // default timeout of 90 seconds

#else

[assembly: Timeout(60 * 1000)] // default timeout of 60 seconds
[assembly: FixtureLifeCycle(LifeCycle.SingleInstance)]
[assembly: Parallelizable(ParallelScope.All)]

#endif