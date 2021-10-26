using NUnit.Framework;

#if NCRUNCH
[assembly: Timeout(10 * 1000)] // default timeout of 10 seconds
#else
[assembly: Timeout(60 * 1000)] // default timeout of 60 seconds
#endif