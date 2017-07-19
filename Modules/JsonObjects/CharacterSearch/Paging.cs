using System.Collections.Generic;
using Discord.Commands;

namespace Nero
{
    public class Paging {
        public int page { get; set; }
        public int total { get; set; }
        public List<int> pages { get; set; }
        public int next { get; set; }
        public int prev { get; set; }
    }

}