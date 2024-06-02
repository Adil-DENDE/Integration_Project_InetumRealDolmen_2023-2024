using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireMock.Logging;
using WireMock.Server;
using WireMock.Settings;

namespace Groupservice.MockServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var server = WireMockServer.Start(new WireMockServerSettings
            {
                Urls = new[] { "http://localhost:9090" },
                ReadStaticMappings = true,
                Logger = new WireMockConsoleLogger()
            });
            Console.WriteLine($"Server running at {server.Urls[0]}");
            Console.ReadKey();
            server.Stop();

        }
    }
}
