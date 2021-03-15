using System;
using System.Collections.Generic;
using System.Text;

namespace WebbShopApi.Database
{
using Microsoft.EntityFrameworkCore;

class MyContext : DbContext
    {
        private const string DatabaseName = "WebbShopAndreiKozel";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($@"Server = .\SQLEXPRESS;Database={DatabaseName};trusted_connection=true");
        }
    }
}
