using NUnit.Framework;

#if NCRUNCH
[assembly: Timeout(20 * 1000)] // default timeout of 20 seconds
#else
[assembly: Timeout(60 * 1000)] // default timeout of 60 seconds
#endif