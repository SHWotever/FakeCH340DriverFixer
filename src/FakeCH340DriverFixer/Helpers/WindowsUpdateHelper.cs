using System;
using WUApiLib;

namespace FakeCH340DriverFixer
{
    internal static class WindowsUpdateHelper
    {
        public static bool BlockUpdatesFor(string deviceId)
        {
            return EnumerateUpdatesFor(deviceId, driverUpdate =>
            {
                Console.Write($"Found pending update {driverUpdate.Title} {driverUpdate.DriverVerDate} : ");
                if (driverUpdate.IsHidden)
                {
                    Console.WriteLine("Update was already hidden, skipping");
                }
                else
                {
                    Console.WriteLine("Update was not hidden, hiding it");
                    driverUpdate.IsHidden = true;
                }
            });
        }

        public static bool UnblockUpdatesFor(string deviceId)
        {
            return EnumerateUpdatesFor(deviceId, driverUpdate =>
            {
                Console.Write($"Found pending update {driverUpdate.Title} {driverUpdate.DriverVerDate} : ");
                if (!driverUpdate.IsHidden)
                {
                    Console.WriteLine("Update was not hidden, skipping");
                }
                else
                {
                    Console.WriteLine("Update was hidden, unhiding it");
                    driverUpdate.IsHidden = false;
                }
            });
        }

        private static bool EnumerateUpdatesFor(string deviceId, Action<IWindowsDriverUpdate> action)
        {
            Console.WriteLine($"Looking for pending updates ...");
            IUpdateSession _updateSession = (IUpdateSession)Activator.CreateInstance(Type.GetTypeFromProgID("Microsoft.Update.Session"));
            IUpdateSearcher searcher = _updateSession.CreateUpdateSearcher();
            bool updateFound = false;
            var updates = searcher.Search("IsInstalled = 0");
            foreach (IUpdate update in updates.Updates)
            {
                if (update is IWindowsDriverUpdate driverUpdate)
                {
                    if (driverUpdate.DriverHardwareID.Equals(deviceId, StringComparison.OrdinalIgnoreCase))
                    {
                        action(driverUpdate);

                        updateFound = true;
                    }
                }
            }

            return updateFound;
        }
    }
}