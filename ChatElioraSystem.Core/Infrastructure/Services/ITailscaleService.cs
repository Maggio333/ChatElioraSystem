using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Infrastructure.Services
{
    public interface ITailscaleService
    {
        public string? TailscaleIp { get; }

        public Task InitializeAsync();
        //public Task<string?> GetTailscaleIpFromApiAsync();
        public Task<string?> GetTailscaleIpFromCliAsync();

    }

}
