
using System;
using System.Collections.Generic;


namespace Nero {
    public class Player {
            public string playerName;
            public string dc;
            public string world;
            public List<Fight> fights;
            public List<job> jobs;
            public double bestPercent;
            public double bestDps;
            public int fightsCleared;

            public Player(string _playerName, string _dc, string _world) {
                fights = new List<Fight>();
                jobs = new List<job>();
                playerName = _playerName;
                dc = _dc;
                world = _world;
                bestPercent = 0.0;
                bestDps = 0.0;
                fightsCleared = 0;
            }

            public void AddFight(bool _cleared, int _kills, int _jobAmount, List<job> _jobs, string _fightName, double _bestDps, double _bestPercent) {
                fights.Add(new Fight(_cleared, _kills, _jobAmount, _jobs, _fightName, _bestDps, _bestPercent));
            }

            public void CalculateBest() {
                foreach (Fight fight in fights) {
                    if (fight.bestPercent >= this.bestPercent)
                        this.bestPercent += fight.bestPercent;

                        if (fight.bestDps >= this.bestDps)
                        this.bestDps = fight.bestDps;
                }

                if (fightsCleared > 0) {
                    this.bestPercent /= fightsCleared;
                } else {
                    this.bestPercent = 0.0;
                }
            }

            public List<string> GetClearedFights(){
                var clearedFightsList = new List<string>();
                Console.WriteLine("fight count: " + fights.Count);
                foreach (Fight fight in fights) {
                    if (fight.cleared == true) {
                        clearedFightsList.Add(fight.fightName);
                        fightsCleared++;
                    }

                    foreach (var job in fight.jobs) {
                        if(!this.jobs.Contains(job)) {
                            this.jobs.Add(job);
                        }
                    }
                }
                this.CalculateBest();
                return clearedFightsList;
            }

        }
}