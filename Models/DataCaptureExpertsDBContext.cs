using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataCaptureExpertsWebAPI.Models
{
    public partial class DataCaptureExpertsDBContext : DbContext
    {
        public DataCaptureExpertsDBContext()
        {
        }

        public DataCaptureExpertsDBContext(DbContextOptions<DataCaptureExpertsDBContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                //optionsBuilder.UseSqlServer("Server=Tavish-Laptop;Database=DataCaptureExpertsDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
