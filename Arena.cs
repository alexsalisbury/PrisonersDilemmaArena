using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonersDilemmaArena
{
    public class Arena
    {
        List<Participant> participants = new List<Participant>();

        public Arena()
        {
            participants = new List<Participant>();
            participants.Add(new Participant(new Jesus()));
            participants.Add(new Participant(new Lucifer()));
            participants.Add(new Participant(new Insane()));
            participants.Add(new Participant(new Armageddon()));
            participants.Add(new Participant(new TitForTat()));
            participants.Add(new Participant(new TitForTatForgive()));
        }

        /// <summary>
        /// Play two sets of rounds between each player.(sorta like "home and away")
        /// </summary>
        /// <param name="matchCountPerRound">Matches per round. Move to config?</param>
        /// <remarks>Total matches should equal 2 * (playerCount -1) * matchCountPerRound</remarks>
        public void RoundRobin(int matchCountPerRound)
        {
            foreach (var player in participants)
            {
                foreach (var opponent in participants)
                {
                    if (player != opponent)
                    {
                        PlayRound(player, opponent, matchCountPerRound);
                    }
                }
            }
        }

        public void PlayRound(Participant left, Participant right, int matchCount)
        {
            // For each round, reset on the first match
            PlayMatch(left, right, true);
            matchCount -= 1;

            while (matchCount-- > 0)
            {
                PlayMatch(left, right, false);
            }
        }

        private void PlayMatch(Participant left, Participant right, bool reset)
        {
            var leftConfessed = left.PlayMatch(right.Name, reset);
            var rightConfessed = right.PlayMatch(left.Name, reset);
            DilemmaRoundResult result = new DilemmaRoundResult(left.Name, right.Name, leftConfessed, rightConfessed);
            left.RecordResult(result);
            right.RecordResult(result);
        }

        internal void PrintVerboseResults()
        {
            // TODO: Implement IComparable on part's and sort.
            this.participants.ForEach(p => Console.WriteLine("{0}({1}) : {2}", p.Strategy, p.Name, p.GetScoreVerbose()));
        }

        internal void PrintResults()
        {
            // TODO: Implement IComparable on participants, sort, and fancy print (scoreboard style.)
            this.participants.ForEach( p => Console.WriteLine("{0}({1}) : {2}", p.Strategy, p.Name, p.GetScore()));
        }
    }
}
