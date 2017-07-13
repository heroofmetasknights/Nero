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
            //7bd977bcb89a89934dc26a137b6d2b24
            var world = worldResult.First();
            var url = new Uri($"https://www.fflogs.com/v1/parses/character/{character}/{world.Name}/{world.Region}/?api_key={Configuration.Load().FFLogsKey}");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            bool cleared_susano = false;
            double best_susano_dps = 0;
            double best_susano_percent = 0;
            bool cleared_lakshmi = false;
            double best_lakshmi_dps = 0;
            double best_lakshmi_percent = 0;
            bool big_dps = false;
            double best_historical_perc = 0.0;

            try
            {
                string responseBody = await client.GetStringAsync(url);
                var parses = JsonConvert.DeserializeObject<List<Nero.RootObject>>(responseBody);
                string reply = "";
                var user = Context.Guild.GetUserAsync(Context.User.Id);

                //TODO: Fights object
                foreach (var parse in parses)
                {
                    //Susano
                    if (parse.name == "Susano" && parse.kill > 0)
                    {
                        cleared_susano = true;

                        foreach (var spec in parse.specs)
                        {
                            jobslist.Add(new Nero.job(spec.spec, spec.best_historical_percent));

                            if (spec.best_persecondamount >= best_susano_dps)
                            {
                                best_susano_dps = spec.best_persecondamount;
                            }
                            if (spec.best_historical_percent >= best_susano_percent)
                            {
                                best_susano_percent = spec.best_historical_percent;
                            }
                        }
                    }
                    //Lakshmi
                    if (parse.name == "Lakshmi" && parse.kill > 0)
                    {
                        cleared_lakshmi = true;

                        foreach (var spec in parse.specs)
                        {
                            jobslist.Add(new Nero.job(spec.spec, spec.best_historical_percent));

                            if (spec.best_persecondamount >= best_lakshmi_dps)
                            {
                                best_lakshmi_dps = spec.best_persecondamount;
                            }
                            if (spec.best_historical_percent >= best_lakshmi_percent)
                            {
                                best_lakshmi_percent = spec.best_historical_percent;

                            }
                        }
                    }
                    //Future
                }

                if (cleared_lakshmi == false && cleared_susano == false)
                {
                    reply += "This user has not cleared any extreme/savage content.";
                }

                // Susano Role
                if (cleared_susano == true && user.Result.RoleIds.Contains(roles["cleared-susano-ex"]) == false)
                    await user.Result.AddRoleAsync(Context.Guild.GetRole(roles["cleared-susano-ex"]));
                Console.WriteLine("Susano added");

                // Lakshmi Role
                if (cleared_lakshmi == true && user.Result.RoleIds.Contains(roles["cleared-lakshmi-ex"]) == false)
                {
                    await user.Result.AddRoleAsync(Context.Guild.GetRole(roles["cleared-lakshmi-ex"]));
                    Console.WriteLine("Lakshmi added");
                }

                //BigDPS Role
                if (cleared_lakshmi == true && cleared_susano == true)
                {
                    big_dps = true;
                    best_historical_perc = (best_lakshmi_percent + best_susano_percent) / 2.0;
                }

                // top 5%
                if (best_historical_perc >= 95.0)
                {
                    await user.Result.AddRoleAsync(Context.Guild.GetRole(roles["bigdps-ex"]));
                }

                // World Role
                if (!roles.ContainsKey(world.Name.ToLower()))
                {
                    await ReplyAsync($"@{Context.Guild.GetRole(roles["administrator"])} role: {world.Name} does not exist yet, please create it.");
                }
                else
                {
                    if (user.Result.RoleIds.Contains(roles[world.Name.ToLower()]) == false)
                        await user.Result.AddRoleAsync(Context.Guild.GetRole(roles[world.Name.ToLower()]));
                }

                // DC Role
                if (!roles.ContainsKey(world.DC.ToLower()))
                {
                    await ReplyAsync($"@{Context.Guild.GetRole(roles["administrator"])} role: {world.DC} does not exist yet, please create it.");
                }
                else
                {
                    if (user.Result.RoleIds.Contains(roles[world.DC.ToLower()]) == false)
                        await user.Result.AddRoleAsync(Context.Guild.GetRole(roles[world.DC.ToLower()]));
                }

                if (jobslist.Count == 0)
                {
                    await msg.ModifyAsync(x =>
                    {
                        x.Content = "No parses found";
                    });
                    return;
                }
                else
                {
                    foreach (var classjob in jobslist)
                    {

                        // Job role
                        if (!roles.ContainsKey(classjob.name.ToLower()))
                        {
                            await ReplyAsync($"@{Context.Guild.GetRole(roles["administrator"])} role: {classjob.name} does not exist yet, please create it.");
                        }
                        else
                        {
                            if (user.Result.RoleIds.Contains(roles[classjob.name.ToLower()]) == false)
                                await user.Result.AddRoleAsync(Context.Guild.GetRole(roles[classjob.name.ToLower()]));
                        }

                        //Role role
                        if (!roles.ContainsKey(classjob.role.ToLower()))
                        {
                            await ReplyAsync($"@{Context.Guild.GetRole(roles["administrator"])} role: {classjob.role} does not exist yet, please create it.");
                        }
                        else
                        {
                            if (user.Result.RoleIds.Contains(roles[classjob.role.ToLower()]) == false)
                                await user.Result.AddRoleAsync(Context.Guild.GetRole(roles[classjob.role.ToLower()]));
                        }

                    }
                }

                await msg.ModifyAsync(x =>
                {
                    x.Content = "Roles Updated";
                });
                Console.WriteLine($"susano % {best_susano_percent}, lakshmi % {best_lakshmi_percent}, total: {best_historical_perc}");

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