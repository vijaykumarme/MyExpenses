using System.ComponentModel.DataAnnotations;

namespace MyExpenses.Models
{
    public class Expenses
    {
        [Key]
        public int Id { get; set; }
        [Required]

        public int Category { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public String Location { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int Money { get; set; }
    }
}
