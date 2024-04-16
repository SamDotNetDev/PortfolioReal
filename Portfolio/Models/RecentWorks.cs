using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Models
{
    [Table("RecentWorks")]
    public class RecentWorks
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string AboutProject { get; set; }
    }
}
