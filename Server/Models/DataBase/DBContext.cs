using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TheVoid.Models.DataBase
{
    public class DBContext : DbContext{

        public DBContext(){
           
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=SampleDB;User Id=sa;Password=Gtx580OC;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public class Order{
            [Key]
            public int Order_Id { get; set; }
            public string First_Name { get; set; }
            public string Last_Name { get; set; }
            public virtual ICollection<Ticket> Tickets { get; set; }
        }

        public class Ticket{
            [Key]
            public int Ticket_Id { get; set; }
            public string Ticket_Number { get; set; }
            public DateTime Event_Date { get; set; }
            public int Order_Id { get; set; }
            public Order Order { get; set; }
        }
      
        public DbSet<Order> Orders { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

    }
}