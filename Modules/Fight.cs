using System.Collections.Generic;
using Discord.Commands;


namespace Nero
{
    public struct Fight {
        public bool cleared;
        public int kills;
        public int jobAmount;
        public List<job> jobs;
        public string fightName;
        public double bestDps;
        public double bestPercent;

        public Fight (bool _cleared, int _kills, int _jobAmount, List<job> _jobs, string _fightName, double _bestDps, double _bestPercent) {
            cleared = _cleared;
            kills = _kills;
            jobAmount = _jobAmount;
            jobs = _jobs;
            fightName = _fightName;
            bestDps = _bestDps;
            bestPercent = _bestPercent;
        }
    }
}
