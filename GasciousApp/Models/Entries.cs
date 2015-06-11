using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GasciousApp.Models
{
    public class Entry
    {
        public int Id { get; set; }
        [Display(Name="Date (MM-DD-YYYY)")]
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public decimal Gallons { get; set; }
        [Required]
        [StringLength(30)]
        public string Station { get; set; }
        [StringLength(30)]
        public string Vehicle { get; set; }
        [Required]
        [Range(1.0, 600.0)]
        [Display(Name="Trip Length (Miles)")]
        public decimal Miles { get; set; }
        public string Username { get; set; }    // not shown
    }
}