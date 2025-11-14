using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystem.Core.Application_.Models
{
    public class PromptsCrossWithOcenaModel
    {
        public string LlmName { get; set; }
        public string PromptAssistance { get; set; }
        public string PromptUser { get; set; }

        public List<decimal> Ocena { get; set; } = new List<decimal>();
        public List<string> OpisOceny { get; set; } = new List<string>();
        public int Id { get; set; }
    }
}
