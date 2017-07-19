using Newtonsoft.Json;

namespace XivDB {
    public class XIVDBCharacter {
        public int lodestone_id { get; set; }
        public string name { get; set; }
        public string server { get; set; }
        public string avatar { get; set; }
        public string added { get; set; }
        public string last_updated { get; set; }
        public string last_synced { get; set; }
        public string data_last_changed { get; set; }
        public string data_hash { get; set; }
        public int update_count { get; set; }
        public string achievements_last_updated { get; set; }
        public string achievements_last_changed { get; set; }
        public int achievements_public { get; set; }
        public int achievements_score_reborn { get; set; }
        public int achievements_score_legacy { get; set; }
        public int achievements_score_reborn_total { get; set; }
        public int achievements_score_legacy_total { get; set; }
        public string deleted { get; set; }
        public int priority { get; set; }
        public int patch { get; set; }
        [JsonProperty("data")]
        public data data { get; set; }
        public Grand_Companies grand_companies { get; set; }
        public string portrait { get; set; }
        public string last_active { get; set; }
        public string url { get; set; }
        public string url_api { get; set; }
        public string url_xivdb { get; set; }
        public string url_lodestone { get; set; }
        public string url_type { get; set; }
        public float achievements_score_reborn_percent { get; set; }
        public int achievements_score_legacy_percent { get; set; }
        public Extras extras { get; set; }
    }
}


