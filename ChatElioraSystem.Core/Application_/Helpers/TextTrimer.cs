namespace ChatElioraSystem.Core.Application_.Helpers
{
    public static class TextTrimer
    {
        public static string GetValueFromString(string napis, string stringToCut)
        {
            string słowo = string.Empty;
            bool flagToWrite = false;

            foreach (var sign in napis)
            {
                if (sign == '<')
                {
                    flagToWrite = true;
                }

                if (słowo.Contains($"<{stringToCut}>", StringComparison.InvariantCultureIgnoreCase))
                {
                    słowo = string.Empty;
                }

                if (słowo.Contains($"</{stringToCut}>", StringComparison.InvariantCultureIgnoreCase))
                {
                    return słowo.Replace($"</{stringToCut}>", string.Empty);
                }

                if (flagToWrite)
                {
                    słowo += sign;
                }
                else
                {
                    słowo = string.Empty;
                }
            }

            return "brak oceny";
        }

        public static string[] ClearThinking(string[] outPutLlms)
        {
            List<string> result = new List<string>();
            foreach (string outPutLlm in outPutLlms)
            {
                var resoning = GetValueFromString(outPutLlm, "think");
                result.Add(outPutLlm.Replace(resoning, string.Empty).Replace("<think>", string.Empty).Replace("</think>", string.Empty));
            }

            return result.ToArray();
        }

        public static string ClearThinking(string outPutLlms)
        {
            var resoning = GetValueFromString(outPutLlms, "think");
            outPutLlms = outPutLlms.Replace(resoning, string.Empty).Replace("<think>", string.Empty).Replace("</think>", string.Empty);

            return outPutLlms;
        }

        public static decimal GetValueFromOcena(string ocena)
        {
            int ocenaDec;
            ocena = ocena.Split('/')[0];
            var tryParseResult = int.TryParse(ocena, out ocenaDec);
            return ocenaDec;
        }
    }
}
