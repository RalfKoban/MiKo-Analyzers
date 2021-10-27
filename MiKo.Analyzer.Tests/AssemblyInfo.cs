using NUnit.Framework;

#if NCRUNCH
[assembly: Timeout(15 * 1000)] // default timeout of 15 seconds
#else
[assembly: Timeout(60 * 1000)] // default timeout of 60 seconds
#endif