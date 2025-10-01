using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marvin2.Models
{
    [NotMapped]
    public abstract class PersonChore
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("PersonId")]        
        private Person? _person;
        public virtual Person? Person
        {
            get
            {
                return _person;
            }
            set
            {
                if(value != null)
                {
                    this._person = value;
                    OnSetPerson(value);
                }
            }
        }
        [ForeignKey("ChoreId")]
        private Chore? _chore;
        public virtual Chore? Chore
        {
            get
            {
                return _chore;
            }
            set
            {
                if(value != null)
                {
                    this._chore = value;
                    OnSetChore(value);
                }
            }
        }

        private void OnSetPerson(Person person) 
        {
            Person = person;
        }
        
        private void OnSetChore(Chore chore)
        {
            Chore = chore;
        }
		
		
        
        public void Activate()
        {
            IsActive = true;
        }
        
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}