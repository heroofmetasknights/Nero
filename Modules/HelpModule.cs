using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System;
using System.IO;
using Newtonsoft.Json;

namespace Manderville.Modules {
	public class HelpModule : ModuleBase<SocketCommandContext> {
		private CommandService _service;

		// Create a constructor for the CommandService dependency
		public HelpModule(CommandService service) {
			_service = service;
		}



        [Command("help")]
        [Alias("h")]
        [Summary("Lists all commands")]
        public async Task HelpAsync() {
            var application = await Context.Client.GetApplicationInfoAsync();
            string reply = $"__**Player Commands**__\n" +
                $"**A**ssign `server` `character name`:  Assigns a user roles depending on their fflogs rankings. \n" +
                $"**V**iew: Views a users profile.\n" +
                $"**V**iew `@mention`: view a specified users profile.\n" +
                $"**V**iew `server` `character name`: View a user by server and character name.\n" +
                "**I**nvite**B**ot: Gives a link to invite nero to your server.\n" +
                $"__**Static Commands**__\n" +
                $"**S**tatic **A**pplications: View, approve or deny applications to the static.\n" +
                $"**S**tatic **C**reate: Launches the static creation wizard. **!!Needs a discord server!!**\n" +
                $"**S**tatic **F**ilters: View/Edit Static Recruitment Filters.\n" +
                $"**S**tatic **J**oin: Launches the static join wizard.\n" +
                $"**S**tatic **J**oin `Static Name`: Skips the wizard.\n" +
                $"**S**tatic **K**ick: Launches a prompt to kick a member from a static\n" +
                $"**S**tatic **L**ist: Lists all recruiting statics - not available for use in DM's.\n" +
                $"**S**tatic **L**ist `job acronym`: Lists all statics recruiting for that particular job - not available for use in DM's.\n" +
                $"**S**tatic **R**ecruitment: Enables or disables Static Recruitment - not available for use in DM's.\n" +
                $"**S**tatic **S**earch `name`: Searches for statics by name - not available for use in DM's.\n" +
                "**S**tatic **V**iew: Views the members of your static and their clears.\n" +
                $"__**Server Commands**__\n" +
                $"Setup: Launches a prompt to resetup your server.\n" +
                $"Settings: Lets you view/change server settings.\n" +
                $"**Contact**\n" +
                $"Please send all feature suggestions to:" +
                $"<@202862830934818816>.\n" +
								$"Please send all bot problems to:" +
								$"{application.Owner.Mention}.\n" +
                $"Donate to keep the bot alive and contribute to future development:\n" +
                $"http://paypal.me/NeroBot";


            var embed = new EmbedBuilder()
                .WithColor(new Color(250, 140, 73))
                .WithTitle("Help")
                .WithFooter(new EmbedFooterBuilder().WithText($"Nero v4.25.0"))
                .WithDescription(reply)
                .WithUrl("https://gist.github.com/Infinifrui/43bc37aeb91533699ff748f787e61852")
                .Build();


            await ReplyAsync("", embed: embed);
        }

        [Command("help")]
		[Alias("h")]
		[Summary("Lists help for specified command")]
		public async Task HelpAsync([Remainder]string input) {
            var result = from Module in _service.Modules
                         from command in Module.Commands
                         where command.Name.ToLower().Contains(input.ToLower()) && Module.Name != "RecipeMisc"
                         select command;

            var builder = new EmbedBuilder() {
                Color = new Color(250, 140, 73),
                Description = $"Here are some commands like **{input}**"
            };

            foreach (var cmd in result) {
                Console.WriteLine($"Input: {input}\nCommand: {cmd.Name}\nmatch: {input.ToLower() == cmd.Name.ToLower()}");
                if (cmd.Remarks != null) {
                    builder.AddField(x => {

                        x.Name = cmd.Module.Name + ": " + string.Join(", ", cmd.Name);
                        x.Value = $"Aliases: `{string.Join(", ", cmd.Aliases)}`\nSummary: {cmd.Summary}\n";
                    });
                }

                Console.WriteLine("Builder: Field Added");
            }

            builder.Build();

            if (result == null) {
				await ReplyAsync($"Sorry, I couldn't find a command like **{input}**");
				return;
			}

			await ReplyAsync("", embed: builder);
		}

        [Command("Invite Bot")]
        [Alias("ib")]
        public async Task InviteBot() {
            await ReplyAsync("If you want to invite the original Nero (aka <@202862830934818816>'s version : https://discordapp.com/oauth2/authorize?permissions=2080898303&scope=bot&client_id=332176591042117634");
						await ReplyAsync("If you want to invite this copy of Nero : https://discordapp.com/oauth2/authorize?permissions=2080898303&scope=bot&client_id=396993856366837760");
        }

        [Command("guild")]
        public async Task UserInfo() {
            var i = "";
            foreach (var g in Context.Client.Guilds) {
                if (Context.User.Id == g.Owner.Id)
                    i += $"{g.Name}\n";
            }

            await Context.User.SendMessageAsync($"{i}");
        }



        [Command("send")]
        [RequireOwner]
        public async Task sendMes(string server, [Remainder]string message) {
            var guilds = Context.Client.Guilds;
            var targetlist = from g in guilds
            where g.Name.ToLower() == server.ToLower()
            select g;

            var target = targetlist.First();

            await target.Owner.SendMessageAsync(message);

        }

        [Command("notify")]
        [RequireOwner]
        public async Task sendNot([Remainder]string message) {
            var guilds = Context.Client.Guilds;
            foreach (var serv in guilds) {
                await serv.Owner.SendMessageAsync($"Notification: {message}");
            }
        }



    }
}
