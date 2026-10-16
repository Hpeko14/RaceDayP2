using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RaceDayAPI.Data;

#nullable disable

namespace RaceDayAPI.Migrations
{
    [DbContext(typeof(RaceDayContext))]
    partial class RaceDayContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            modelBuilder.Entity("RaceDayAPI.Models.Category", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                b.Property<string>("Name").IsRequired().HasColumnType("nvarchar(max)");
                b.HasKey("Id");
                b.ToTable("Categories");
            });

            modelBuilder.Entity("RaceDayAPI.Models.Event", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                b.Property<DateTime>("Date").HasColumnType("datetime2");
                b.Property<string>("Location").IsRequired().HasColumnType("nvarchar(max)");
                b.Property<string>("Title").IsRequired().HasColumnType("nvarchar(max)");
                b.HasKey("Id");
                b.ToTable("Events");
            });

            modelBuilder.Entity("RaceDayAPI.Models.User", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                b.Property<string>("Email").IsRequired().HasColumnType("nvarchar(max)");
                b.Property<string>("Name").IsRequired().HasColumnType("nvarchar(max)");
                b.Property<string>("Password").IsRequired().HasColumnType("nvarchar(max)");
                b.Property<string>("Role").IsRequired().HasColumnType("nvarchar(max)");
                b.HasKey("Id");
                b.ToTable("Users");
            });

            modelBuilder.Entity("RaceDayAPI.Models.Enrollment", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                b.Property<int>("CategoryId").HasColumnType("int");
                b.Property<int>("EventId").HasColumnType("int");
                b.Property<DateTime?>("RegistrationDate").HasColumnType("datetime2");
                b.Property<int>("UserId").HasColumnType("int");
                b.HasKey("Id");
                b.HasIndex("EventId");
                b.HasIndex("UserId");
                b.HasIndex("CategoryId");
                b.ToTable("Enrollments");
            });

            modelBuilder.Entity("RaceDayAPI.Models.Result", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                b.Property<int>("EnrollmentId").HasColumnType("int");
                b.Property<TimeSpan>("FinishTime").HasColumnType("time");
                b.Property<int>("Position").HasColumnType("int");
                b.HasKey("Id");
                b.HasIndex("EnrollmentId");
                b.ToTable("Results");
            });

            modelBuilder.Entity("RaceDayAPI.Models.Enrollment", b =>
            {
                b.HasOne("RaceDayAPI.Models.Event")
                    .WithMany()
                    .HasForeignKey("EventId")
                    .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("RaceDayAPI.Models.User")
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("RaceDayAPI.Models.Category")
                    .WithMany()
                    .HasForeignKey("CategoryId")
                    .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("RaceDayAPI.Models.Result", b =>
            {
                b.HasOne("RaceDayAPI.Models.Enrollment")
                    .WithMany()
                    .HasForeignKey("EnrollmentId")
                    .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade)
                    .IsRequired();
            });
        }
    }
}
