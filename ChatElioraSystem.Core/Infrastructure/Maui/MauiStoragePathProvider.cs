using ChatElioraSystem.Core.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Infrastructure.Maui
{
    public class MauiStoragePathProvider : IStoragePathProvider
    {
        public string GetMemoryDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    }
}
