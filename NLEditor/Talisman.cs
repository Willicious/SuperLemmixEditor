using System;
using System.Collections.Generic;

namespace NLEditor
{
    class Talisman : IEquatable<Talisman>
    {
        public Talisman()
        {
            AwardType = C.TalismanType.Bronze;
            Requirements = new Dictionary<C.TalismanReq, int>();
            Title = string.Empty;
            ID = -1;
        }

        public C.TalismanType AwardType { get; set; }
        public Dictionary<C.TalismanReq, int> Requirements { get; set; }
        public string Title { get; set; }
        public int ID { get; set; }

        public bool Equals(Talisman otherTalisman)
        {
            if (!this.Title.Equals(otherTalisman.Title))
                return false;
            if (this.AwardType != otherTalisman.AwardType)
                return false;
            if (this.Requirements.Count != otherTalisman.Requirements.Count)
                return false;

            foreach (C.TalismanReq requirement in this.Requirements.Keys)
            {
                if (!otherTalisman.Requirements.ContainsKey(requirement))
                    return false;
                if (this.Requirements[requirement] != otherTalisman.Requirements[requirement])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the text of the requirement to be displayed on the form.
        /// </summary>
        /// <param name="requirement"></param>
        public string GetRequirementText(C.TalismanReq requirement)
        {
            if (!Requirements.ContainsKey(requirement))
                return string.Empty;

            if (requirement == C.TalismanReq.TimeLimit)
            {
                int min = Requirements[requirement] / 17 / 60;
                int sec = (Requirements[requirement] / 17) % 60;
                return C.TalismanReqText[requirement] + ": " + min.ToString() + ":" + sec.ToString();
            }
            else if (requirement == C.TalismanReq.UseOnlySkill)
            {
                return C.TalismanReqText[requirement] + ": " + C.TalismanSkills[Requirements[requirement]];
            }
            else if (requirement == C.TalismanReq.KillZombies ||
                     requirement == C.TalismanReq.ClassicMode ||
                     requirement == C.TalismanReq.NoPause)
            {
                return C.TalismanReqText[requirement];
            }
            else
            {
                return C.TalismanReqText[requirement] + ": " + Requirements[requirement].ToString();
            }
        }
    }
}
