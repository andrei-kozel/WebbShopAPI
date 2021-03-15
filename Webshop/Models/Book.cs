using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebbShopApi.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public int Price { get; set; }
        public int Amount { get; set; }
        public int CategoryId { get; set; }
    }
}
