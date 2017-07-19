using Discord.Commands;

namespace Nero
{
    public class CharacterResult {
        public string name { get; set; }
        public string server { get; set; }
        public string icon { get; set; }
        public string last_updated { get; set; }
        public int id { get; set; }
        public string url { get; set; }
        public string url_type { get; set; }
        public string url_api { get; set; }
        public string url_xivdb { get; set; }
    }
}