using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrisonersDilemmaArena
{
    /// <summary>
    /// Participant in the arena.
    /// </summary>
    public class Participant
    {
        /// <summary>
        /// Counters for personal results.
        /// </summary>
        private int zeroYears, oneYears, fiveYears, tenYears = 0;

        /// <summary>
        /// Counter for rounds played.
        /// </summary>
        private int roundsPlayed = 0;

        /// <summary>
        /// Counter for number of rounds played without input.
        /// </summary>
        private int blindRoundsPlayed = 0;

        /// <summary>
        /// The Prisoner participating.
        /// </summary>
        private IPrisoner prisoner;

        /// <summary>
        /// The last result against each player. 
        /// Setting up for parallelization.
        /// </summary>
        private Dictionary<Guid, PersonalResult> lastResults;

        public Guid Name
        {
            get;
            private set;
        }

        public string Strategy
        {
            get
            {
                return this.prisoner.GetStrategy();
            }
        }

        public Participant(IPrisoner prisoner)
        {
            this.prisoner = prisoner;
            this.Name = this.prisoner.GetName();
            this.lastResults = new Dictionary<Guid, PersonalResult>();
        }

        public int GetRoundsPlayed()
        {
            return this.roundsPlayed;
        }

        /// <summary>
        /// Play a match. "Arrest" the participant and a known cohort and demand a confession.
        /// </summary>
        /// <param name="opponent">The opponent for this round.</param>
        /// <param name="reset">Is this the first match this round?</param>
        /// <returns>Does this player confess?</returns>
        public bool PlayMatch(Guid opponent, bool reset = false)
        {
            PersonalResult lastResult = PersonalResult.Unknown;
            if (!reset && this.lastResults.ContainsKey(opponent))
            {
                lastResult = this.lastResults[opponent];
            }

            if (lastResult == PersonalResult.Unknown)
                Interlocked.Increment(ref this.blindRoundsPlayed);

            Interlocked.Increment(ref this.roundsPlayed);
            return this.prisoner.DemandConfession(lastResult);
        }

        /// <summary>
        /// Get the score for this player in terms of years of prison.
        /// </summary>
        /// <returns>Total jail time.</returns>
        public int GetScore()
        {
            return oneYears + 5 * fiveYears + 10 * tenYears;
        }

        /// <summary>
        /// Tell the participant the result of this round. Uh, I mean, sentence them!
        /// </summary>
        /// <param name="result">The result of the most recent confession.</param>
        public void RecordResult(DilemmaRoundResult result)
        {
            if (result.IsValid(this.Name))
            {
                var lastResult = result.GetPlayerLastResult(this.Name);
               Debug.WriteLine("{0} ({1}) reports {2}", this.Name, this.Strategy, lastResult);
                this.lastResults[result.GetOpponent(this.Name)] = lastResult;
                switch (lastResult)
                {
                    case PersonalResult.SellOut:
                        Interlocked.Increment(ref this.zeroYears);
                        break;
                    case PersonalResult.BothSilent:
                        Interlocked.Increment(ref this.oneYears);
                        break;
                    case PersonalResult.Sloppy:
                        Interlocked.Increment(ref this.fiveYears);
                        break;
                    case PersonalResult.SoldOut:
                        Interlocked.Increment(ref this.tenYears);
                        break;
                    case PersonalResult.Unknown:
                        break;
                }
            }
        }

        /// <summary>
        /// Get diagnostic info
        /// </summary>
        /// <returns>Detailed score info.</returns>
        internal string GetScoreVerbose()
        {
            return string.Format(
                "{0} :: 0:{1}; 1:{2}; 5:{3}; 10:{4}; Confessed {5}/{6} Blind {7}",
                this.GetScore(),
                this.zeroYears,
                this.oneYears,
                this.fiveYears,
                this.tenYears,
                this.zeroYears + this.fiveYears,
                this.roundsPlayed,
                this.blindRoundsPlayed);
        }
    }
}
