using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace BenchmarkConsole
{
    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            AddJob(Job.Default.With(CsProjCoreToolchain.NetCoreApp20));
            AddJob(Job.Default.With(CsProjCoreToolchain.NetCoreApp21));
            AddJob(Job.Default.With(CsProjClassicNetToolchain.Net462));
            AddJob(Job.Default.With(CsProjClassicNetToolchain.Net48));
        }
    }
}