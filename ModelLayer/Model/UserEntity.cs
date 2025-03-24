using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ModelLayer.DTO;

namespace ModelLayer.Model
{
    [Table("Users")]
    public class UserEntity
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]

        public string PasswordHash { get; set; }

        public string? ResetToken { get; set; }

        public DateTime? ResetTokenExpiry { get; set; }

        [JsonIgnore]
        public List<AddressBookEntity> Contacts { get; set; }   = new List<AddressBookEntity>();

    }
}
