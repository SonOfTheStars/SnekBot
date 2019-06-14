using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using SnekBot.Data;
using SnekBot.Modules.Protection;

namespace SnekBot
{
    public class SnekBot
    {
        private static DiscordClient discord;
        private static ProtectionHandler protectionHandler;

        #region Singleton

        private static SnekBot _instance;

        private SnekBot()
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Console.WriteLine("Initializing SnekBot....");
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = DataService.GetConfig("Token"),
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            protectionHandler = new ProtectionHandler(discord);
        }

        public static SnekBot Instance => _instance ?? new SnekBot();

        #endregion

        public async Task OnStartup()
        {
            await discord.ConnectAsync();
        }

        private async void OnProcessExit(object sender, EventArgs e)
        {
            DataService.OnShutdown();
            await discord.DisconnectAsync().ConfigureAwait(false);
        }
    }
}
