using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swishes.Models
{
    [Table("Wishes")]
    public class Wish
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(250)]
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

        [Timestamp]
        public byte[] TimeStamp { get; set; }

        [Column("UserId")]
        [Required]
        public string UserId { get; set; }

        public UserProfile Profile { get; set; }
    }
}