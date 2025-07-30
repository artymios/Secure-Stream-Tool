using System;
using System.Management;
using System.Diagnostics;

class AutomationTool
{
    static void Main()
    {
        Console.WriteLine("Monitoring for obs64.exe start/stop...");

        var startWatcher = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
        var stopWatcher = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStopTrace");

        startWatcher.EventArrived += (sender, e) =>
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            Console.WriteLine($"Process started: {processName}");

            if (processName.Equals("obs64.exe", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("OBS detected. Enabling Do Not Disturb...");
                EnableDoNotDisturb();
            }
        };

        stopWatcher.EventArrived += (sender, e) =>
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            Console.WriteLine($"Process stopped: {processName}");

            if (processName.Equals("obs64.exe", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("OBS exited. Disabling Do Not Disturb...");
                DisableDoNotDisturb();
            }
        };

        startWatcher.Start();
        stopWatcher.Start();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        startWatcher.Stop();
        stopWatcher.Stop();
    }

    static void EnableDoNotDisturb()
    {
        RunPowerShellScript("on");
    }

    static void DisableDoNotDisturb()
    {
        RunPowerShellScript("off");
    }

    static void RunPowerShellScript(string state)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-ExecutionPolicy Bypass -File \"toggle-focus.ps1\" {state}",
            UseShellExecute = false,
            CreateNoWindow = true
        };
        Process.Start(startInfo);
    }
}