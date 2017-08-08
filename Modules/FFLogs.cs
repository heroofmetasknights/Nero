//TODO : Redo xivdb character parsing to actually work
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Newtonsoft.Json;
using Nero.Modules.Utilities;

namespace Nero
{

    public partial class fflogs : ModuleBase {
        
        public Dictionary<string, ulong> GetRoles() {
            var roles = new Dictionary<string, ulong>();
            foreach(var role in Context.Guild.Roles) {
                if (role.Name.ToLower() != "new role") {
                    roles.Add(role.Name.ToLower(), role.Id);
                }
            }
            return roles;
        }

        public async Task AddRoleAsync(Dictionary<string, ulong> roles, string name, Task<IGuildUser> user) {
            if (!roles.ContainsKey(name.ToLower())) {
                await ReplyAsync($"@{Context.Guild.GetRole(roles["administrator"])} role: {name} does not exist yet, please create it.");
            }
            else {
                if (user.Result.RoleIds.Contains(roles[name.ToLower()]) == false)
                await user.Result.AddRoleAsync(Context.Guild.GetRole(roles[name.ToLower()]));
            }
        }

        public async Task AddDataCenterWorldRoles(Dictionary<string, ulong> roles, Task<IGuildUser> user, Player _player) {
            var rolesToAdd = new List<IRole>();
            // World Role
            if (!roles.ContainsKey(_player.world.ToLower())) {
                Console.WriteLine($"@{Context.Guild.GetRole(roles["administrator"])} role: {_player.world.ToLower()} does not exist yet, please create it.");
            }
            else
            {
                if (user.Result.RoleIds.Contains(roles[_player.world.ToLower()]) == false)
                    rolesToAdd.Add(Context.Guild.GetRole(roles[_player.world.ToLower()]));
            }

            // DC Role
            if (!roles.ContainsKey(_player.dc.ToLower()))
            {
                Console.WriteLine($"@{Context.Guild.GetRole(roles["administrator"])} role: {_player.dc} does not exist yet, please create it.");
            }
            else
            {
                if (user.Result.RoleIds.Contains(roles[_player.dc.ToLower()]) == false)
                    rolesToAdd.Add(Context.Guild.GetRole(roles[_player.dc.ToLower()]));
            }

            if (rolesToAdd.Count > 0) {
                await user.Result.AddRolesAsync(rolesToAdd);
            }
        }

        
        public async Task AssignRolesAsync(Dictionary<string, ulong> roles, Task<IGuildUser> user, Player _player) {
            var context = Context;
            var clearedFights = _player.GetClearedFights(context);
            var savageJobs = _player.GetSavageJobs();
            var rolesToAdd = new List<IRole>();
            int savageFightCount = 4;
            
            if (clearedFights.Count == 0)
                await ReplyAsync("This player has not cleared any extreme/savage fights");

            // Susano Role
            if (clearedFights.Contains("Susano") && user.Result.RoleIds.Contains(roles["cleared-susano-ex"]) == false)
                rolesToAdd.Add(Context.Guild.GetRole(roles["cleared-susano-ex"]));

            // Lakshmi Role
            if (clearedFights.Contains("Lakshmi") && user.Result.RoleIds.Contains(roles["cleared-lakshmi-ex"]) == false)
                rolesToAdd.Add(Context.Guild.GetRole(roles["cleared-lakshmi-ex"]));

            // Savage
            for (int i = 1; i<=savageFightCount;i++) {
                if(!roles.ContainsKey($"cleared-o{i}s")){
                    Console.WriteLine($"@{Context.Guild.GetRole(roles["administrator"])} role: cleared-o{i}s does not exist yet, please create it.");
                } else {
                    if (clearedFights.Contains($"O{i}S") && user.Result.RoleIds.Contains(roles[$"cleared-o{i}s"]) == false) {
                        Console.WriteLine($"Adding Role: Cleared-O{i}S");
                        rolesToAdd.Add(Context.Guild.GetRole(roles[$"cleared-o{i}s"]));
                    }
                }
            }
            

            //top 10%
            if (_player.bestSavagePercent >= 90.0 && _player.fightsCleared > 0) // magic number lol
            {
                if (roles.ContainsKey($"{_player.dc.ToLower()}-bigdps-club"))
                    rolesToAdd.Add(Context.Guild.GetRole(roles[$"{_player.dc.ToLower()}-bigdps-club"]));
            }

            //Savage-Job
            if (savageJobs.Count > 0 && _player.cleared.Contains("O1S") &&
            _player.cleared.Contains("O2S") && _player.cleared.Contains("O3S") && _player.cleared.Contains("O4S")) {
                foreach (var job in savageJobs) {
                    try {
                        if (roles.ContainsKey($"savage%-{job.ToLower()}"))
                            rolesToAdd.Add(Context.Guild.GetRole(roles[$"savage%-{job.ToLower()}"]));
                    } catch (Exception exception) {
                        System.Console.WriteLine($"Exception caught: {exception}");
                    }
                }
            }

            

            if (_player.jobs.Count == 0)
            {
                await ReplyAsync("No recorded Parses");
                return;
            }
            else
            {
                foreach (var classjob in _player.jobs)
                {

                    // Job role
                    if (!roles.ContainsKey(classjob.name.ToLower()))
                    {
                        Console.WriteLine($"@{Context.Guild.GetRole(roles["administrator"])} role: {classjob.name} does not exist yet, please create it.");
                    }
                    else
                    {
                        if (user.Result.RoleIds.Contains(roles[classjob.name.ToLower()]) == false)
                            rolesToAdd.Add(Context.Guild.GetRole(roles[classjob.name.ToLower()]));
                    }

                    //Role role
                    if (!roles.ContainsKey(classjob.role.ToLower()))
                    {
                        Console.WriteLine($"@{Context.Guild.GetRole(roles["administrator"])} role: {classjob.role} does not exist yet, please create it.");
                    }
                    else
                    {
                        if (user.Result.RoleIds.Contains(roles[classjob.role.ToLower()]) == false)
                            rolesToAdd.Add(Context.Guild.GetRole(roles[classjob.role.ToLower()]));
                    }

                }
            }

            if (rolesToAdd.Count > 0) {
                await user.Result.AddRolesAsync(rolesToAdd);
            }
        }


        [Command("assign")]
        [Alias("a")]
        public async Task assign(string server, [Remainder] string character) {
            var msg = await ReplyAsync("Working...");

            await GetParse(server, character, Context.User.Id);

            await msg.ModifyAsync(x =>
                {
                    x.Content = "Roles Updated";
                });
        }

        public async Task GetParse(string server, [Remainder] string character, ulong userID)
        {
            Console.WriteLine("\n ");
            var roles = GetRoles();
            var user = Context.Guild.GetUserAsync(Context.User.Id);
            var jobslist = new List<Nero.job>();
            var worlds = Nero.Worlds.GetWorlds();
            var worldResult = from wrld in worlds
                              where wrld.Name.ToLower().Contains(server.ToLower())
                              select wrld;
            var world = worldResult.First();
            var url = new Uri($"https://www.fflogs.com/v1/parses/character/{character}/{world.Name}/{world.Region}/?api_key={Configuration.Load().FFLogsKey}");
            HttpClient client = HTTPHelpers.NewClient();

            var player = new Player(userID, character, world.DC, world.Name);
            await AddDataCenterWorldRoles(roles, user, player);

            try
            {
                Console.WriteLine($"FFlogs URl: {url}");
                string responseBody = await client.GetStringAsync(url);
                 
                var parses = JsonConvert.DeserializeObject<List<Nero.Parses>>(responseBody);
                

                // instantiate best% and bestdps
                double bestPercent = 0.0;
                double bestDps = 0.0;

                foreach (var parse in parses) {
                    bool cleared = false;
                    int specAmount = 0;
                    if (parse.kill > 0) {
                        cleared = true;
                    }
                    List<job> specs = new List<job>();

                    foreach(var spec in parse.specs) {
                        Console.WriteLine($"parse fight: {parse.name}, spec name: {spec.spec}, spec hist %: {spec.best_historical_percent}");
                        specs.Add(new job(spec.spec, spec.best_historical_percent, spec.best_persecondamount));
                        if (spec.best_persecondamount >= bestDps)
                            bestDps = spec.best_persecondamount;
                        
                        if (spec.best_historical_percent >= bestPercent) 
                            bestPercent = spec.best_historical_percent;
                        
                        

                        specAmount++;
                    }
                    player.AddFight(cleared, parse.kill, specAmount, specs, parse.name, bestDps, bestPercent);
                    bestPercent = 0.0;
                    bestDps = 0.0;
                    
                }
                    await AssignRolesAsync(roles, user, player);

                

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException caught");
                Console.WriteLine($"Message: {e.Message}");
            }

            client.Dispose();
        }

        
        [Command("view")]
        [Alias("v")]
        public async Task ViewProfile() {
            await UpdateProfile(Context.User);
           await SendProfile(Context.User);
        }


        [Command("view")]
        [Alias("v")]
        public async Task TaskViewUserProfile(IUser serverUser) {
            await UpdateProfile(serverUser);
            Console.WriteLine($"user: {serverUser.Username} - {serverUser.Id}");
            await SendProfile(serverUser);
        }

        [Command("update")]
        [Alias("u")]
        public async Task UpdateProfile(IUser serverUser) {
            Console.WriteLine($"mention update| Name: {serverUser.Username} ID: {serverUser.Id}");
            var player = Player.Load(serverUser.Id);
            await GetParse(player.world, player.playerName, serverUser.Id);
        }

        public async Task SendProfile(IUser User) {
            var context = Context;
             Console.WriteLine($"does the players profile exist: {Player.DoesProfileExist(User.Id)}");
            if (!Player.DoesProfileExist(User.Id)) {
                await ReplyAsync($"Profile does for user {Context.User.Username} not exist, please run the following command.\n!n assign `server` `character name`");
                return;
            }
            var player = Player.Load(User.Id);

            if (player.xivdbURL == "" || player.xivdbURL.Length == 0 || player.xivdbURL == null) {
                player.xivdbURL = GetXivDB(player.playerName, player.world, false).Result.ToString();
            }

            if (player.xivdbURL_API == "" || player.xivdbURL_API.Length == 0 || player.xivdbURL_API == null) {
                player.xivdbURL_API = GetXivDB(player.playerName, player.world, true).Result.ToString();
            }

            Console.WriteLine($"xivdb: {player.xivdbURL}\napi: {player.xivdbURL_API}");

            var client = HTTPHelpers.NewClient();

            string responseBody = await client.GetStringAsync(player.xivdbURL_API);         
            var xivdbCharacter = JsonConvert.DeserializeObject<XivDB.XIVDBCharacter>(responseBody);
            
            var raidJobs = "";
            foreach(var job in player.jobs) {
                raidJobs += $" - **{job.name}**\n" + 
                        $"    •  Historical DPS: {job.historical_dps}\n" +
                        $"    •  Historic Best  %: {job.historical_percent}%\n" +
                        $"    •  Savage Average %: {job.savageP}%\n"; 
            }



            try {
                foreach (var job in xivdbCharacter.data.classjobs.class_jobs) {
                    //jobs += $"{job.name} - {job.level}";
                }
            } catch {
                Console.WriteLine($"classjobs is null: {xivdbCharacter.data.classjobs.class_jobs == null}");
            }

            var clears = "";

            foreach (var clear in player.GetClearedFights(context)) {
                clears += $" - {clear}\n";
            }

            

            var reply = $"**Best DPS:** {player.bestDps}\n" + 
            $"**Avg Best %:** {player.bestPercent}%\n\n" + 
            $"__**Raid Jobs**__\n" + 
            $"{raidJobs}\n" + 
            $"__**Clears**__\n" + 
            $"{clears}\n" + 
            $""; //+ 
            //$"__**Jobs**__\n" + 
            //$"{jobs}";



            var embed = new EmbedBuilder()
            .WithTitle($"{xivdbCharacter.name} - {xivdbCharacter.data.title}")
            .WithUrl(player.xivdbURL)
            .WithThumbnailUrl(xivdbCharacter.avatar)
            .WithImageUrl(xivdbCharacter.portrait)
            .WithFooter(new EmbedFooterBuilder()
            .WithText($"{player.dc} - {player.world} | {xivdbCharacter.data.race}"))
            .WithColor(new Color(102, 255, 222))
            .WithDescription(reply)
            .Build();


            await ReplyAsync("", embed: embed);
            
        }
        public async Task<string> GetXivDB(string name, string world, bool api) {
            if (api == true) {
                var url = new Uri($"https://api.xivdb.com/search?one=characters&string={name}&pretty=1");
            var client = HTTPHelpers.NewClient();

            string responseBody = await client.GetStringAsync(url);         
            var xivdbCharacters = JsonConvert.DeserializeObject<CharacterSearch>(responseBody);
            
            var results = from charResult in xivdbCharacters.characters.results
            where charResult.name.ToLower() == name.ToLower() && charResult.server == world
            select charResult;

            var playerLink = results.First().url_api;

            return playerLink;
            } else {
                var url = new Uri($"https://api.xivdb.com/search?one=characters&string={name}&pretty=1");
            var client = HTTPHelpers.NewClient();

            string responseBody = await client.GetStringAsync(url);         
            var xivdbCharacters = JsonConvert.DeserializeObject<CharacterSearch>(responseBody);
            
            var results = from charResult in xivdbCharacters.characters.results
            where charResult.name.ToLower() == name.ToLower() && charResult.server == world
            select charResult;

            var playerLink = results.First().url_xivdb;

            return playerLink;
            }
        }

    }
}