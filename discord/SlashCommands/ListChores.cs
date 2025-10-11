

using System.Text;
using Discord;
using Discord.WebSocket;
using marvin2.discord.Services;
using marvin2.Models;
using marvin2.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Identity.Client;

namespace marvin2.discord.SlashCommands
{
    public class ListChores
    {
        private readonly ChoreService _choreService;
        private readonly ResponseService _responseService;
        
        public ListChores(ChoreService choreService, ResponseService responseService)
        {
            _choreService = choreService;
            _responseService = responseService;
        }
        
        public SlashCommandBuilder CreateBuilder()
        {
            SlashCommandBuilder builder = new SlashCommandBuilder();
            builder.WithName("listchores");
            builder.WithDescription("Lists the chores for all members of the family, or for one person (optionally.)");
            builder.AddOption(new SlashCommandOptionBuilder()
            .WithName("person")
            .WithDescription("Optional: The person for whom to list chores.")
            .WithRequired(false)
            .AddChoice("Colin", "Colin")
            .AddChoice("Debi", "Debi")
            .AddChoice("Garrett", "Garrett")
            .AddChoice("Caitlin", "Caitlin")
            .AddChoice("Morgan", "Morgan")
            .AddChoice("Liam", "Liam")
            .AddChoice("Theo", "Theo")
            .AddChoice("Diane", "Diane")
            .AddChoice("Cassie", "Cassie")
            .WithType(ApplicationCommandOptionType.String));

            return builder;
        }
        
        public async Task HandleCommand(SocketSlashCommand command, SocketTextChannel responseChannel)
        {
            await command.RespondAsync(_responseService.GetRandomResponse());
            if (command.Data.Options.Count == 0) listAllChores(command, responseChannel);
            else listChores(command);
        }
        
        private async Task listAllChores(SocketSlashCommand command, SocketTextChannel responseChannel)
        {
            string dayOfWeek = DateTime.Today.DayOfWeek.ToString();
            int dayOfMonth = DateTime.Today.Day;

            List<Person> people = _choreService.GetPeople();
            
            foreach(Person person in people)
            {

                StringBuilder stringBuilder = new StringBuilder();
                
                List<PersonChore> personChores = _choreService.GetPersonChores(person);

                List<Chore> chores = new List<Chore>();
                
                foreach(PersonChore personChore in personChores)
                {
                    switch(personChore.GetType().Name)
                    {
                        case nameof(DailyChore):
                            chores.Add(personChore.Chore);
                            break;
                        case nameof(WeeklyChore):
                            WeeklyChore weeklyChore = personChore as WeeklyChore;
                            if (weeklyChore.DayOfWeek == dayOfWeek) chores.Add(personChore.Chore);
                            break;
                        case nameof(MonthlyChore):
                            MonthlyChore monthlyChore = personChore as MonthlyChore;
                            if (monthlyChore.DayOfMonth == dayOfMonth) chores.Add(personChore.Chore);
                            break;
                        default:
                            break;
                    }
                }

                stringBuilder.AppendLine(@"## " + person.Name + @"'s chores for today are:");
                foreach(Chore chore in chores)
                { 
                    stringBuilder.AppendLine(chore.Name);
                    stringBuilder.AppendLine(@"-# " + chore.Description);
                }
                stringBuilder.AppendLine();

                await responseChannel.SendMessageAsync(stringBuilder.ToString());
            }
        }
        
        private async Task listChores(SocketSlashCommand command)
        {
            ISocketMessageChannel channel = command.Channel;
        
            string name = command.Data.Options.First().Value.ToString();

            Person? person = _choreService.GetPerson(name);

            if (person is null) command.RespondAsync("Name not valid, try again.");
            
            StringBuilder stringBuilder = new StringBuilder();
            
            string dayOfWeek = DateTime.Today.DayOfWeek.ToString();
            int dayOfMonth = DateTime.Today.Day;
            
            List<PersonChore> personChores = _choreService.GetPersonChores(person);

            List<Chore> chores = new List<Chore>();
            
            foreach(PersonChore personChore in personChores)
            {
                switch(personChore.GetType().Name)
                {
                    case nameof(DailyChore):
                        chores.Add(personChore.Chore);
                        break;
                    case nameof(WeeklyChore):
                        WeeklyChore weeklyChore = personChore as WeeklyChore;
                        if (weeklyChore.DayOfWeek == dayOfWeek) chores.Add(personChore.Chore);
                        break;
                    case nameof(MonthlyChore):
                        MonthlyChore monthlyChore = personChore as MonthlyChore;
                        if (monthlyChore.DayOfMonth == dayOfMonth) chores.Add(personChore.Chore);
                        break;
                    default:
                        break;
                }
            }

            stringBuilder.AppendLine(@"## " + person.Name + @"'s chores for today are:");
            stringBuilder.AppendLine();
            foreach(Chore chore in chores)
            { 
                stringBuilder.AppendLine(chore.Name);
                stringBuilder.AppendLine(@"-# " + chore.Description);
            }
            stringBuilder.AppendLine();
            
            channel.SendMessageAsync(stringBuilder.ToString());
        }
    }
}