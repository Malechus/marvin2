namespace marvin2.Models.WebModels
{
    /// <summary>
    /// View model backing the standalone Chore CRUD page (create/list chores),
    /// kept separate from <see cref="ChoreViewModel"/> which binds chores to people.
    /// </summary>
    public class ManageChoresViewModel
    {
        public Chore NewChore { get; set; } = new Chore();
        public List<Chore> ExistingChores { get; set; } = new List<Chore>();
        public bool? Success { get; set; }
    }
}
