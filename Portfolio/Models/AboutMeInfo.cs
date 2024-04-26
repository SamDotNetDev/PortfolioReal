using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Models
{
    [Table("AboutMe")]
    public class AboutMeInfo
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string AboutMe { get; set; }
    }
}
