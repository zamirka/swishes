using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace swishes.Core.Entities.Profile
{
    [Table("webpages_Membership")]
    public class Membership
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        public DateTime? CreateDate { get; set; }

        [MaxLength(128)]
        public string ConfirmationToken { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTime? LastPasswordFailureDate { get; set; }

        [Required]
        public int PasswordFailuresSinceLastSuccess { get; set; }

        [Required]
        [MaxLength(128)]
        public string Password { get; set; }

        public DateTime? PasswordChangedDate { get; set; }

        [Required]
        [MaxLength(128)]
        public string PasswordSalt { get; set; }

        [MaxLength(128)]
        public string PasswordVerificationToken { get; set; }

        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }

        public Membership()
        {
            IsConfirmed = false;
            PasswordFailuresSinceLastSuccess = 0;

        }
    }
}