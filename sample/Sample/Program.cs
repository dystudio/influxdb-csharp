using InfluxDB.Collector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Collect().Wait();
        }

        static async Task Collect()
        {
            var process = Process.GetCurrentProcess();

            Metrics.Collector = new CollectorConfiguration()
                .Tag.With("host", Environment.GetEnvironmentVariable("Lenovo-PC"))
                .Tag.With("os", Environment.GetEnvironmentVariable("OS"))
                .Tag.With("process", Path.GetFileName(process.MainModule.FileName))
                .Batch.AtInterval(TimeSpan.FromSeconds(2))
                .WriteTo.InfluxDB("http://192.168.80.130:8086", "collectd")
                .CreateCollector();

            while (true)
            {
                Metrics.Increment("iterations");

                Metrics.Write("cpu_time",
                    new Dictionary<string, object>
                    {
                        { "value", process.TotalProcessorTime.TotalMilliseconds },
                        { "user", process.UserProcessorTime.TotalMilliseconds }
                    });

                Metrics.Measure("working_set", process.WorkingSet64);
                Console.WriteLine("write cup_time successfully.");
                await Task.Delay(1000);
            }
        }
    }
}
