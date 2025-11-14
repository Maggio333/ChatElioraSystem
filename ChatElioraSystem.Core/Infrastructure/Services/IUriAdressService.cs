namespace ChatElioraSystem.Core.Infrastructure.Services
{
    public interface IUriAdressService
    {
        string? Ip { get; }
        public int DbVecPortRest { get; }

        //Task<Uri?> GetDbVecAdress();
        //Task<Uri?> GetLlmAdress();
        //Task<Uri?> GetLlmEmbeddingAdress();
        Uri? GetDbVecAdressRest();
        Uri? GetDbVecAdressgRPC();

        Uri? GetLlmAdress();
        Uri? GetLlmEmbeddingAdress();
    }
}