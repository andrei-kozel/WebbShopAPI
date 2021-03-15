﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WebbShopApi.Models
{
    class SoldBooks
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int CategoryId { get; set; }
        public int Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int UserId { get; set; }
    }
}
