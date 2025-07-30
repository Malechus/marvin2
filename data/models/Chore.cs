using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marvin2.Models
{
    [NotMapped]
    public abstract class Chore
    {
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
        
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
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

        public void OnSetPerson(Person person) 
        {
            Person = person;
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