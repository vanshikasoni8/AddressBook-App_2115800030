using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ModelLayer.Model;

namespace ModelLayer.DTO

{
    [Table("AddressEntries")]
    public class AddressBookEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [ForeignKey("Users")]
        public int UserId { get; set; }

        [JsonIgnore]
        public UserEntity User { get; set; }



    }
}
