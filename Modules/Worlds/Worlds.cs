using System.Collections.Generic;


namespace Nero
{
    public class Worlds {
        public static List<Nero.World> GetWorlds() {
            var worlds = new List<Nero.World>();
            
            // -------------
            // | Primal 11 |
            // -------------
            worlds.Add(new Nero.World("Behemoth", "Primal", "NA"));
            worlds.Add(new Nero.World("Brynhildr", "Primal", "NA"));
            worlds.Add(new Nero.World("Diabolos", "Primal", "NA"));
            worlds.Add(new Nero.World("Excalibur", "Primal", "NA"));
            worlds.Add(new Nero.World("Exodus", "Primal", "NA"));
            worlds.Add(new Nero.World("Famfrit", "Primal", "NA"));
            worlds.Add(new Nero.World("Hyperion", "Primal", "NA"));
            worlds.Add(new Nero.World("Lamia", "Primal", "NA"));
            worlds.Add(new Nero.World("Leviathan", "Primal", "NA"));
            worlds.Add(new Nero.World("Malboro", "Primal", "NA"));
            worlds.Add(new Nero.World("Ultros", "Primal", "NA"));

            // ----------------
            // | Elemental 10 | 21
            // ----------------
            worlds.Add(new Nero.World("Aegis", "Elemental", "JP"));
            worlds.Add(new Nero.World("Atomos", "Elemental", "JP"));
            worlds.Add(new Nero.World("Carbuncle", "Elemental", "JP"));
            worlds.Add(new Nero.World("Garuda", "Elemental", "JP"));
            worlds.Add(new Nero.World("Gungnir", "Elemental", "JP"));
            worlds.Add(new Nero.World("Kujata", "Elemental", "JP"));
            worlds.Add(new Nero.World("Ramuh", "Elemental", "JP"));
            worlds.Add(new Nero.World("Tonberry", "Elemental", "JP"));
            worlds.Add(new Nero.World("Typhon", "Elemental", "JP"));
            worlds.Add(new Nero.World("Unicorn", "Elemental", "JP"));

            // ------------
            // | Chaos 10 | 31
            // ------------
            worlds.Add(new Nero.World("Cerberus", "Chaos", "EU"));
            worlds.Add(new Nero.World("Lich", "Chaos", "EU"));
            worlds.Add(new Nero.World("Louisoix", "Chaos", "EU"));
            worlds.Add(new Nero.World("Moogle", "Chaos", "EU"));
            worlds.Add(new Nero.World("Odin", "Chaos", "EU"));
            worlds.Add(new Nero.World("Omega", "Chaos", "EU"));
            worlds.Add(new Nero.World("Phoenix", "Chaos", "EU"));
            worlds.Add(new Nero.World("Ragnarok", "Chaos", "EU"));
            worlds.Add(new Nero.World("Shiva", "Chaos", "EU"));
            worlds.Add(new Nero.World("Zodiark", "Chaos", "EU"));

            // -----------
            // | Gaia 11 | 42
            // -----------
            worlds.Add(new Nero.World("Alexander", "Gaia", "JP"));
            worlds.Add(new Nero.World("Bahamut", "Gaia", "JP"));
            worlds.Add(new Nero.World("Durandal", "Gaia", "JP"));
            worlds.Add(new Nero.World("Fenrir", "Gaia", "JP"));
            worlds.Add(new Nero.World("Ifrit", "Gaia", "JP"));
            worlds.Add(new Nero.World("Ridill", "Gaia", "JP"));
            worlds.Add(new Nero.World("Tiamat", "Gaia", "JP"));
            worlds.Add(new Nero.World("Ultima", "Gaia", "JP"));
            worlds.Add(new Nero.World("Valefor", "Gaia", "JP"));
            worlds.Add(new Nero.World("Yojimbo", "Gaia", "JP"));
            worlds.Add(new Nero.World("Zeromus", "Gaia", "JP"));

            // -----------
            // | Mana 11 | 53
            // -----------
            worlds.Add(new Nero.World("Anima", "Mana", "JP"));
            worlds.Add(new Nero.World("Asura", "Mana", "JP"));
            worlds.Add(new Nero.World("Belias", "Mana", "JP"));
            worlds.Add(new Nero.World("Chocobo", "Mana", "JP"));
            worlds.Add(new Nero.World("Hades", "Mana", "JP"));
            worlds.Add(new Nero.World("Ixion", "Mana", "JP"));
            worlds.Add(new Nero.World("Mandragora", "Mana", "JP"));
            worlds.Add(new Nero.World("Masamune", "Mana", "JP"));
            worlds.Add(new Nero.World("Pandaemonium", "Mana", "JP"));
            worlds.Add(new Nero.World("Shinryu", "Mana", "JP"));
            worlds.Add(new Nero.World("Titan", "Mana", "JP"));

            // -------------
            // | Aether 13 | 66
            // -------------
            worlds.Add(new Nero.World("Adamantoise", "Aether", "NA"));
            worlds.Add(new Nero.World("Balmung", "Aether", "NA"));
            worlds.Add(new Nero.World("Cactuar", "Aether", "NA"));
            worlds.Add(new Nero.World("Coeurl", "Aether", "NA"));
            worlds.Add(new Nero.World("Faerie", "Aether", "NA"));
            worlds.Add(new Nero.World("Gilgamesh", "Aether", "NA"));
            worlds.Add(new Nero.World("Goblin", "Aether", "NA"));
            worlds.Add(new Nero.World("Jenova", "Aether", "NA"));
            worlds.Add(new Nero.World("Mateus", "Aether", "NA"));
            worlds.Add(new Nero.World("Midgardsormr", "Aether", "NA"));
            worlds.Add(new Nero.World("Sargatanas", "Aether", "NA"));
            worlds.Add(new Nero.World("Siren", "Aether", "NA"));
            worlds.Add(new Nero.World("Zalera", "Aether", "NA"));

            return worlds;
        }
    }
}