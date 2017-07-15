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


namespace Nero
{

    public class fflogs : ModuleBase {
        
        public Dictionary<string, ulong> GetRoles() {
            var roles = new Dictionary<string, ulong>();
            foreach(var role in Context.Guild.Roles) {
                //Console.WriteLine($"{role.Name} - {roles.ContainsKey(role.Name)}");
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

        
        public async Task AssignRolesAsync(Dictionary<string, ulong> roles, Task<IGuildUser> user, Player _player) {
            var clearedFights = _player.GetClearedFights();
            
            if (clearedFights.Count == 0) {
                    await ReplyAsync("This player has not cleared any extreme/savage fights");
            }

                // Susano Role
            if (clearedFights.Contains("Susano") && user.Result.RoleIds.Contains(roles["cleared-susano-ex"]) == false)
                await AddRoleAsync(roles, "cleared-susano-ex", user);
            Console.WriteLine("Susano added");

                // Lakshmi Role
            if (clearedFights.Contains("Lakshmi") && user.Result.RoleIds.Contains(roles["cleared-lakshmi-ex"]) == false)
            {
                await AddRoleAsync(roles, "cleared-lakshmi-ex", user);
                Console.WriteLine("Lakshmi added");
            }

            // top 5%
            if (_player.bestPercent >= 95.0 && _player.fightsCleared == 2) // magic number lol
            {
                await AddRoleAsync(roles, "bigdps-ex", user);
                Console.WriteLine($"bigdps-ex added");
            }

            // World Role
            if (!roles.ContainsKey(_player.world.ToLower())) {
                await ReplyAsync($"@{Context.Guild.GetRole(roles["administrator"])} role: {_player.world.ToLower()} does not exist yet, please create it.");
            }
            else
            {
                if (user.Result.RoleIds.Contains(roles[_player.world.ToLower()]) == false)
                    //await user.Result.AddRoleAsync(Context.Guild.GetRole(roles[world.Name.ToLower()]));
                    await AddRoleAsync(roles, _player.world, user);
            }

            // DC Role
            if (!roles.ContainsKey(_player.dc.ToLower()))
            {
                await ReplyAsync($"@{Context.Guild.GetRole(roles["administrator"])} role: {_player.dc} does not exist yet, please create it.");
            }
            else
            {
                if (user.Result.RoleIds.Contains(roles[_player.dc.ToLower()]) == false)
                    //await user.Result.AddRoleAsync(Context.Guild.GetRole(roles[world.DC.ToLower()]));
                    await AddRoleAsync(roles, _player.dc, user);
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
                        await ReplyAsync($"@{Context.Guild.GetRole(roles["administrator"])} role: {classjob.name} does not exist yet, please create it.");
                    }
                    else
                    {
                        if (user.Result.RoleIds.Contains(roles[classjob.name.ToLower()]) == false)
                            await AddRoleAsync(roles, classjob.name, user);
                    }

                    //Role role
                    if (!roles.ContainsKey(classjob.role.ToLower()))
                    {
                        await ReplyAsync($"@{Context.Guild.GetRole(roles["administrator"])} role: {classjob.role} does not exist yet, please create it.");
                    }
                    else
                    {
                        if (user.Result.RoleIds.Contains(roles[classjob.role.ToLower()]) == false)
                            await AddRoleAsync(roles, classjob.role, user);
                    }

                }
            }
            Console.WriteLine($"total: {_player.bestPercent}%, {_player.bestDps}");
        }
        

        [Command("assign")]
        [Alias("a")]
        public async Task GetParse(string server, [Remainder] string character)
        {
            var msg = await ReplyAsync("Working...");

            Console.WriteLine("\n ");
            var roles = GetRoles();
            var jobslist = new List<Nero.job>();
            var worlds = Nero.Worlds.GetWorlds();
            var worldResult = from wrld in worlds
                              where wrld.Name.ToLower().Contains(server.ToLower())
                              select wrld;
            var world = worldResult.First();
            var url = new Uri($"https://www.fflogs.com/v1/parses/character/{character}/{world.Name}/{world.Region}/?api_key={Configuration.Load().FFLogsKey}");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var player = new Player(character, world.DC, world.Name);

            try
            {
                string responseBody = await client.GetStringAsync(url);
                var parses = JsonConvert.DeserializeObject<List<Nero.RootObject>>(responseBody);
                var user = Context.Guild.GetUserAsync(Context.User.Id);

                double bestPercent = 0.0;
                double bestDps = 0.0;

                foreach (var parse in parses)
                {
                    bool cleared = false;
                    int specAmount = 0;
                    if (parse.kill > 0) {
                        cleared = true;
                    }
                    List<job> specs = new List<job>();

                    foreach(var spec in parse.specs) {
                        specs.Add(new job(spec.spec, spec.best_historical_percent));
                        if (spec.best_persecondamount >= bestDps)
                            bestDps = spec.best_persecondamount;
                        
                        if (spec.best_historical_percent >= bestPercent)
                            bestPercent = spec.best_historical_percent;

                        specAmount++;
                    }
                    player.AddFight(cleared, parse.kill, specAmount, specs, parse.name, bestDps, bestPercent);
                    
                }
                    await AssignRolesAsync(roles, user, player);


                await msg.ModifyAsync(x =>
                {
                    x.Content = "Roles Updated";
                });
                

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException caught");
                Console.WriteLine($"Message: {e.Message}");
            }

            client.Dispose();
        }
    }
}