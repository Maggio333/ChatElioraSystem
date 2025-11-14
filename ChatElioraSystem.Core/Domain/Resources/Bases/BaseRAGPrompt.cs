using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Domain.Resources.Bases
{
    public abstract class BaseRAGPrompt : IBaseRAGPrompt
    {
        public string Role => string.Empty;
    }
}
