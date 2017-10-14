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
using Discord.Addons.Interactive;
using Newtonsoft.Json;
using Nero.Modules.Utilities;

namespace Nero {
	[Group("static")]
	[Alias("s")]
	public partial class LFG : InteractiveBase {
		//private readonly PaginationService paginator;

		//public LFG(PaginationService _paginator) {
		//	paginator = _paginator;
		//}

		[Command("create", RunMode = RunMode.Async)]
		[Alias("c")]
		public async Task CreateStatic() {
			await Context.User.SendMessageAsync("Welcome to the Static Setup Wizard. Next, please invite the bot (use the command `!n invite bot` for a link) to your static's discord server **(MAKE SURE that the name of the static and the server match for this)** and select yes for the question asking if it's for a static. Type next to continue");

			var goNextResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
			
			if (goNextResponse.Content.ToLower() == "next") {
				// Static Name
				await Context.User.SendMessageAsync("Please input your static name (**NOTE** Must match static discord name)");
				var staticNameResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));

				// Static Datacenter
				await Context.User.SendMessageAsync("Please enter the datacenter (primal, chaos, aether, etc) where your static is located: ");
				var staticDCResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));

				if(staticDCResponse.Content.ToLower() != "primal" && staticDCResponse.Content.ToLower() != "aether" && staticDCResponse.Content.ToLower() != "elemental" && staticDCResponse.Content.ToLower() != "gaia" && staticDCResponse.Content.ToLower() != "chaos" && staticDCResponse.Content.ToLower() != "mana") {
					await Context.User.SendMessageAsync("Thats not a datacenter, (primal, chaos, aether, etc) try again: ");
					staticDCResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
				}

				// Gets the Server, probably should refactor this
				var serv = Context.Client.Guilds;
				var results = from server in serv
				where server.Name.ToLower().Contains(staticNameResponse.Content.ToLower())
				select server;

				if (results.Count() == 0) {
					await Context.User.SendMessageAsync($"Error: Static server `{staticNameResponse.Content}` not found, are you sure you added Nero to it?");
					return;
				}
				
				// Loads the server and player objects to set the player static ID
				ulong seId = results.First().Id;
				Server serpento = Server.Load(seId);
				var player = Player.Load(Context.User.Id);
				player.staticId = serpento.discordServerID;
				player.EnsureExists();

				// Creates the Static Object
				PlayerStatic ps = new PlayerStatic(results.First().Id, staticNameResponse.Content.ToLower(), staticDCResponse.Content.ToLower(), Context.User.Id, serpento.invite);

				// Gets the recruitment filters from the user
				await Context.User.SendMessageAsync("Please input any recruiting filters you want seperated by commas, these can be changed later. Filter List: `job`, `cleared-o#s`\nTo skip this type **skip**");
				var filterResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
				if (!filterResponse.Content.ToLower().Contains("skip")) {
					var filterList = filterResponse.Content.ToLower().Split(',').ToList();
					ps.Filters = filterList;
				}

				// Recruitment
				await Context.User.SendMessageAsync("Enable Recruitment? This can be enabled later. Y/N");
				var recruitmentResponse = await NextMessageAsync(true, false, timeout:TimeSpan.FromMinutes(5));
				if (recruitmentResponse.Content.ToLower() == "y") {
					ps.recruiting = true;	
				}
				await Context.User.SendMessageAsync("Recruitment set");

				// Adds the Creator to the Member List
				ps.Members.Add(Context.User.Username, Context.User.Id);
				ps.EnsureExists();

				await Context.User.SendMessageAsync("Static setup Complete.\nNow instruct your members to use the command !n static join  to start the wizard. Nero will then dm you for approval before sending them a server invite.");

			}
			
		}

		[Command("recruitment", RunMode = RunMode.Async)]
		[Alias("r")]
		public async Task RecruitmentToggle() {
			var player = Player.Load(Context.User.Id);
			
			PlayerStatic ps = ListOwnerServers(Context).Result;
			

			if (ps.recruiting == true) {
				await Context.User.SendMessageAsync($"Recruitment is **Enabled** for {ps.PlayerStaticName}");
			} else {
				await Context.User.SendMessageAsync($"Recruitment is **Disabled** for {ps.PlayerStaticName}");
			}

			await Context.User.SendMessageAsync("Type On to enable Recruitment, Off to Disable recruitment or Exit to exit this prompt");
			var recruitmentResponse = await NextMessageAsync(true, false, timeout:TimeSpan.FromMinutes(5));
			if (recruitmentResponse.Content.ToLower() == "on") {
				ps.recruiting = true;
				ps.EnsureExists();
				await Context.User.SendMessageAsync("Recruitment is now **Enabled**");
			} else if (recruitmentResponse.Content.ToLower() == "off") {
				ps.recruiting = false;
				ps.EnsureExists();
				await Context.User.SendMessageAsync("Recruitment is now **Disabled**");
			} else {
				return;
			}
		}

		[Command("view", RunMode = RunMode.Async)]
		[Alias("v")]
		public async Task ViewStatic() {
			var player = Player.Load(Context.User.Id);
			if(player.staticId == 0) {
				await Context.User.SendMessageAsync("You are not in a static");
				return;
			} 
			PlayerStatic ps = ListOwnerServersInChannel(Context).Result;
			var reply = "";

			foreach (var member in ps.Members) {
				if(Player.DoesProfileExist(member.Value)) {
					var membah = Player.Load(member.Value);
					reply += $"{membah.playerName} - ";
					var cleared = membah.GetClearedFights();
					foreach (var clear in cleared) {
						reply += $"{clear} ";
					}
					reply += "\n";
				} else {
					reply += $"{member.Key}\n";
				}
			}

			var embed = new EmbedBuilder()
				.WithTitle(ps.PlayerStaticName)
				.WithColor(new Color(250, 140, 73))
				.WithDescription(reply)
				.WithFooter(new EmbedFooterBuilder()
				.WithText($"{ps.Applications.Count()} pending application(s)"))
				.Build();

			await ReplyAsync("", embed: embed);
		}


		[Command("filters", RunMode = RunMode.Async)]
		[Alias("f")]
		public async Task UpdateFilters() {
			var player = Player.Load(Context.User.Id);
			PlayerStatic ps = ListOwnerServers(Context).Result;

			var reply = "";

			if (ps.Filters != null) {
				foreach(var filt in ps.Filters) {
					reply += $"{filt}\n";
				}
			}
			

			await Context.User.SendMessageAsync($"Current Filters for {ps.PlayerStaticName}:\n{reply}\nPlease input new filters seperated by commas. Available filters: `job`, `cleared-o#s`\nThe old filters are overwritten\nTo exit type **exit**");
			if (ps.Filters != null){
				ps.Filters.Clear();
			}
			var filterResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
			if(filterResponse.Content.ToLower() == "exit") {
				await Context.User.SendMessageAsync("exiting");
				return;
			}
			var filterList = filterResponse.Content.ToLower().Split(',').ToList();
			ps.Filters = filterList;
			ps.EnsureExists();
			await Context.User.SendMessageAsync("Filters set");
		}

		[Command("join", RunMode = RunMode.Async)]
		[Alias("j")]
		public async Task JoinStatic() {
			if (Player.DoesProfileExist(Context.User.Id) == false) {
				await Context.User.SendMessageAsync("Please run the command !n add profile `world` `firstname lastname` first.");
				return;
			}
			await Context.User.SendMessageAsync("Greetings! Please input the name of the static(case insensitive) you wish to join: ");
			var staticNameR = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));

			var serv = Context.Client.Guilds;
				var results = from server in serv
				where server.Name.ToLower().Contains(staticNameR.Content.ToLower())
				select server;

				if (results.Count() == 0) {
					await Context.User.SendMessageAsync($"Error: Static server `{staticNameR.Content} not found");
					return;
				}

			PlayerStatic ps = PlayerStatic.Load(results.First().Id);
			Player playa = Player.Load(Context.User.Id);
			
			if (playa.dc.ToLower() != ps.dc.ToLower()) {
				await Context.User.SendMessageAsync($"You are not located in the {ps.dc} datacenter");
			}

			if (!ps.Applications.ContainsValue(Context.User.Id) && !ps.Members.ContainsValue(Context.User.Id)) {
				if (ps.Filters.Count == 0) {
					ps.Applications.Add(Context.User.Username.ToLower(), Context.User.Id);
					ps.EnsureExists();
					var fflogs = new fflogs();
					Console.WriteLine("sending profile");
					Embed embed = await fflogs.SendProfileReply(Context.User.Id);
					Console.WriteLine("profile sent");
					await results.First().Owner.SendMessageAsync($"New Application from {Context.User.Username}, use !n static applications to approve/deny the application", embed: embed);
				} else {
					foreach(var filt in ps.Filters) {
						//Job Filter
						if (filt.Length == 3) {
							foreach (var jb in playa.jobs) {
								if (filt.ToLower() == jb.short_name) {
									ps.Applications.Add(Context.User.Username.ToLower(), Context.User.Id);
									ps.EnsureExists();
									var fflogs = new fflogs();
									Embed embed = await fflogs.SendProfileReply(Context.User.Id);
									await Context.User.SendMessageAsync("Application sent");
									await results.First().Owner.SendMessageAsync($"New Application from {Context.User.Username}, use !n static applications to approve/deny the application", embed: embed);
								} else {
									await Context.User.SendMessageAsync("Requirements not met");
								}
							}
						} 
						// Cleared Fights Filter
						else {
							foreach(var clrd in playa.cleared) {
								if (filt.ToLower().Contains(clrd.ToLower())) {
									ps.Applications.Add(Context.User.Username.ToLower(), Context.User.Id);
									ps.EnsureExists();
									var fflogs = new fflogs();
									Embed embed = await fflogs.SendProfileReply(Context.User.Id);
									await Context.User.SendMessageAsync("Application sent");
									await results.First().Owner.SendMessageAsync($"New Application from {Context.User.Username}, use !n static applications to approve/deny the application", embed: embed);
								} else {
									await Context.User.SendMessageAsync("Requirements not met");
								}
							}
						}
					}
				}
			} else {
				await Context.User.SendMessageAsync($"You are either already a member of {ps.PlayerStaticName} or have already sent in an application");
			}
		}

		[Command("join", RunMode = RunMode.Async)]
		[Alias("j")]
		public async Task JoinStatic([Remainder] string input) {
			if (Player.DoesProfileExist(Context.User.Id) == false) {
				await Context.User.SendMessageAsync("Please run the command !n add profile `world` `firstname lastname` first.");
				return;
			}
			
			var staticNameR = input;

			var serv = Context.Client.Guilds;
				var results = from server in serv
				where server.Name.ToLower().Contains(staticNameR.ToLower())
				select server;

				if (results.Count() == 0) {
					await Context.User.SendMessageAsync($"Error: Static server `{staticNameR} not found");
					return;
				}

			PlayerStatic ps = PlayerStatic.Load(results.First().Id);
			Player playa = Player.Load(Context.User.Id);
			
			if (playa.dc.ToLower() != ps.dc.ToLower()) {
				await Context.User.SendMessageAsync($"You are not located in the {ps.dc} datacenter");
			}

			if (!ps.Applications.ContainsValue(Context.User.Id) && !ps.Members.ContainsValue(Context.User.Id)) {
				if (ps.Filters.Count == 0) {
					ps.Applications.Add(Context.User.Username.ToLower(), Context.User.Id);
					ps.EnsureExists();
					var fflogs = new fflogs();
					Console.WriteLine("sending profile");
					Embed embed = await fflogs.SendProfileReply(Context.User.Id);
					Console.WriteLine("profile sent");
					await results.First().Owner.SendMessageAsync($"New Application from {Context.User.Username}, use !n static applications to approve/deny the application", embed: embed);
				} else {
					foreach(var filt in ps.Filters) {
						//Job Filter
						if (filt.Length == 3) {
							foreach (var jb in playa.jobs) {
								if (filt.ToLower() == jb.short_name) {
									ps.Applications.Add(Context.User.Username.ToLower(), Context.User.Id);
									ps.EnsureExists();
									var fflogs = new fflogs();
									Embed embed = await fflogs.SendProfileReply(Context.User.Id);
									await Context.User.SendMessageAsync("Application sent");
									await results.First().Owner.SendMessageAsync($"New Application from {Context.User.Username}, use !n static applications to approve/deny the application", embed: embed);
								} else {
									await Context.User.SendMessageAsync("Requirements not met");
								}
							}
						} 
						// Cleared Fights Filter
						else {
							foreach(var clrd in playa.cleared) {
								if (filt.ToLower().Contains(clrd.ToLower())) {
									ps.Applications.Add(Context.User.Username.ToLower(), Context.User.Id);
									ps.EnsureExists();
									var fflogs = new fflogs();
									Embed embed = await fflogs.SendProfileReply(Context.User.Id);
									await Context.User.SendMessageAsync("Application sent");
									await results.First().Owner.SendMessageAsync($"New Application from {Context.User.Username}, use !n static applications to approve/deny the application", embed: embed);
								} else {
									await Context.User.SendMessageAsync("Requirements not met");
								}
							}
						}
					}
				}
			} else {
				await Context.User.SendMessageAsync($"You are either already a member of {ps.PlayerStaticName} or have already sent in an application");
			}
		}

		[Command("applications", RunMode = RunMode.Async)]
		[Alias("a")]
		public async Task Applications() {
			var playe = Player.Load(Context.User.Id);

			PlayerStatic ps = ListOwnerServers(Context).Result;

			string reply = "";

			int i=0;
			foreach (var app in ps.Applications) {
				reply += $"	**{i}**. {app.Key}\n";
				Console.WriteLine($"");
				i++;
			}

			await Context.User.SendMessageAsync($"Greetings! here are the Current applications for {ps.PlayerStaticName}: \n{reply}\n type approve or deny to launch a prompt.\n");

			var response = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));

			//Approve
			if (response.Content.ToLower().Contains("approve")) {
				await Context.User.SendMessageAsync("Type one or more names seperated by commas to approve them, or type `all` to approve all applicants");
				var approveResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
				if (approveResponse.Content.Contains(',')) {
					var approveList = approveResponse.Content.Split(',').ToList();
					foreach (var approve in approveList) {
						ulong value = ps.Applications[approve.ToLower()];
						ps.Members.Add(approve.ToLower(), value);
						ps.Applications.Remove(approve.ToLower());
						var usr = Context.Client.GetUser(value);
						var player = Player.Load(usr.Id);
						player.staticId = ps.discordServerID;
						player.EnsureExists();
						await usr.SendMessageAsync($"Your Application to join {ps.PlayerStaticName} has been approved, click here to join: {ps.InviteLink}");
					}
					ps.EnsureExists();
				} else {
					if (approveResponse.Content.ToLower().Substring(0,3) == "all") {
						foreach (var approved in ps.Applications) {
							ps.Members.Add(approved.Key, approved.Value);
							var usr = Context.Client.GetUser(approved.Value);
							var player = Player.Load(usr.Id);
							player.staticId = ps.discordServerID;
							player.EnsureExists();
							await usr.SendMessageAsync($"Your Application to join {ps.PlayerStaticName} has been approved, click here to join: {ps.InviteLink}");	
						}
						ps.Applications.Clear();
						ps.EnsureExists();

					} else {
						ulong value = ps.Applications[approveResponse.Content.ToLower()];
						ps.Members.Add(approveResponse.Content.ToLower(), value);
						ps.Applications.Remove(approveResponse.Content.ToLower());
						var usr = Context.Client.GetUser(value);
						var player = Player.Load(usr.Id);
						player.staticId = ps.discordServerID;
						player.EnsureExists();
						await usr.SendMessageAsync($"Your Application to join {ps.PlayerStaticName} has been approved, click here to join: {ps.InviteLink}");
						ps.EnsureExists();
					}
				}
				
				
			}
			//Deny
			else if (response.Content.ToLower().Contains("deny")) {
				await Context.User.SendMessageAsync("Type one or more names seperated by commas to deny them, or type `all` to deny all applicants");
				var denyResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
				if (denyResponse.Content.ToLower().Contains(',')) {
					var denyList = denyResponse.Content.Split(',').ToList();
					foreach (var deny in denyList) {
						ulong value = ps.Applications[deny.ToLower()];
						ps.Members.Add(deny.ToLower(), value);
						ps.Applications.Remove(deny.ToLower());
					}
					ps.EnsureExists();
					await Context.User.SendMessageAsync("Selected applications cleared.");
				} else {
					if (denyResponse.Content.ToLower().Substring(0,3) == "all") {
							ps.Applications.Clear();
							ps.EnsureExists();
							await Context.User.SendMessageAsync("Applications cleared.");
					} else {
						ulong value = ps.Applications[denyResponse.Content.ToLower()];
						ps.Applications.Remove(denyResponse.Content.ToLower());
						ps.EnsureExists();
					}
				}
			}

		}


		[Command("kick", RunMode = RunMode.Async)]
		[Alias("k")]
		public async Task Memberkick() {
			var playe = Player.Load(Context.User.Id);
			ulong playerID = 0; 
			PlayerStatic ps = ListOwnerServers(Context).Result;

			string reply = "";

			int i=1;
			foreach (var member in ps.Members) {
				reply += $"	**{i}**. {member.Key}\n";
				Console.WriteLine($"");
				i++;
			}

			await Context.User.SendMessageAsync($"Greetings! here are the current members for {ps.PlayerStaticName}: \n{reply}\n type in a players number to kick them\n");

			var kickResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
			
            int responseInt = Convert.ToInt32(kickResponse.Content);
			var membID = ps.Members.ElementAt(responseInt-1).Value;
			var membname = ps.Members.ElementAt(responseInt-1).Key;
			ps.Members.Remove(ps.Members.ElementAt(responseInt-1).Key);
			ps.EnsureExists();

			var targetServ = Context.Client.GetGuild(ps.discordServerID);
			var botInstance = targetServ.GetUser(Context.Client.CurrentUser.Id);
			var targetMember = targetServ.GetUser(membID);

			if (botInstance.GuildPermissions.KickMembers) {
				if (targetServ.OwnerId == targetMember.Id){
					await Context.User.SendMessageAsync("Nero can not kick the owner of that server.");
					return;
				}

				await targetMember.KickAsync();
				await ReplyAsync("Nero kicked " + membname);
			}

			
		}




		[Command("search", RunMode = RunMode.Async)]
		[Alias("s")]
		public async Task SearchStatics([Remainder] string input) {
			var reply = new List<string>();
			reply.Add("");
			var files = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, $"statics/"));

			int i=0;
			int pageIndex = 0;
			foreach(var file in files) {
				ulong servId = Convert.ToUInt64(Path.GetFileNameWithoutExtension(file));

				var ps = PlayerStatic.Load(servId);
				if(ps.recruiting == true) {
					if (ps.PlayerStaticName.ToLower().Contains(input.ToLower())) {
							if ((i % 10 == 0 && i != 1 && i != 0)) {
							reply.Add($"{ps.PlayerStaticName}\n");
							i++;
							pageIndex++;
						} else {
							reply[pageIndex] += ps.PlayerStaticName + "\n";
							i++;
						}
					}
				}
			}

			var message = new Discord.Addons.Interactive.PaginatedMessage();
			message.Title = $"{files.Count()} Statics";
			message.Color = new Color(250, 140, 73);
			message.Pages = reply;
			await PagedReplyAsync(reply);
		}

		[Command("list", RunMode = RunMode.Async)]
		[Alias("l")]
		public async Task ListStatics() {
			var reply = new List<string>();
			reply.Add("");
			var files = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, $"statics/"));

			int i=0;
			int pageIndex = 0;
			foreach(var file in files) {
				ulong servId = Convert.ToUInt64(Path.GetFileNameWithoutExtension(file));

				var ps = PlayerStatic.Load(servId);
				if(ps.recruiting == true) {
					if ((i % 10 == 0 && i != 1 && i != 0)) {
						reply.Add($"{ps.PlayerStaticName}\n");
						i++;
						pageIndex++;
					} else {
						reply[pageIndex] += ps.PlayerStaticName + "\n";
						i++;
					}
				}
					
				
			}

			var message = new Discord.Addons.Interactive.PaginatedMessage();
			message.Title = $"{files.Count()} Statics";
			message.Color = new Color(250, 140, 73);
			message.Pages = reply;
			
			

			await PagedReplyAsync(reply);
		}

		[Command("list", RunMode = RunMode.Async)]
		[Alias("l")]
		public async Task ListStaticsWithJobFilter([Remainder] string job) {
			var reply = new List<string>();
			reply.Add("");
			var files = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, $"statics/"));

			int i = 0;
			int pageIndex = 0;
			foreach(var file in files) {
				ulong servId = Convert.ToUInt64(Path.GetFileNameWithoutExtension(file));

				var ps = PlayerStatic.Load(servId);
				if(ps.recruiting == true) {
					if (ps.Filters.Contains(job.ToLower())) {
						if ((i % 10 == 0 && i != 1 && i != 0)) {
							reply.Add($"{ps.PlayerStaticName}\n");
							i++;
							pageIndex++;
						} else {
							reply[pageIndex] += ps.PlayerStaticName + "\n";
							i++;
						}
					}
				}
			}

			Console.WriteLine($"{files.Count()} files found");
			
			
			var message = new Discord.Addons.Interactive.PaginatedMessage();
			message.Title = $"{files.Count()} Statics";
			message.Color = new Color(250, 140, 73);
			message.Pages = reply;
			await PagedReplyAsync(reply);
		}

		public async Task<PlayerStatic> ListOwnerServers(SocketCommandContext Context) {
			ulong serverID = 0;

			string servers = "";
            int k = 1;
            Dictionary<string, ulong> userControlledservers = new Dictionary<string, ulong>();
            foreach (var g in Context.Client.Guilds) {
                if (Context.User.Id == g.Owner.Id){
					if (PlayerStatic.DoesProfileExist(g.Id)) {
						userControlledservers.Add(g.Name, g.Id);
                     	servers += $"  {k}. {g.Name} \n";
                    	k++;
					}
                     
                }
            }

            if(userControlledservers.Count > 1) {
				await Context.User.SendMessageAsync($"You control the following servers:\n{servers} Please enter the number of the server you want to select.");
            	var serverNameResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
            	int responseInt = Convert.ToInt32(serverNameResponse.Content);
				serverID = userControlledservers.ElementAt(responseInt-1).Value;
            	await Context.User.SendMessageAsync($"You have selected: **{userControlledservers.ElementAt(responseInt-1).Key}**");
			} else {
				serverID = userControlledservers.Values.First();
			}
			
			

			PlayerStatic ps = PlayerStatic.Load(serverID);
			return ps;
		}

		public async Task<PlayerStatic> ListOwnerServersInChannel(SocketCommandContext Context) {
			ulong serverID = 0;

			string servers = "";
            int k = 1;
            Dictionary<string, ulong> userControlledservers = new Dictionary<string, ulong>();
            foreach (var g in Context.Client.Guilds) {
                if (Context.User.Id == g.Owner.Id){
					if (PlayerStatic.DoesProfileExist(g.Id)) {
						userControlledservers.Add(g.Name, g.Id);
                     	servers += $"  * {g.Name} \n";
                    	k++;
					}
                     
                }
            }

			if(userControlledservers.Count > 1) {
				await ReplyAsync($"You control the following servers:\n{servers} Please enter the name of the server you want to select, it does not have to be the full name.");
            	var serverNameResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
            	foreach(var u in userControlledservers) {
                	if (u.Key.ToLower().Contains(serverNameResponse.Content.ToLower())) {
                    	serverID = u.Value;
                    	break;
                	}
            	}
			} else {
				serverID = userControlledservers.Values.First();
			}
            
			
			

			PlayerStatic ps = PlayerStatic.Load(serverID);
			return ps;
		}

	}
}