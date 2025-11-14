namespace ChatElioraSystem.Core.Infrastructure.Services
{
    public class UriAdressService : IUriAdressService
    {
        private ITailscaleService _tailscaleService;
        public UriAdressService(ITailscaleService tailscaleService)
        {
            _tailscaleService = tailscaleService;
            //_tailscaleService.InitializeAsync();
        }

        public string? Ip { get; set; }

        public int DbVecPortRest { get; set; } = 6333;
        public int DbVecPortgRPC { get; set; } = 6334;



        private Uri? GetDbVecAdress(int port)
        {
            //await _tailscaleService.InitializeAsync();
            Ip = _tailscaleService.TailscaleIp;

            if (Ip != null)
            {
                return new Uri($"http://{Ip}:{port}");
            }

            return null;
        }

        public Uri? GetDbVecAdressRest() => GetDbVecAdress(DbVecPortRest);
        public Uri? GetDbVecAdressgRPC() => GetDbVecAdress(DbVecPortgRPC);

        public Uri? GetLlmAdress()
        {
            //await _tailscaleService.InitializeAsync();
            Ip = _tailscaleService.TailscaleIp;

            if (Ip != null)
            {
                return new Uri($"http://{Ip}:8123/v1/chat/completions");
            }

            return null;
        }

        public Uri? GetLlmEmbeddingAdress()
        {
            //await _tailscaleService.InitializeAsync();
            Ip = _tailscaleService.TailscaleIp;

            if (Ip != null)
            {
                return new Uri($"http://{Ip}:8123");
            }

            return null;
        }
    }
}
