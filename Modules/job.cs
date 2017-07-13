namespace Nero
{
    public class job {
        public string name;
        public string savage_name;
        public string fflogs_name;
        public string role;
        public string subrole;
        public double historical_percent;

        public job (string _fflogs_name, double _percent) {
            fflogs_name = _fflogs_name;
            historical_percent = _percent;

            if (fflogs_name == "BlackMage") {
                name = "Black-Mage";
                role = "DPS";
                subrole = "Caster";
                savage_name = "Savage%-BlackMage";
            }
            if (fflogs_name == "RedMage") {
                name = "Red-Mage";
                role = "DPS";
                subrole = "Caster";
                savage_name = "Savage%-RedMage";
            }
            if (fflogs_name == "WhiteMage") {
                name = "White-Mage";
                role = "Healer";
                subrole = "Caster";
                savage_name = "Savage%-WhiteMage";
            }
            if (fflogs_name == "Bard") {
                name = "Bard";
                role = "DPS";
                subrole = "Ranged";
                savage_name = "Savage%-Bard";
            }
            if (fflogs_name == "Warrior") {
                name = "Warrior";
                role = "Tank";
                subrole = "Melee";
                savage_name = "Savage%-Warrior";
            }
            if (fflogs_name == "Scholar") {
                name = "Scholar";
                role = "Healer";
                subrole = "Caster";
                savage_name = "Savage%-Scholar";
            }
            if (fflogs_name == "Summoner") {
                name = "Summoner";
                role = "DPS";
                subrole = "Caster";
                savage_name = "Savage%-Summoner";
            }
            if (fflogs_name == "Astrologian") {
                name = "Astrologian";
                role = "Healer";
                subrole = "Caster";
                savage_name = "Savage%-Astrologian";
            }
            if (fflogs_name == "Paladin") {
                name = "Paladin";
                role = "Tank";
                subrole = "Melee";
                savage_name = "Savage%-Paladin";
            }
            if (fflogs_name == "Monk") {
                name = "Monk";
                role = "DPS";
                subrole = "Melee";
                savage_name = "Savage%-Monk";
            }
            if (fflogs_name == "Dragoon") {
                name = "Dragoon";
                role = "DPS";
                subrole = "Melee";
                savage_name = "Savage%-Dragoon";
            }
            if (fflogs_name == "DarkKnight") {
                name = "Dark-Knight";
                role = "Tank";
                subrole = "Melee";
                savage_name = "Savage%-DarkKnight";
            }
            if (fflogs_name == "Machinist") {
                name = "Machinist";
                role = "DPS";
                subrole = "Ranged";
                savage_name = "Savage%-Machinist";
            }
            if (fflogs_name == "Samurai") {
                name = "Samurai";
                role = "DPS";
                subrole = "Melee";
                savage_name = "Savage%-Samurai";
            }
        }
    }
}