namespace Nero
{
    public class job {
        public string name;
        public string short_name;
        public string savage_name;
        public string fflogs_name;
        public string role;
        public string subrole;
        public double historical_percent;
        public double historical_dps;
        public double dps;
        public double savageP;

        public job (string _fflogs_name, double _percent, double _dps) {
            fflogs_name = _fflogs_name;
            historical_percent = _percent;
            historical_dps = _dps;
            dps = _percent;
            savageP = 0.0;

            if (fflogs_name == "BlackMage") {
                name = "BlackMage";
                short_name = "blm";
                role = "DPS";
                subrole = "Caster";
                savage_name = "Savage%-BlackMage";
            }
            if (fflogs_name == "RedMage") {
                name = "RedMage";
                short_name = "rdm";
                role = "DPS";
                subrole = "Caster";
                savage_name = "Savage%-RedMage";
            }
            if (fflogs_name == "WhiteMage") {
                name = "WhiteMage";
                short_name = "whm";
                role = "Healer";
                subrole = "Caster";
                savage_name = "Savage%-WhiteMage";
            }
            if (fflogs_name == "Bard") {
                name = "Bard";
                short_name = "brd";
                role = "DPS";
                subrole = "Ranged";
                savage_name = "Savage%-Bard";
            }
            if (fflogs_name == "Warrior") {
                name = "Warrior";
                short_name = "war";
                role = "Tank";
                subrole = "Melee";
                savage_name = "Savage%-Warrior";
            }
            if (fflogs_name == "Scholar") {
                name = "Scholar";
                short_name = "sch";
                role = "Healer";
                subrole = "Caster";
                savage_name = "Savage%-Scholar";
            }
            if (fflogs_name == "Summoner") {
                name = "Summoner";
                short_name = "smn";
                role = "DPS";
                subrole = "Caster";
                savage_name = "Savage%-Summoner";
            }
            if (fflogs_name == "Astrologian") {
                name = "Astrologian";
                short_name = "ast";
                role = "Healer";
                subrole = "Caster";
                savage_name = "Savage%-Astrologian";
            }
            if (fflogs_name == "Paladin") {
                name = "Paladin";
                short_name = "pld";
                role = "Tank";
                subrole = "Melee";
                savage_name = "Savage%-Paladin";
            }
            if (fflogs_name == "Monk") {
                name = "Monk";
                short_name = "mnk";
                role = "DPS";
                subrole = "Melee";
                savage_name = "Savage%-Monk";
            }
            if (fflogs_name == "Ninja") {
                name = "Ninja";
                short_name = "nin";
                role = "DPS";
                subrole = "Melee";
                savage_name = "Savage%-Ninja";
            }
            if (fflogs_name == "Dragoon") {
                name = "Dragoon";
                short_name = "drg";
                role = "DPS";
                subrole = "Melee";
                savage_name = "Savage%-Dragoon";
            }
            if (fflogs_name == "DarkKnight") {
                name = "DarkKnight";
                short_name = "drk";
                role = "Tank";
                subrole = "Melee";
                savage_name = "Savage%-DarkKnight";
            }
            if (fflogs_name == "Machinist") {
                name = "Machinist";
                short_name = "mch";
                role = "DPS";
                subrole = "Ranged";
                savage_name = "Savage%-Machinist";
            }
            if (fflogs_name == "Samurai") {
                name = "Samurai";
                short_name = "sam";
                role = "DPS";
                subrole = "Melee";
                savage_name = "Savage%-Samurai";
            }
        }
    }
}