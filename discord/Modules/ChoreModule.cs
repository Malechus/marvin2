using Discord;
using Discord.Commands;
using Discord.WebSocket;
using marvin2.Models;
using marvin2.Services;


namespace marvin2.Modules
{
    public class ChoreModule : ModuleBase<SocketCommandContext>
    {
        private readonly IServiceProvider _provider;
        private readonly ChoreService _choreService;
        
        public ChoreModule(IServiceProvider serviceProvider, ChoreService choreService)
        {
            _provider = serviceProvider;
            _choreService = choreService;
        }
        
        //[Command("Chores")]
        //[Summary("Lists the chores that need to be completed today.")]
        //public async Task GetChoresAsync()
        //{
        //    string dayOfWeek = DateTime.Today.DayOfWeek.ToString();
        //    int dayOfMonth = DateTime.Today.Day;
//
        //    List<DailyChore> dailyChores = _choreService.GetDailyChores();
        //    List<WeeklyChore> weeklyChores = _choreService.GetWeeklyChores(dayOfWeek);
        //    List<MonthlyChore> monthlyChores = _choreService.GetMonthlyChores(dayOfMonth);
//
        //    List<Person> people = _choreService.GetPeople();
        //    
        //    foreach(Person person in people)
        //    {
        //        List<Chore> chores = new List<Chore>();
        //        
        //        foreach(DailyChore dailyChore in dailyChores)
        //        {
        //            if(dailyChore.Person == person){ chores.Add(dailyChore); }
        //        }
        //        
        //        foreach(WeeklyChore weeklyChore in weeklyChores)
        //        {
        //            if(weeklyChore.Person == person){ chores.Add(weeklyChore); }
        //        }
        //        
        //        foreach(MonthlyChore monthlyChore in monthlyChores)
        //        {
        //            if(monthlyChore.Person == person){ chores.Add(monthlyChore); }
        //        }
//
        //        await ReplyAsync(person.Name + @"'s chores for today are:");
        //        foreach(Chore chore in chores){ await ReplyAsync(chore.Name); }
        //    }
        //}
        
    }
}