

using Microsoft.AspNetCore.Mvc.Rendering;

namespace marvin2.Models.WebModels
{
    public class ChoreViewModel
    {
        public SelectList ChoreTypes{ get; private set; }

        private SelectListItem WeeklyChoreItem = new SelectListItem("Weekly Chore", "weeklychore");
        private SelectListItem MonthlyChoreItem = new SelectListItem("Monthly Chore", "monthlychore");
        private SelectListItem DailyChoreItem = new SelectListItem("Daily Chore", "dailychore");
        
        public DailyChore? DailyChore { get; set; }
        public WeeklyChore? WeeklyChore{ get; set; }
        public MonthlyChore? MonthlyChore{ get; set; }
        
        public bool AdditionalItem{ get; set; }
        public bool? Success = null;
        
        public ChoreViewModel()
        {
            List<SelectListItem> items = new List<SelectListItem>()
            {
                WeeklyChoreItem,
                MonthlyChoreItem,
                DailyChoreItem
            };

            ChoreTypes = new SelectList(items);
        }
        
        public bool IsValid()
        {
            if (DailyChore is not null && WeeklyChore is null && MonthlyChore is null) return true;
            if (DailyChore is null && WeeklyChore is not null && MonthlyChore is null) return true;
            if (DailyChore is null && WeeklyChore is null && MonthlyChore is not null) return true;

            return false;
        }
    }
}