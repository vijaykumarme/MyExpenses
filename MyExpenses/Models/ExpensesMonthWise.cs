using System.ComponentModel.DataAnnotations;

namespace MyExpenses.Models
{
    public class ExpensesMonthWise
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public int Money { get; set; }
        [Required]
        public int Month { get; set; }
    }
}
