using System.Collections.Generic;

namespace pcrpg.src.Gameplay.Actor
{
    #region Dialogue Class
    public class Dialogue
    {
        public List<string> Messages { get; }

        public List<string> Answers { get; }

        public Dialogue(List<string> messages = null, List<string> answers = null)
        {
            Messages = messages ?? new List<string>();

            Answers = answers ?? new List<string>();
        }
    }
    #endregion
}
