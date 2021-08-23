﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinApp.Domain.Entities
{
    public class Coin
    {
        public Coin()
        {
            Users = new HashSet<User>();
        }
        [Key]
        public string Id { get; set; }
        [Required]
        public string Rank { get; set; }
        [Required]
        public string Symbol { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Supply { get; set; }
        [Required]
        public string MaxSupply { get; set; }

        public ICollection<User> Users { get; set; }

    }
}
