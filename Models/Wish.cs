using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using swishes.Models.Entities;

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

        [Column("Priority", TypeName = "int")]
        public WishPrioriries Prioriry { get; set; }

        [Column("Status", TypeName = "int")]
        public WishStatuses Status { get; set; }

        public int WishListId { get; set; }

        public string ImageName { get; set; }

        public decimal? Price { get; set; }
    }
}