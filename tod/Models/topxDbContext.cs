using System;
using Microsoft.EntityFrameworkCore;
using tod.Models;

namespace tod.Models
{
    public class topxDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Commentary> Commentaries { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TopicReaction> TopicReactions { get; set; }
        public DbSet<CommentaryReaction> CommentaryReactions { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        //public DbSet<TagTopic> TagTopic { get; set; }
        //public DbSet<TagUser> TagUser { get; set; }

        public topxDbContext(DbContextOptions<topxDbContext> options)
            :base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1433; Database=topxDB;User=sa; Password=KAnITOWKA13");
        }

        public DbSet<tod.Models.ViewModels.RegisterModel> RegisterViewModel { get; set; }



    }
}
