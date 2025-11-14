using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Domain.Services
{
    public interface IStoragePathProvider
    {
        string GetMemoryDirectory();
    }
}
