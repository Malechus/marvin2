

using Microsoft.AspNetCore.Mvc.Rendering;

namespace marvin2.Models.WebModels
{
    /// <summary>
    /// View model for binding an existing <see cref="Chore"/> to an existing <see cref="Person"/>,
    /// creating a scheduled PersonChore assignment. Chore creation/editing is handled separately
    /// by <see cref="ManageChoresViewModel"/>.
    /// </summary>
    public class ChoreViewModel
    {
        /// <summary>
        /// The chore type ("dailychore", "weeklychore", "monthlychore") chosen on the form.
        /// Bound directly to the select element; <see cref="ChoreTypes"/> only supplies its options.
        /// </summary>
        public string SelectedChoreType { get; set; } = "dailychore";

        public SelectList ChoreTypes{ get; private set; }

        public int? PersonId { get; set; }
        public SelectList? People { get; set; }

        public int? ChoreId { get; set; }
        public SelectList? Chores { get; set; }

        public DailyChore.Priority DailyPriority { get; set; } = DailyChore.Priority.Medium;
        public string WeeklyDayOfWeek { get; set; } = "Monday";
        public int? MonthlyDayOfMonth { get; set; } = 1;

        public bool AdditionalItem{ get; set; }
        public bool? Success = null;
        
        public ChoreViewModel()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            SelectListItem dc = new SelectListItem();
            dc.Text = "Daily Chores";
            dc.Value = "dailychore";
            items.Add(dc);

            SelectListItem wc = new SelectListItem();
            wc.Text = "Weekly Chores";
            wc.Value = "weeklychore";
            items.Add(wc);

            SelectListItem mc = new SelectListItem();
            mc.Text = "Monthly Chores";
            mc.Value = "monthlychore";
            items.Add(mc);

            ChoreTypes = new SelectList(items, "Value", "Text");
        }
        
        public bool IsValid()
        {
            if (!PersonId.HasValue || !ChoreId.HasValue) return false;

            return SelectedChoreType switch
            {
                "dailychore" => true,
                "weeklychore" => !string.IsNullOrWhiteSpace(WeeklyDayOfWeek),
                "monthlychore" => MonthlyDayOfMonth is > 0 and < 32,
                _ => false,
            };
        }
    }
}