using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Models
{
    [Table("CvLink")]
    public class CV
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public byte[] CVLink { get; set; }
    }
}
