using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Shares.Model
{
    public partial class Shares_DBContext : DbContext
    {
        private readonly IConfiguration Configuration;
        public virtual DbSet<Share> Shares { get; set; } = null!;
        public Shares_DBContext(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //To protect potentially sensitive information in your connection string, you should move it out of source code.
                //You can avoid scaffolding the connection string by using the Name = syntax to read it from configuration -
                //see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer("Server=MSI-LAPTOP;Database=Shares_DB;Trusted_Connection=True;");
                string strConnString = this.Configuration.GetConnectionString("Azure_DB");
                optionsBuilder.UseSqlServer(strConnString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Share>(entity =>
            {
                entity.HasKey(e => e.Symbol);

                entity.Property(e => e.Symbol)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Count)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.MktValue)
                    .HasColumnType("money")
                    .HasColumnName("Mkt_Value");

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Total).HasColumnType("money");

                entity.Property(e => e.LastUpdateTime).IsUnicode(false);

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
