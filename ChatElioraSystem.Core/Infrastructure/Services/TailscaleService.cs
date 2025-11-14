using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace ChatElioraSystem.Core.Infrastructure.Services
{
    public class TailscaleService : ITailscaleService
    {
        public string? TailscaleIp { get; private set; }


        public string? DNSName { get; private set; } =
#if DEBUG
        //"10.0.2.2"; // For Android emulator

        "127.0.0.1"; // Localhost for development
#else
    // TODO: Configure your Tailscale DNS name via environment variable or configuration
    // Example: Environment.GetEnvironmentVariable("TAILSCALE_DNS") ?? "your-device.tailXXXXXX.ts.net";
    null; // Will be resolved via Tailscale CLI or should be configured
    //"your-device.tailXXXXXX.ts.net."; // Replace with your Tailscale DNS name

#endif


        public TailscaleService()
        {
            // Fire-and-forget
            _ = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            TailscaleIp = DNSName;

            if (TailscaleIp == null)
            {
                TailscaleIp = await GetTailscaleIpFromCliAsync();
            }

            if (TailscaleIp == null)
            {
                throw new Exception("Brak adresu z TailscaleIP, sprawdź CLI a najlepiej wpisz DNS z Apki web. Możesz sobie zmienić nazwy - polecam serdecznie. Gdy telefon włącz zawsze DNS");
            }

            Console.WriteLine($"[TailscaleService] IP: {TailscaleIp}");
        }

        public class TailscaleSelf
        {
            public List<string>? TailscaleIPs { get; set; }
        }

        public class TailscaleStatus
        {
            public List<string>? TailscaleIPs { get; set; }
            public TailscaleSelf? Self { get; set; }
        }


        public async Task<string?> GetTailscaleIpFromCliAsync()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "tailscale",
                    Arguments = "status --json",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = startInfo };

                StringBuilder outputBuilder = new();
                StringBuilder errorBuilder = new();

                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                        outputBuilder.AppendLine(e.Data);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                        errorBuilder.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                string output = outputBuilder.ToString();
                string error = errorBuilder.ToString();

                Console.WriteLine($"[STDOUT]: {output}");
                Console.WriteLine($"[STDERR]: {error}");

                if (!string.IsNullOrWhiteSpace(error))
                {
                    Console.WriteLine("‼️ Błąd CLI:");
                    return null;
                }

                var status = JsonConvert.DeserializeObject<TailscaleStatus>(output);
                var ip = status?.TailscaleIPs?.FirstOrDefault(ip => ip.Contains('.'))
                         ?? status?.Self?.TailscaleIPs?.FirstOrDefault(ip => ip.Contains('.'));

                return ip;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[❌ EXCEPTION]: {ex.Message}");
                return null;
            }
        }
    }
}
