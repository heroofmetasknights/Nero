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
            await owner.SendMessageAsync("Hi there, to start setup type `!n setup`");
            return;
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
                     servers += $"  * {g.Name} \n";
                     i++;
                }
            }


            await Context.User.SendMessageAsync($"You control the following servers:\n{servers} Please enter the name of the server you want to setup. Copy and Pasting is recommended.");
            var serverNameResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
            foreach(var u in userControlledservers) {
                if (u.Key.ToLower().Contains(serverNameResponse.Content.ToLower())) {
                    serverID = u.Value;
                    break;
                }
            }

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
                await Context.User.SendMessageAsync("Setup Complete!");
            }

        }

        
    }

}