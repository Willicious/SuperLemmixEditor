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
            ID = 0;
        }

        public C.TalismanType AwardType { get; set; }
        public Dictionary<C.TalismanReq, int> Requirements { get; set; }
        public string Title { get; set; }
        public int ID { get; set; }

        public bool Equals(Talisman otherTalisman)
        {
            if (!this.Title.Equals(otherTalisman.Title)) return false;
            if (this.AwardType != otherTalisman.AwardType) return false;
            if (this.Requirements.Count != otherTalisman.Requirements.Count) return false;

            foreach (C.TalismanReq requirement in this.Requirements.Keys)
            {
                if (!otherTalisman.Requirements.ContainsKey(requirement)) return false;
                if (this.Requirements[requirement] != otherTalisman.Requirements[requirement]) return false;
            }

            return true;
        }
    }
}
