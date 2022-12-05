using Discord;
using Discord.Commands;
using Bumba;

namespace Bumba
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        public async Task TestCommand()
        {
            await ReplyAsync(":D");
        }
        [Command("dog")]
        public async Task SayCommand()
        {
            RestApi restClient = new RestApi();
            var responseFromApi = restClient.GetPicture("api/breeds/image/random");
            await ReplyAsync(responseFromApi.message);
        }
    }
}
