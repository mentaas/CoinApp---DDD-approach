using CoinApp.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinApp.Domain.Entities
{
    public class User //: Entity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        [Column(TypeName = "varbinary(max)")]
        public string Password { get; set; }
        [Required]
        [Column(TypeName = "varbinary(max)")]
        public string PasswordSalt { get; set; }
        public DateTime EntryDate { get; set; }

        public string RefreshToken { get; set; }

        public ICollection<Coin> Coins { get; set; }

    }
}
