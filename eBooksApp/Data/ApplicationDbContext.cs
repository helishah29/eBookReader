using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using eBooksApp.Models;

namespace eBooksApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<eBook> eBooks { get; set; }
        public DbSet<Comments> Comments { get; set; }

        public DbSet<ErrorViewModel> Errors { get; set; }
        //public DbSet<eBookItem> eBookItems { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<Comments>().ToTable("Comments");
        //}
    }
}
