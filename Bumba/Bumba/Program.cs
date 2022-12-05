using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Bumba.config;

class Program
{
    static Task Main(string[] args)
    {
        return new Program().MainAsync();
    }

    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private Program()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,

        });
        _commands = new CommandService(new CommandServiceConfig
        {
            LogLevel = LogSeverity.Info,

            CaseSensitiveCommands = false,
        });
        _client.Log += Log;
        _commands.Log += Log;
        _services = ConfigureServices();
    }
    private static IServiceProvider ConfigureServices()
    {
        var map = new ServiceCollection();
        return map.BuildServiceProvider();
    }
    private static Task Log(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }
        Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
        Console.ResetColor();
        return Task.CompletedTask;
    }
    private async Task MainAsync()
    {
        await InitCommands();
        ConfigObject config = JsonConvert.DeserializeObject<ConfigObject>(File.ReadAllText("C:\\Users\\Admin\\Desktop\\C#Nauka\\Bumba-DiscordBot\\Bumba\\Bumba\\config\\config.json"));
        await _client.LoginAsync(TokenType.Bot, config.BotSecretToken);
        await _client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }
    private async Task InitCommands()
    {
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        _client.MessageReceived += HandleCommandAsync;
    }
    private async Task HandleCommandAsync(SocketMessage arg)
    {
        Console.WriteLine(arg.Content);
        var msg = arg as SocketUserMessage;
        if (msg == null) return;
        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;
        int pos = 0;
        if (msg.HasCharPrefix('!', ref pos) || msg.HasMentionPrefix(_client.CurrentUser, ref pos))
        {
            var context = new SocketCommandContext(_client, msg);
            var result = await _commands.ExecuteAsync(context, pos, _services);
            if (!result.IsSuccess)
            {
                Console.WriteLine($"{DateTime.Now} at Commands] Something went wrong with executing a command. Text: {context.Message.Content} | Error: {result.ErrorReason}");
            }
        }
    }
}