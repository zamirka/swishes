﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace swishes.Models
{
    [Table("webpages_OAuthMembership")]
    public class OAuthMembership
    {
        [Key]
        [Required]
        [MaxLength(30)]
        public string Provider { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProviderUserId { get; set; }

        [Required]
        [Column("UserId")]
        public int UserId { get; set; }

        public UserProfile Profile { get; set; }
    }
}