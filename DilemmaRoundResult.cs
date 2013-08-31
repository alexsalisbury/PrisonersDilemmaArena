using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonersDilemmaArena
{
    public enum PersonalResult
    {
        /// <summary>
        /// Default Value
        /// </summary>
        Unknown,

        /// <summary>
        /// Neither Defected.
        /// </summary>
        BothSilent,

        /// <summary>
        /// Defected, but cohort didn't
        /// </summary>
        SellOut,

        /// <summary>
        /// Cohort Defected, you didn't.
        /// </summary>
        SoldOut,

        /// <summary>
        /// Both confessed.
        /// </summary>
        Sloppy,
    }

    public static class RoundResultExtensions
    {
        public static bool PlayerDefected(this PersonalResult result)
        {
            return result == PersonalResult.SellOut || result == PersonalResult.Sloppy;
        }

        public static bool OpponentDefected(this PersonalResult result)
        {
            return result == PersonalResult.SoldOut || result == PersonalResult.Sloppy;
        }
    }

    public class DilemmaRoundResult
    {
        public Dictionary<Guid, PersonalResult> Results
        {
            get;
            private set;
        }

        public DilemmaRoundResult(Guid left, Guid right, bool leftDefected, bool rightDefected)
        {
            this.Results = new Dictionary<Guid, PersonalResult>();
            this.Results.Add(left, GetResult(leftDefected, rightDefected));
            this.Results.Add(right, GetResult(rightDefected, leftDefected));
        }

        public PersonalResult GetResult(bool defected, bool otherDefected)
        {
            if (defected)
            {
                return otherDefected ? PersonalResult.Sloppy : PersonalResult.SellOut;
            }
            else
            {
                return otherDefected ? PersonalResult.SoldOut : PersonalResult.BothSilent;
            }
        }

        public bool IsValid(Guid playerID)
        {
            return this.Results.Count == 2 && this.Results.ContainsKey(playerID);
        }

        public PersonalResult GetPlayerLastResult(Guid playerID)
        {
            return Results.ContainsKey(playerID) ? Results[playerID] : PersonalResult.Unknown;
        }

        internal Guid GetOpponent(Guid guid)
        {
            return this.Results.Keys.Where(r => r != guid).FirstOrDefault();
        }
    }
}
