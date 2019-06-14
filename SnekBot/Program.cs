using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Net;
using SnekBot.Data;

namespace SnekBot
{
    public class Program
    {
        
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            SnekBot snek = SnekBot.Instance;
            await snek.OnStartup();

            
            await Task.Delay(-1);
        }

        
    }
}
