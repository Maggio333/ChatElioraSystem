using ChatElioraSystem.Core.Domain.Resources.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Domain.Resources
{
    internal class RAGPromptReflectionExample : BaseRAGPrompt, IRAGPromptReflection
    {
        public string Theme => "dummy";

        public string Emotional => "dummy";
    }
}
