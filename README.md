# Nero
[Invite Link](https://discordapp.com/oauth2/authorize?permissions=2080898303&scope=bot&client_id=332176591042117634)

Feature List (click for screenshots)

 * [FFLogs Profiles accessible from any discord server!](https://puu.sh/xvOCi.png)
 * [Role System (optional)](https://puu.sh/xvP3c.png)
 * [Static Creation, Management & Recruitment](https://puu.sh/xvP9b.png)


______

Hi, Nero is a project I've been working on the last few months and its finally at the point where I'm alright with releasing it to the general public. Nero has what I think are some very powerful features such as a role system integrated with XIVDB and FFLogs, a profile system that available anywhere nero is available and a static manager/recruitment/LFG tool with more to come in the future!
______

# Role System

Nero's role system works with fflogs data to automatically create and 
assign relevant roles to each user, for example a bard on gilgamesh who has cleared O1S - O4S with a 95% or higher rating will be assigned the roles `Aether`, `Gilgamesh`, `Bard`, `Savage%-Bard` `Cleared-O1S`, `Cleared-O2S`, `Cleared-O3S`, `Cleared-O4S`, `Aether-Bigdps-Club` & last but not least `DPS`. 

A Paladin with O1S-O4S cleared who sometimes plays WHM from excalibur with only O1S-O3S cleared would look like this: `Primal`, `Excalibur`, `PLD`, `WHM` `Cleared-O1S`, `Cleared-O2S`, `Cleared-O3S`, `Cleared-O4S`, `Primal-Bigdps-Club`



 Role Explanations
-----
`Datacenter`: Your datacenter(s) Ex: `Primal`, `Aether`, `Chaos`.

`World`: Your World(s) Ex: `Excalibur`, `Gilgamesh`, `Tonberry`.

`Job`: Your Job(s) Ex: `BARD`, `SAM`, `WHM`. (**Note**: Will only be added if there is fflogs data for you)

`Cleared-fight`: The fight(s) you have cleared according to fflogs. EX: `cleared-lakshmi-ex`, `cleared-O4S`

`Datacenter-Bigdps-Club`: If you have at least a 90th percentile score on any savage fight nero will assign you the Bigdps role for your datacenter. EX: `Primal-Bigdps-Club`

`Savage%-Job`: If you have atleast a 95th percentile average across all savage fights for a single job nero will assign you this role. Ex: `Savage%-NIN`, `Savage%-WAR`, `Savage%-RDM`


__Requirements__
  * [Discord.net v1.0.1](https://github.com/RogueException/Discord.Net)
  * .Net Core
