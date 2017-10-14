using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;

namespace Nero {
    public class Server {
        public string fileName;
        public ulong discordServerID;
        public string discordServerName;
        public int memberCount;
        public int channelCount;
        public int roleCount;

        public bool useRoles;

        public Dictionary<string, ulong> ServerStatics = new Dictionary<string, ulong>();
        public bool isStatic;

        public string invite;

        public Server(ulong _discordServerID, string _discordServerName, int _memberCount, int _channelCount, int _roleCount, string _invite) {
            fileName = $"servers/{_discordServerID}.json";
            discordServerID = _discordServerID;
            discordServerName = _discordServerName;
            memberCount = _memberCount;
            channelCount = _channelCount;
            roleCount = _roleCount;
            isStatic = false;
            useRoles = false;
            invite = _invite;
            this.EnsureExists();
        }

        public void EnsureExists() {
            string file = Path.Combine(AppContext.BaseDirectory, fileName);
            if (!File.Exists(file)) {
                string path = Path.GetDirectoryName(file);
                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                this.SaveJson();
            } else {
                this.SaveJson();
            }
        }

        public void SaveJson() {
            string file = Path.Combine(AppContext.BaseDirectory, fileName);
            File.WriteAllText(file, ToJson());
        }

        public static Server Load(ulong id) {
            string file = Path.Combine(AppContext.BaseDirectory, $"servers/{id}.json");
            return JsonConvert.DeserializeObject<Server>(File.ReadAllText(file));
        }

        public static bool DoesProfileExist(ulong id) {
            string file = Path.Combine(AppContext.BaseDirectory, $"servers/{id}.json");
            if (!File.Exists(file)) {
                return false;
            } else {
                return true;
            }
        }

        public string ToJson() 
            => JsonConvert.SerializeObject(this, Formatting.Indented);

    }

    public class ServerIntro : InteractiveBase {
        
        public static async Task JoinedServer(SocketGuild guild, SocketGuildUser owner) {
            await owner.SendMessageAsync("Hi there, to start setup type `!n setup`, if you encounter any problems just run the setup command again.");
            return;
        }


        [Command("settings", RunMode = RunMode.Async)]
        public async Task ServerSettings() {
            ulong serverID = 0;
            bool ex = false;
            
            Console.WriteLine(Context.Message.Source.ToString());
            
            string servers = "";
            int i = 1;
            Dictionary<string, ulong> userControlledservers = new Dictionary<string, ulong>();
            foreach (var g in Context.Client.Guilds) {
                if (Context.User.Id == g.Owner.Id){
                     userControlledservers.Add(g.Name, g.Id);
                     servers += $"  {i}. {g.Name} \n";
                     i++;
                }
            }

            await Context.User.SendMessageAsync($"You control the following servers:\n{servers} Please enter the number of the server you want.");
            var serverNameResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
            int responseInt = Convert.ToInt32(serverNameResponse.Content);
            serverID = userControlledservers.ElementAt(responseInt-1).Value;
            await Context.User.SendMessageAsync($"You have selected: **{userControlledservers.ElementAt(responseInt-1).Key}**");

            var guild = Context.Client.GetGuild(serverID);

            var server = Server.Load(serverID);

            var currentSettings = 
            "__**Current Settings**__\n" + 
            $"Discord: {server.discordServerName}\n" + 
            $"ID: {server.discordServerID}\n" + 
            $"Members: {guild.MemberCount}\n" + 
            $"Static: {server.isStatic}\n" +
            $"Use Roles: {server.useRoles}\n" + 
            $"";

            await Context.User.SendMessageAsync(currentSettings + "\nType the name of the setting you wish to change.");

            var settingsChoiceResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
            switch (settingsChoiceResponse.Content.ToLower()) {
                case "static":
                    await Context.User.SendMessageAsync("Type **Y** to make this server a static or **N** to make it a normal server.");
                    var settingsStaticResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
                    if (settingsStaticResponse.Content.ToLower() == "Y") {
                        server.isStatic = true;
                        server.EnsureExists();
                    } else {
                        server.isStatic = false;
                        server.EnsureExists();
                    }
                    break;
                case "use roles":
                await Context.User.SendMessageAsync("Type **Y** to make this server a static or **N** to make it a normal server.");
                    var settingsRoleResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
                    if (settingsRoleResponse.Content.ToLower() == "Y") {
                        server.useRoles = true;
                        server.EnsureExists();
                    } else {
                        server.useRoles = false;
                        server.EnsureExists();
                    }
                    break;
                default:
                    break;
            }

        }

        [Command("setup", RunMode = RunMode.Async)]
        public async Task Test_NextMessageAsync() {
            ulong serverID = 0;
            
            Console.WriteLine(Context.Message.Source.ToString());
            
            string servers = "";
            int i = 1;
            Dictionary<string, ulong> userControlledservers = new Dictionary<string, ulong>();
            foreach (var g in Context.Client.Guilds) {
                if (Context.User.Id == g.Owner.Id){
                     userControlledservers.Add(g.Name, g.Id);
                     servers += $"  {i}. {g.Name} \n";
                     i++;
                }
            }


            await Context.User.SendMessageAsync($"You control the following servers:\n{servers} Please enter the number of the server you want to setup.");
            var serverNameResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
            int responseInt = Convert.ToInt32(serverNameResponse.Content);
            serverID = userControlledservers.ElementAt(responseInt-1).Value;
            await Context.User.SendMessageAsync($"You have selected: **{userControlledservers.ElementAt(responseInt-1).Key}**");

            var guild = Context.Client.GetGuild(serverID);

            await Context.User.SendMessageAsync("Is this server for a static? Y/N");
            var response = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
            if (response.Content.ToLower() == "y") {
                await Context.User.SendMessageAsync("Please enter an non-expiring invite link to your server.");
                var inviteResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));

                var server = new Server(guild.Id, guild.Name, guild.Users.Count(), guild.Channels.Count(), guild.Roles.Count, inviteResponse.Content);
                server.isStatic = true;
                await Context.User.SendMessageAsync("Do you want to use Nero's role system? Y/N");

                var roleResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));

                if (roleResponse.Content.ToLower() == "y")
                    server.useRoles = true;
                
                server.EnsureExists();
                await Context.User.SendMessageAsync("Setup Complete!");

            } else {
                Server server = new Server(guild.Id, guild.Name, guild.Users.Count(), guild.Channels.Count(), guild.Roles.Count, "");
                
                await Context.User.SendMessageAsync("Do you want to use Nero's role system? Y/N");

                var roleResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));

                if (roleResponse.Content.ToLower() == "y")
                    server.useRoles = true;
                
                await Context.User.SendMessageAsync("Does this server host multiple statics? Y/N (**Note** The static must have already ran `!n static create` if they haven't choose N). However you can always add them later.");


                var multipleResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
                if (multipleResponse.Content.ToLower() == "y") {
                    await Context.User.SendMessageAsync("Input the static(s) name with a trailing comma like so `Nyan Direction, Catgirl Bargains, 123Go`, **otherwise** type `invalid` and rerun the setup.");
                    var staticResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
                    if (!staticResponse.Content.ToLower().Contains("invalid")) {
                        if (staticResponse.Content.Contains(',')) {
                            var staticList = staticResponse.Content.Split(',').ToList();
                            ulong j = 0;
                            foreach (var s in staticList) {
                                server.ServerStatics.Add(s, j);
                                j++;
                            }
                        } else {
                            Random random = new Random();
                            int k = random.Next();
                            ulong l = Convert.ToUInt64(k);
                            server.ServerStatics.Add(staticResponse.Content, l);
                        }
                        
                    }
                }
                server.EnsureExists();
                await Context.User.SendMessageAsync("Setup Complete!, to change any setting you can run `!n settings`");
            }

        }

        
    }

}