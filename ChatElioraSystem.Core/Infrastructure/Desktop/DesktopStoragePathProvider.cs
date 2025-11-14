using ChatElioraSystem.Core.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Infrastructure.Desktop
{
    public class DesktopStoragePathProvider : IStoragePathProvider
    {
        public string GetMemoryDirectory()
        { 
            // Use application base directory and create Memory subdirectory
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(baseDirectory, "Memory");
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
