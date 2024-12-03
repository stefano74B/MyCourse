using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyCourse.Models.ValueObjects;

namespace MyCourse.Models.Entities
{
    public partial class MyCourseDbContext : DbContext
    {
        public MyCourseDbContext()
        {
        }

        public MyCourseDbContext(DbContextOptions<MyCourseDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlite("Data Source=Data/MyCourse.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses"); //superfluo se la tabella si chiama come la proprietà che espone il DbSet
                entity.HasKey(course => course.Id); // superfluo se si chiama Id o CoursesId

                // in caso di più chiavi primarie
                // entity.HasKey(course => new { course.Id, course.Author });
                
                // mapping per gli owned types
                entity.OwnsOne(course => course.CurrentPrice, builder => {
                    builder.Property(money => money.Currency)
                           .HasConversion<string>() // avvisiamo che nel db è un campo text e deve fare una conversione da enum
                           .HasColumnName("CurrentPrice_Currency"); // questa è superflua perchè questa colonna seguono già la convenzione di nomi
                    builder.Property(money => money.Amount).HasColumnName("CurrentPrice_Amount"); // questa è superflua perchè questa colonna seguono già la convenzione di nomi
                });

                entity.OwnsOne(course => course.FullPrice, builder => {
                    builder.Property(money => money.Currency).HasConversion<string>(); // avvisiamo che nel db è un campo text e deve fare una conversione da enum
                });

                // mapping per le relazioni
                entity.HasMany(course => course.Lessons)
                      .WithOne(lesson => lesson.Course)
                      .HasForeignKey(lesson => lesson.CourseId); // Superflua se la proprietà si chiama CourseId


                #region Mapping generato in automatico dal tool di reverse engineering
                    
                /*
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasColumnType("TEXT (100)");

                entity.Property(e => e.CurrentPriceAmount)
                    .IsRequired()
                    .HasColumnName("CurrentPrice_Amount")
                    .HasColumnType("NUMERIC")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.CurrentPriceCurrency)
                    .IsRequired()
                    .HasColumnName("CurrentPrice_Currency")
                    .HasColumnType("TEXT (3)")
                    .HasDefaultValueSql("'EUR'");

                entity.Property(e => e.Description).HasColumnType("TEXT (10000)");

                entity.Property(e => e.Email).HasColumnType("TEXT (100)");

                entity.Property(e => e.FullPriceAmount)
                    .IsRequired()
                    .HasColumnName("FullPrice_Amount")
                    .HasColumnType("NUMERIC")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.FullPriceCurrency)
                    .IsRequired()
                    .HasColumnName("FullPrice_Currency")
                    .HasColumnType("TEXT (3)")
                    .HasDefaultValueSql("'EUR'");

                entity.Property(e => e.ImagePath).HasColumnType("TEXT (100)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("TEXT (100)");

                */
                #endregion

            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                // Questo mapping della relazione è alternativo a quello fatto sopra
                // in quanto basta evidenziarlo in una delle due tabelle relazionate
                // in questo caso si invertono 'One' e 'Many'
                entity.HasOne(lesson => lesson.Course)
                      .WithMany(course => course.Lessons) 

                #region Mapping generato in automatico dal tool di reverse engineering
                    
                // entity.Property(e => e.Id).ValueGeneratedNever();

                // entity.Property(e => e.Description).HasColumnType("TEXT (10000)");

                // entity.Property(e => e.Duration)
                //     .IsRequired()
                //     .HasColumnType("TEXT (8)")
                //     .HasDefaultValueSql("'00:00:00'");

                // entity.Property(e => e.Title)
                //     .IsRequired()
                //     .HasColumnType("TEXT (100)");

                // entity.HasOne(d => d.Course)
                //     .WithMany(p => p.Lessons)
                //     .HasForeignKey(d => d.CourseId);
                #endregion
            });
        }
    }
}
