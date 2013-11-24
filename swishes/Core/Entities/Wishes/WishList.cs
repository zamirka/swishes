using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swishes.Core.Entities.Wishes
{
    [Table("WishLists")]
    public class WishList
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public virtual ICollection<Wish> Whishes { get; set; }
    }
}