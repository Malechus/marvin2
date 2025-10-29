using System.Runtime.CompilerServices;
using Azure.Messaging;
using data.Services;
using Discord;
using Discord.WebSocket;
using marvin2.Models.PiModels;

namespace marvin2.discord.SlashCommands
{
    public class Status : ISlashCommandHandler
    {
        private readonly PiService _piService;
        
        public Status(PiService piService)
        {
            _piService = piService;
        }
        
        public SlashCommandBuilder CreateBuilder()
        {
            SlashCommandBuilder builder = new SlashCommandBuilder();
            builder.WithName("status");
            builder.WithDescription("Gets the status of various tools and services necessary to the house.");

            return builder;
        }
        
        public async Task HandleCommand(SocketSlashCommand command, SocketTextChannel responseChannel)
        {
            await ReportPiStatus(responseChannel);
            await GetTopClients(responseChannel);
            await GetBlockedClients(responseChannel);
        }
        
        public async Task TriggerResponse(SocketTextChannel responseChannel)
        {
            
        }
        
        private async Task ReportPiStatus(SocketTextChannel responseChannel)
        {
            bool isBlocking = _piService.IsBlocking();
            
            if(isBlocking)
            {
                await responseChannel.SendMessageAsync("The Pi Hole is up and blocking.");
            }
            else
            {
                await responseChannel.SendMessageAsync("The Pi Hole is down or not blocking.");
            }
        }
        
        private async Task GetTopClients(SocketTextChannel responseChannel)
        {
            await responseChannel.SendMessageAsync("### Top Pi Hole Clients:");
        
            List<QueryClient> clientList = _piService.GetTopClients();
            
            foreach(QueryClient client in clientList)
            {
                await responseChannel.SendMessageAsync("Name: " + client.name + " , Queries: " + client.count);
            }

            await responseChannel.SendMessageAsync("...");
        }
        
        private async Task GetBlockedClients(SocketTextChannel responseChannel)
        {
            await responseChannel.SendMessageAsync("### Top Blocked Clients:");

            List<QueryClient> clientList = _piService.GetTopBlockedClients();
            
            foreach(QueryClient client in clientList)
            {
                await responseChannel.SendMessageAsync("Name: " + client.name + " , Queries: " + client.count);
            }

            await responseChannel.SendMessageAsync("...");
        }
    }
}