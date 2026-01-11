using ChatElioraSystem.Core.Domain.Resources.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Domain.Resources
{
    public interface IRAGPromptJudge : IBaseRAGPrompt
    {
        string GetTheme();
        string Description { get; }
    }
}
