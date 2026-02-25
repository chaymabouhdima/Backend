using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Circuit   // ← حرف كبير
    {
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        public string Description { get; set; }

        public decimal Prix { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;
    }
}