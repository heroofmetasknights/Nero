
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Nero {
    public class Player {
        public ulong discordID;
        public string fileName;
        public string playerName;
        public string dc;
        public string world;
        public List<Fight> fights;
        public List<job> jobs;
        public double bestPercent;
        public double bestDps;
        public int fightsCleared;
        public string xivdbURL;
        public string xivdbURL_API;

        public Player(ulong _discordID, string _playerName, string _dc, string _world) {
            discordID = _discordID;
            playerName = _playerName;
            fileName = $"characters/{discordID}.json";
            fights = new List<Fight>();
            jobs = new List<job>();
            dc = _dc;
            world = _world;
            bestPercent = 0.0;
            bestDps = 0.0;
            fightsCleared = 0;
            xivdbURL = "";
            xivdbURL_API = "";
        }

        public void AddFight(bool _cleared, int _kills, int _jobAmount, List<job> _jobs, string _fightName, double _bestDps, double _bestPercent) {
            fights.Add(new Fight(_cleared, _kills, _jobAmount, _jobs, _fightName, _bestDps, _bestPercent));
        }

        public void CalculateBest() {
            this.bestPercent = 0.0;

            foreach (var fight in fights) {
                this.bestPercent += fight.bestPercent;
                if (this.bestDps <= fight.bestDps)
                    this.bestDps = fight.bestDps;
            }

            //foreach(var job in jobs) {
            //    this.bestPercent += job.historical_percent;
            //    if (this.bestDps <= job.historical_dps) 
            //        this.bestDps = job.historical_dps;    
            //}

            if (fightsCleared > 0) {
                this.bestPercent /= fightsCleared;
            } else {
                this.bestPercent = 0.0;
            }
        }

        public List<string> GetClearedFights(){
            var clearedFightsList = new List<string>();
            fightsCleared = 0;
            var jobnames = new List<string>();

            foreach (Fight fight in fights) {
                if (fight.cleared == true && clearedFightsList.Contains(fight.fightName) == false) {
                    clearedFightsList.Add(fight.fightName);
                    fightsCleared++;
                }

                foreach (var job in fight.jobs) {
                    if (jobnames.Contains(job.name) == false) {
                        jobnames.Add(job.name);
                        jobs.Add(job);
                    } else {
                        foreach (var jb in jobs) {
                            if (jb.name == job.name && jb.historical_percent <= job.historical_percent) {
                                jb.historical_percent = job.historical_percent;
                            }
                            if (jb.name == job.name && jb.historical_dps <= job.historical_dps) {
                                jb.historical_dps = job.historical_dps;
                            }
                        }
                    }
                    
                }
            }
            this.CalculateBest();
            this.EnsureExists();
            return clearedFightsList;
        }

        public void EnsureExists() {
            string file = Path.Combine(AppContext.BaseDirectory, fileName);
            if (!File.Exists(file)) {
                string path = Path.GetDirectoryName(file);
                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                this.SaveJson();
            } else {
                this.SaveJson();
            }
        }

        public void SaveJson() {
            string file = Path.Combine(AppContext.BaseDirectory, fileName);
            File.WriteAllText(file, ToJson());
        }

        public static Player Load(ulong id) {
            string file = Path.Combine(AppContext.BaseDirectory, $"characters/{id}.json");
            return JsonConvert.DeserializeObject<Player>(File.ReadAllText(file));
        }

        public static bool DoesProfileExist(ulong id) {
            string file = Path.Combine(AppContext.BaseDirectory, $"characters/{id}.json");

            if (!File.Exists(file)) {
                return false;
            } else {
                return true;
            }
        }

        public string ToJson() 
            => JsonConvert.SerializeObject(this, Formatting.Indented);

    }
}