namespace ChatElioraSystem.Core.Domain.Resources
{
    public interface IRAGPromptsDbVec
    {
        public string ReadDbVectorPrompt { get; }
        public string Role { get; }
        public string SaveToDbVectorPrompt { get; }
        public string DbVectorPrompt { get; }

        //string DbVecName { get; }

        //int TopK { get; }
        //string UnicodeAction { get; }

        //public string GetOnlyReadPrompt { get; }

        //public string GetOnlySavePrompt { get; }
    }
}