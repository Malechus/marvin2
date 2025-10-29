

using Discord;
using Discord.WebSocket;

namespace marvin2.discord.SlashCommands
{
    public interface ISlashCommandHandler
    {
        public SlashCommandBuilder CreateBuilder();

        public Task HandleCommand(SocketSlashCommand command, SocketTextChannel responseChannel);

        public Task TriggerResponse(SocketTextChannel responseChannel);
    }
}