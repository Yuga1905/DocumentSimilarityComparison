using Azure.Core;
using DocumentSimilarityComparison.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DocumentSimilarityComparison
{
    public class DocumentDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; } = null!;
        public DbSet<Job_Description_Model> JobDescriptions { get; set; } = null!;
        public DbSet<Requestor_Model> Requestors { get; set; } = null!;
        public DbSet<Resume_Details_Model> ResumeDetails { get; set; } = null!;

        public DocumentDbContext(DbContextOptions<DocumentDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<UserModel>()
            .HasIndex(u => u.Email)
                .IsUnique();

            // Requestor
            modelBuilder.Entity<Requestor_Model>()
                .HasOne(r => r.JobDescription)
                .WithMany(j => j.Requestors)
                .HasForeignKey(r => r.JdId)
                .OnDelete(DeleteBehavior.Cascade);

            // ResumeDetail
            modelBuilder.Entity<Resume_Details_Model>()
                .HasOne(r => r.JobDescription)
                .WithMany(j => j.ResumeDetails)
                .HasForeignKey(r => r.JdId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
