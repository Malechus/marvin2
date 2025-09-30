

using System.Text;
using Discord;
using Discord.WebSocket;
using marvin2.Models;
using marvin2.Services;

namespace marvin2.discord.SlashCommands
{
    public class ListChores
    {
        private readonly ChoreService _choreService;
        
        public ListChores(ChoreService choreService)
        {
            _choreService = choreService;
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
        
        public async Task HandleCommand(SocketSlashCommand command)
        {
            if (command.Data.Options.Count == 0) listAllChores(command);
            else listChores(command);
        }
        
        private async Task listAllChores(SocketSlashCommand command)
        {

            StringBuilder stringBuilder = new StringBuilder();
            
            string dayOfWeek = DateTime.Today.DayOfWeek.ToString();
            int dayOfMonth = DateTime.Today.Day;

            List<DailyChore> dailyChores = _choreService.GetDailyChores();
            List<WeeklyChore> weeklyChores = _choreService.GetWeeklyChores(dayOfWeek);
            List<MonthlyChore> monthlyChores = _choreService.GetMonthlyChores(dayOfMonth);

            List<Person> people = _choreService.GetPeople();
            
            foreach(Person person in people)
            {
                List<Chore> chores = new List<Chore>();
                
                foreach(DailyChore dailyChore in dailyChores)
                {
                    if(dailyChore.Person == person){ chores.Add(dailyChore); }
                }
                
                foreach(WeeklyChore weeklyChore in weeklyChores)
                {
                    if(weeklyChore.Person == person){ chores.Add(weeklyChore); }
                }
                
                foreach(MonthlyChore monthlyChore in monthlyChores)
                {
                    if(monthlyChore.Person == person){ chores.Add(monthlyChore); }
                }

                stringBuilder.AppendLine(person.Name + @"'s chores for today are:");
                foreach(Chore chore in chores){ stringBuilder.AppendLine(chore.Name); }
            }

            command.RespondAsync(stringBuilder.ToString());
        }
        
        private static async Task listChores(SocketSlashCommand command)
        {
            
        }
    }
}