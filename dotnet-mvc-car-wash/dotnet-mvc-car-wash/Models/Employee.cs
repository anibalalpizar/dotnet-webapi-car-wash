using System.ComponentModel.DataAnnotations;

namespace dotnet_mvc_car_wash.Models
{
    public class Employee
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [Required]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        [Required]
        [Display(Name = "Daily Salary")]
        public decimal DailySalary { get; set; }

        [Display(Name = "Accumulated Vacation Days")]
        public int AccumulatedVacationDays { get; set; }

        [Display(Name = "Termination Date")]
        public DateTime? TerminationDate { get; set; }

        [Display(Name = "Severance Amount")]
        public decimal? SeveranceAmount { get; set; }
    }
}
