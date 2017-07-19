using System.Collections.Generic;
using Discord.Commands;

namespace Nero
{

    public class Characters {
        public List<Nero.CharacterResult> results { get; set; }
        public int total { get; set; }
        public Nero.Paging paging { get; set; }
    }
}