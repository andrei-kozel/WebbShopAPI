using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebbShopApi.Models
{
    class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; } = "Codic2021";
        public DateTime LastLogin { get; set; }
        public DateTime SessionTimer { get; set; }
        public bool IsActiove { get; set; } = true;
        public bool IsAdmin { get; set; } = false;
    }
}
