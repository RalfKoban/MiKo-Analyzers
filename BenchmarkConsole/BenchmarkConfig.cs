using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace BenchmarkConsole
{
    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            AddJob(Job.Default.WithToolchain(CsProjCoreToolchain.NetCoreApp20));
            AddJob(Job.Default.WithToolchain(CsProjCoreToolchain.NetCoreApp21));
            AddJob(Job.Default.WithToolchain(CsProjClassicNetToolchain.Net462));
            AddJob(Job.Default.WithToolchain(CsProjClassicNetToolchain.Net48));
        }
    }
}