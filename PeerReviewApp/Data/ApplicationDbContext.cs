using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeerReviewApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PeerReviewApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public DbSet<AssignmentTemplate> AssignmentTemplates { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<PeerReviewSubmission> PeerReviewSubmissions { get; set; }
        public DbSet<PeerReviewTemplate> PeerReviewTemplates { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<PeerReviewPartners> prPartners { get; set; }

        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Seed();
        }
    }
}