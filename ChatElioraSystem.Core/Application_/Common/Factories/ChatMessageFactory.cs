using ChatElioraSystem.Core.Infrastructure.Models;

namespace ChatElioraSystem.Core.Application_.Common.Factories
{
    public static class ChatMessageFactory
    {
        public static IChatMessage Create(Role role, string content)
        {
            return new ChatMessage
            {
                Role = role,
                Content = content,
                Timestamp = DateTime.Now
            };
        }

        public static IChatMessage User(string content) => Create(Role.user, content );
        public static IChatMessage Assistant(string content) => Create(Role.assistant, content);
        public static IChatMessage System(string content) => Create(Role.system, content);

        public static ICollection<IChatMessage> Collection<T>(string userContent, string assistantContent, string systemContent) where T : ICollection<IChatMessage>, new()
        {
            var collectionChatMessage = new T();
            collectionChatMessage.Add(System(systemContent));
            collectionChatMessage.Add(Assistant(assistantContent));
            collectionChatMessage.Add(User(userContent));

            return collectionChatMessage;
        }
    }
}
