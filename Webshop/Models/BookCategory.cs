using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebbShopApi.Models
{
    public class BookCategory
    {
        [Key]
        public int BookCategoryId { get; set; }
        public string Name { get; set; }
    }
}
