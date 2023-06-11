using FakeCH340DriverFixer.Helpers;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace FakeCH340DriverFixer
{
    internal partial class Program
    {
        private static void Main(string[] args)
        {
            // Device id to be handled (CH340)
            var deviceId = "USB\\VID_1A86&PID_7523";

            // Guilty driver version to be uninstalled
            var driverVersion = "3.8.2023.02";

            if (args.Length > 0 && args[0].Equals("unblock", StringComparison.OrdinalIgnoreCase))
            {
                if (!WindowsUpdateHelper.UnblockUpdatesFor(deviceId))
                {
                    Console.WriteLine("Could not find pending driver updates");
                }
            }
            else
            {
                if (AnalyzePorts())
                {
                    Console.WriteLine($"Installing default driver ...");
                    InstallDefaultDriver();

                    if (!OEMDriversHelper.UninstallDriverVersion(deviceId, driverVersion))
                    {
                        Console.WriteLine($"Driver non compatible with fake CH340 not found");
                    }

                    if (!WindowsUpdateHelper.BlockUpdatesFor(deviceId))
                    {
                        Console.WriteLine("Could not find pending driver updates");
                    }
                }
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static bool AnalyzePorts()
        {
            Console.WriteLine("Checking ports :");

            var CH340GPorts = SerialPortDescriptionHelper.GetSerialPorts().Where(i => i.IsCH340).ToList();
            foreach (var port in CH340GPorts)
            {
                Console.WriteLine($"\tFound CH340G on port {port.PortName}, driver : {port.DriverVersion} ({port.DriverInf}) : likely to be {(port.IsFakeCH340 ? "Fake" : "Legit")}");
            }

            if (!CH340GPorts.Any())
            {
                Console.WriteLine($"\tNo CH340G found, please make sure to plug it first.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private static void InstallDefaultDriver()
        {
            var temp2 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            System.IO.Directory.CreateDirectory(temp2);
            ZipFile.ExtractToDirectory(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "CH340G_2019.zip"), temp2);
            string inffile = Path.Combine(temp2, "CH341ser.Inf");
            PNPUtilHelper.InstallDriver(inffile);
        }
    }
}