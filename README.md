# Nero
[Invite Link](https://discordapp.com/oauth2/authorize?permissions=2080898303&scope=bot&client_id=332176591042117634)

Feature List (click for screenshots)

 * [FFLogs Profiles accessible from any discord server!](https://puu.sh/xvOCi.png)
 * [Role System (Requires role creation/assignment privledges)](https://puu.sh/xvP3c.png)
 * [Static Creation, Management & Recruitment](https://puu.sh/xvP9b.png)

______

Hi, Nero is a project I've been working on the last few months and its finally at the point where I'm alright with releasing it to the general public. Nero has what I think are some very powerful features such as a role system integrated with XIVDB and FFLogs, a profile system that available anywhere nero is available and a static manager/recruitment/LFG tool with more to come in the future!
______

# Important Commands

* `!n help`: Displays all commands available from nero along with explanations
* `!n assign` `server` `character name`: Nero will fetch your fflogs data and create your profile!
* `!n view`: Show your profile to the world!
* `!n view` `@mention`: Look at another discord users profile
* `!n view` `server`, `character name`: Look at the fflogs data of someone who hasn't yet created a profile
* `!n static List`: Lists all recruiting statics
* `!n static List` `job acronym`: Lists all statics recruiting only the specified job
* `!n static search` `static name`: searches for statics that match the given name
* `!n invite bot`: Creates an invite link for the bot, useful when making your static.

Server Admins / Static Leaders Only
* `!n setup`: Setup prompt, its the same one you go through when nero first joins the server.
* `!n static create`: Launches a prompt to walk you through the static creation process(**Note**: make sure to run `!n setup` first)
* `!n settings`: Lets you view and change various server settings/info
* `!n static filters`: View/Edit static recruitment filters
* `!n static Applications` : View/Approve/Reject membership applications
* `!n static recruitment`: View/Enable/Disable static recruitment

______

# Role System

Nero's role system works with fflogs data to automatically create and 
assign relevant roles to each user, for example a bard on gilgamesh who has cleared O5S - O8S with a 95% or higher rating will be assigned the roles `Aether`, `Gilgamesh`, `Bard`, `Savage%-Bard` `Cleared-O5S`, `Cleared-O6S`, `Cleared-O7S`, `Cleared-O8S`, `Aether-Bigdps-Club` & last but not least `DPS`. 

A Paladin with O5S-O8S cleared who sometimes plays WHM from excalibur with only O5S-O7S cleared would look like this: `Primal`, `Excalibur`, `PLD`, `WHM` `Cleared-O5S`, `Cleared-O6S`, `Cleared-O7S`, `Cleared-O8S`, `Primal-Bigdps-Club`



 Role Explanations
-----
`Datacenter`: Your datacenter(s) Ex: `Primal`, `Aether`, `Chaos`.

`World`: Your World(s) Ex: `Excalibur`, `Gilgamesh`, `Tonberry`.

`Job`: Your Job(s) Ex: `BARD`, `SAM`, `WHM`. (**Note**: Will only be added if there is fflogs data for you)

`Cleared-fight`: The fight(s) you have cleared according to fflogs. EX: `cleared-lakshmi-ex`, `cleared-O8S`

`Datacenter-Bigdps-Club`: If you have at least a 90th percentile score on any savage fight nero will assign you the Bigdps role for your datacenter. EX: `Primal-Bigdps-Club`

`Savage%-Job`: If you have atleast a 95th percentile average across all savage fights for a single job nero will assign you this role. Ex: `Savage%-NIN`, `Savage%-WAR`, `Savage%-RDM`

______

# Profiles

Nero's Profile system allows you to link your fflogs data easily in any server nero is in, as well as apply to statics in the Nero system.

It also allows you to show off details about your character such as your highest parse and percentage per job per fight and shows every job you've beaten a fight with.

https://puu.sh/xvQ9G.png

______

# Statics

Nero offers some pretty powerful tools to statics such as automated filtering of applicants depending on specified filters such as fights cleared & jobs, membership application processing, and recruitment. Nero will automatically DM(Direct Message) the static creator with Profiles of applicants and allows any static member to view data about static members(clears, etc)

_____

# LFG

Nero tries to streamline finding a new static thanks to the ability to search for currently recruiting statics with job filters, once one is selected Nero will then send the application to the static leader and if you're accepted, Nero will give you an invite link to the statics discord.


____

# Required Privileges & Explanations

Nero requires the following privledges to work properly

`Read Messages` - Required to respond to commands

`Send Messages` - Pretty obvious why its required

`Embed Links` - Needed for profile viewing

`Read Message History`: Required to pretty print some stuff

`Add Reactions`: Required for the `!n static list`/`!n static search` commands as it allows paginated searches.

`Manage Roles` - Required if you want to use the bots Role System, please turn off the use roles feature in nero's settings as well.

____

# Contact

Please feel free contact me either on reddit or add me on discord (River City Ransomware#0830)

__Requirements__
  * [Discord.net v1.0.1](https://github.com/RogueException/Discord.Net)
  * .Net Core
