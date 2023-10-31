using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExpenses.Models.Models
{
    internal class MyExpenses
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
