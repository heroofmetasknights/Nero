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
				await Context.User.SendMessageAsync("Please input your static name");
				var staticNameResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));

				// Static Datacenter
				await Context.User.SendMessageAsync("Please enter the datacenter where your static is located: ");
				var staticDCResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));

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
				await Context.User.SendMessageAsync("Please input any recruiting filters you want seperated by commas, these can be changed later. Filter List: `job`, `cleared-o#s`");
				var filterResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
				var filterList = filterResponse.Content.ToLower().Split(',').ToList();
				ps.Filters = filterList;

				// Recruitment
				await Context.User.SendMessageAsync("Enable Recruitment? This can be enabled later. Y/N");
				var recruitmentResponse = await NextMessageAsync(true, false, timeout:TimeSpan.FromMinutes(5));
				if (recruitmentResponse.Content.ToLower() == "y") {
					ps.recruiting = true;	
				}

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
			PlayerStatic ps = PlayerStatic.Load(player.staticId);
			if (ps.recruiting == true) {
				await Context.User.SendMessageAsync("Recruitment is Enabled");
			} else {
				await Context.User.SendMessageAsync("Recruitment is Disabled");
			}

			await Context.User.SendMessageAsync("Type On to enable Recruitment, Off to Disable recruitment or Exit to exit this prompt");
			var recruitmentResponse = await NextMessageAsync(true, false, timeout:TimeSpan.FromMinutes(5));
			if (recruitmentResponse.Content.ToLower() == "on") {
				ps.recruiting = true;
				ps.EnsureExists();	
			} else if (recruitmentResponse.Content.ToLower() == "Off") {
				ps.recruiting = true;
				ps.EnsureExists();
			} else {
				return;
			}
		}


		[Command("filters", RunMode = RunMode.Async)]
		[Alias("f")]
		public async Task UpdateFilters() {
			var player = Player.Load(Context.User.Id);
			PlayerStatic ps = PlayerStatic.Load(player.staticId);

			var reply = "";
			foreach(var filt in ps.Filters) {
				reply += $"{filt}\n";
			}

			await Context.User.SendMessageAsync($"Current Filters for {ps.PlayerStaticName}:\n{reply}\nPlease input new filters seperated by commas. Available filters: `job`, `cleared-o#s`");
			ps.Filters.Clear();
			var filterResponse = await NextMessageAsync(true, false, timeout: TimeSpan.FromMinutes(5));
			var filterList = filterResponse.Content.ToLower().Split(',').ToList();
			ps.Filters = filterList;
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

			if (!ps.Applications.ContainsValue(Context.User.Id)) {
				if (ps.Filters.Count == 0) {
					ps.Applications.Add(Context.User.Username.ToLower(), Context.User.Id);
					ps.EnsureExists();
					var fflogs = new fflogs();
					Console.WriteLine("sending profile");
					Embed embed = await fflogs.SendProfileDM(Context.User.Id);
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
									Console.WriteLine("sending profile");
									Embed embed = await fflogs.SendProfileDM(Context.User.Id);
									Console.WriteLine("profile sent");
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
									Console.WriteLine("sending profile");
									Embed embed = await fflogs.SendProfileDM(Context.User.Id);
									Console.WriteLine("profile sent");
									await results.First().Owner.SendMessageAsync($"New Application from {Context.User.Username}, use !n static applications to approve/deny the application", embed: embed);
								} else {
									await Context.User.SendMessageAsync("Requirements not met");
								}
							}
						}
					}
				}
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

			if (!ps.Applications.ContainsValue(Context.User.Id)) {
				if (ps.Filters.Count == 0) {
					ps.Applications.Add(Context.User.Username.ToLower(), Context.User.Id);
					ps.EnsureExists();
					var fflogs = new fflogs();
					Console.WriteLine("sending profile");
					Embed embed = await fflogs.SendProfileDM(Context.User.Id);
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
									Console.WriteLine("sending profile");
									Embed embed = await fflogs.SendProfileDM(Context.User.Id);
									Console.WriteLine("profile sent");
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
									Console.WriteLine("sending profile");
									Embed embed = await fflogs.SendProfileDM(Context.User.Id);
									Console.WriteLine("profile sent");
									await results.First().Owner.SendMessageAsync($"New Application from {Context.User.Username}, use !n static applications to approve/deny the application", embed: embed);
								} else {
									await Context.User.SendMessageAsync("Requirements not met");
								}
							}
						}
					}
				}
			}
		}

		[Command("applications", RunMode = RunMode.Async)]
		[Alias("a")]
		public async Task Applications() {
			
			var playe = Player.Load(Context.User.Id);
			PlayerStatic ps = PlayerStatic.Load(playe.staticId);
			string reply = "";

			int i=0;
			foreach (var app in ps.Applications) {
				reply += $"	**{i}**. {app.Key}\n";
				Console.WriteLine($"");
				i++;
			}

			await Context.User.SendMessageAsync($"Greetings! here are the Current applications: \n{reply}\n type approve or deny to launch a prompt.\n");

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
			message.Color = new Color(102,255,222);
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
			message.Color = new Color(102,255,222);
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
			message.Color = new Color(102,255,222);
			message.Pages = reply;
			await PagedReplyAsync(reply);
		}

	}
}