using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pajoohesh_V2.Model.Models.Main.AboutUs;
using Pajoohesh_V2.Model.Models.Main.Comment;
using Pajoohesh_V2.Model.Models.Main.ContactUs;
using Pajoohesh_V2.Model.Models.Main.Film;
using Pajoohesh_V2.Model.Models.Main.Subject;
using Pajoohesh_V2.Model.Models.Main.Tag;
using Pajoohesh_V2.Model.Models.Main.User;
using Pajoohesh_V2.Model.Models.Relationship;

namespace Pajoohesh_V2.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FilmTag>().HasKey(bc => new { bc.FilmId, bc.TagId });
            modelBuilder.Entity<FilmTag>().HasOne(bc => bc.Film).WithMany(b => b.FilmTags).HasForeignKey(bc => bc.FilmId); 
            modelBuilder.Entity<FilmTag>().HasOne(bc => bc.Tag).WithMany(c => c.FilmTags).HasForeignKey(bc => bc.TagId);

            modelBuilder.Entity<FilmLikeUser>().HasKey(bc => new { bc.FilmId, bc.UserId });
            modelBuilder.Entity<FilmLikeUser>().HasOne(bc => bc.Film).WithMany(b => b.LikeUsers).HasForeignKey(bc => bc.FilmId); 
            modelBuilder.Entity<FilmLikeUser>().HasOne(bc => bc.User).WithMany(c => c.LikeUsers).HasForeignKey(bc => bc.UserId);
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<FilmTag> FilmTags { get; set; }
        public DbSet<ContactUs> ContactUss { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<ForgetPassword> ForgetPasswords { get; set; }
        public DbSet<NewsLetter> NewsLetters { get; set; }
    }
}
