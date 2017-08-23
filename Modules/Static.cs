
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Discord;
using Discord.Commands;

namespace Nero {
    public class PlayerStatic {
        public ulong discordID;
        public string fileName;
        public string PlayerStaticName;
        public string dc;
        public List<string> cleared;


        public double[] fight_dps;

        public PlayerStatic(ulong _discordID, string _PlayerStaticName, string _dc, string _world) {
            discordID = _discordID;
            PlayerStaticName = _PlayerStaticName;
            fileName = $"characters/{discordID}.json";
            cleared = new List<string>();
            dc = _dc;
            
            
            
            this.EnsureExists();
        }

        //public void AddFight(bool _cleared, int _kills, int _jobAmount, List<job> _jobs, string _fightName, double _bestDps, double _bestPercent) {
        //    fights.Add(new Fight(_cleared, _kills, _jobAmount, _jobs, _fightName, _bestDps, _bestPercent));
        //}

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

        public static PlayerStatic Load(ulong id) {
            string file = Path.Combine(AppContext.BaseDirectory, $"characters/{id}.json");
            return JsonConvert.DeserializeObject<PlayerStatic>(File.ReadAllText(file));
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