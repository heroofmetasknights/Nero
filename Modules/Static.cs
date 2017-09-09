using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Discord;
using Discord.Commands;

namespace Nero {
    public class PlayerStatic {
        public ulong discordServerID;
        public ulong creatorID;
        public string fileName;
        public string PlayerStaticName;
        public string dc;
        public Dictionary<string, ulong> Members = new Dictionary<string, ulong>();
        public List<string> cleared;

        public string InviteLink;

        public Dictionary<string, ulong> Applications = new Dictionary<string, ulong>();

        public double[] fight_dps;

        public bool recruiting;

        public List<string> Filters = new List<string>();

        public PlayerStatic(ulong _discordServerID, string _PlayerStaticName, string _dc, ulong _creatorID, string _invite) {
            discordServerID = _discordServerID;
            PlayerStaticName = _PlayerStaticName;
            fileName = $"statics/{discordServerID}.json";
            cleared = new List<string>();
            dc = _dc;
            creatorID = _creatorID;
            InviteLink = _invite;
            recruiting = false;
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
            string file = Path.Combine(AppContext.BaseDirectory, $"statics/{id}.json");
            return JsonConvert.DeserializeObject<PlayerStatic>(File.ReadAllText(file));
        }

        public static bool DoesProfileExist(ulong id) {
            string file = Path.Combine(AppContext.BaseDirectory, $"statics/{id}.json");
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