using System;
using System.Collections.Generic;
using System.Text;

namespace WebbShopApi.Models
{
    class Book
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public int Price { get; set; }
        public int Amount { get; set; }
        public int CategoryId { get; set; }
    }
}
