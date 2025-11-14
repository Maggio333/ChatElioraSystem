using ChatElioraSystem.Core.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatElioraSystemMobile.Template
{
    public class ChatMessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UserTemplate { get; set; }
        public DataTemplate AiTemplate { get; set; }
        public DataTemplate DbTemplate { get; set; }
        public DataTemplate ToolTemplate { get; set; }
        public DataTemplate SystemTemplate { get; set; }


        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is not IChatMessage message)
                return AiTemplate;

            if (message.IsUser)
                return UserTemplate;

            if (message.IsDbAction)
                return DbTemplate;

            if (message.IsTool)
                return ToolTemplate;

            if(message.IsSystem)
                return SystemTemplate;

            return AiTemplate;
        }
    }

}
