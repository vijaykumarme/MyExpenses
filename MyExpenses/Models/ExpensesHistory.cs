using System.ComponentModel.DataAnnotations;

namespace MyExpenses.Models
{
    public class ExpensesHistory
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public int ExpId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Money { get; set; }
        [Required]
        public string Location { get; set; }

    }
}
