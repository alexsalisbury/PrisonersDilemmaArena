using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonersDilemmaArena
{
    public interface IPrisoner
    {
        bool DemandConfession(PersonalResult lastRound);
        Guid GetName();
        string GetStrategy();
    }

    public abstract class Prisoner : IPrisoner
    {
        public string Strategy
        {
            get;
            private set;
        }

        public Guid Name
        {
            get;
            private set;
        }

        public Guid GetName()
        {
            return this.Name;
        }

        public string GetStrategy()
        {
            return this.Strategy;
        }

        protected Prisoner(string type)
        {
            this.Name = Guid.NewGuid();
            this.Strategy = type;
        }

        public abstract bool DemandConfession(PersonalResult lastRound);
    }

    /// <summary>
    /// Prisoner that always "turns the other cheek"
    /// </summary>
    public class Jesus : Prisoner
    {
        public Jesus() : base("Jesus") { }

        /// <summary>
        /// Demand the prisoner to make a deal and confess.
        /// </summary>
        /// <param name="lastRound">unused.</param>
        /// <returns>false</returns>
        public override bool DemandConfession(PersonalResult lastRound)
        {
            return false;
        }
    }

    /// <summary>
    /// Prisoner that always strikes a deal.
    /// </summary>
    public class Lucifer : Prisoner
    {
        public Lucifer() : base("Lucifer") { }

        /// <summary>
        /// Demand the prisoner to make a deal and confess.
        /// </summary>
        /// <param name="lastRound">unused</param>
        /// <returns>true</returns>
        public override bool DemandConfession(PersonalResult lastRound)
        {
            return true;
        }
    }

    /// <summary>
    /// Prisoner that never forgets if you ever sell him out.
    /// </summary>
    public class Armageddon : Prisoner
    {
        public Armageddon() : base("Armageddon") { }

        /// <summary>
        /// Did the other player defect at any point this round? 
        /// </summary>
        bool hasEverDefected = false;

        /// <summary>
        /// Demand the prisoner to make a deal and confess.
        /// </summary>
        /// <param name="lastRound">result of last round. Resets if last round if "unknown" to prevent carryover.</param>
        /// <returns>true if the other guy ever defected.</returns>
        public override bool DemandConfession(PersonalResult lastRound)
        {
            if (lastRound == PersonalResult.Unknown)
            {
                hasEverDefected = false;
                return false;
            }

            hasEverDefected |= lastRound.OpponentDefected();
            return hasEverDefected;
        }
    }

    /// <summary>
    /// Insane prisoner acts arbitrarily
    /// </summary>
    public class Insane : Prisoner
    {
        private static Random r = new Random();
        public Insane() : base("Insane") { }

        /// <summary>
        /// Demand the prisoner to make a deal and confess.
        /// </summary>
        /// <param name="lastRound">unused</param>
        /// <returns>random coin flip</returns>
        public override bool DemandConfession(PersonalResult lastRound)
        {
            var value = r.Next();
            return value % 2 == 0;
        }
    }

    /// <summary>
    /// Prisoner that echoes your last decision.
    /// </summary>
    public class TitForTat : Prisoner
    {   
        public TitForTat() : base("TitForTat") { }

        /// <summary>
        /// Demand the prisoner to make a deal and confess.
        /// </summary>
        /// <param name="lastRound">The last round result</param>
        /// <returns>The last round result, false initially.</returns>
        public override bool DemandConfession(PersonalResult lastRound)
        {
            if (lastRound == PersonalResult.Unknown) return false;
            return lastRound.OpponentDefected();
        }
    }

    /// <summary>
    /// Prisoner that echoes your last decision, but sometimes forgives.
    /// </summary>
    public class TitForTatForgive : Prisoner
    {
        public TitForTatForgive() : base("TitForTatForgive") { }

        /// <summary>
        /// Demand the prisoner to make a deal and confess.
        /// </summary>
        /// <param name="lastRound">The last round result</param>
        /// <returns>The last round result (except for occasional random false override), false initially.</returns>
        public override bool DemandConfession(PersonalResult lastRound)
        {
            if (lastRound == PersonalResult.Unknown) return false;

            if (lastRound.OpponentDefected())
            {
                bool forgive = (new Random()).Next() % 10 == 0;

                // If s/he ratted and we don't forgive, then rat.
                return !forgive;
            }

            return false;
        }
    }
}
