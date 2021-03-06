using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VOD.Common.Entities;

namespace VOD.Database.Contexts
{
    public class VODContext : IdentityDbContext<VODUser>
    {
        public VODContext(DbContextOptions<VODContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Download> Downloads { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<Video> Videos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedData(builder);

            // Composite key
            builder.Entity<UserCourse>().HasKey(uc => new { uc.UserId, uc.CourseId });
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;

            //// Composite key
            //builder.Entity<UserCourse>().HasKey(uc =>
            //    new { uc.UserId, uc.CourseId });
            //// Restrict cascading deletes
            //foreach (var relationship in builder.Model.GetEntityTypes()
            //    .SelectMany(e => e.GetForeignKeys()))
            //{
            //    relationship.DeleteBehavior = DeleteBehavior.Restrict;
            //}
        }

        private void SeedData(ModelBuilder builder)
        {
            #region Admin Credentials Properties

            var email = "a@b.c";
            var password = "Test123__";

            var user = new VODUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                EmailConfirmed = true
            };

            var passwordHasher = new PasswordHasher<VODUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, password);

            // Add user to database
            builder.Entity<VODUser>().HasData(user);

            #endregion Admin Credentials Properties

            #region Admin Roles and Claims

            var admin = "Admin";
            var role = new IdentityRole
            {
                Id = "1",
                Name = admin,
                NormalizedName = admin.ToUpper()
            };

            builder.Entity<IdentityRole>().HasData(role);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
                    {
                        RoleId = role.Id,
                        UserId = user.Id
                    });

            builder.Entity<IdentityUserClaim<string>>().HasData(new IdentityUserClaim<string>
            {
                Id = 1,
                ClaimType = admin,
                ClaimValue = "true",
                UserId = user.Id
            });

            builder.Entity<IdentityUserClaim<string>>().HasData(new IdentityUserClaim<string>
            {
                Id = 2,
                ClaimType = "VODUser",
                ClaimValue = "true",
                UserId = user.Id
            });

            #endregion Admin Roles and Claims
        }

        public void SeedMembershipData()
        {
            #region Lorem Ipsum - Dummy Data

            var description =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";

            #endregion Lorem Ipsum - Dummy Data

            #region Fetch a User

            var email = "a@b.c";
            var userId = string.Empty;

            if (Users.Any(r => r.Email.Equals(email)))
                userId = Users.First(r => r.Email.Equals(email)).Id;
            else
                return;

            #endregion Fetch a User

            #region Add Instructors if they don't already exist

            if (!Instructors.Any())
            {
                var instructors = new List<Instructor>
                {
                    new Instructor
                    {
                        Name = "John Doe",
                        Description = description.Substring(20, 50),
                        Thumbnail = "/images/Ice-Age-Scrat-icon.png"
                    },
                    new Instructor
                    {
                        Name = "Jane Doe",
                        Description = description.Substring(30, 40),
                        Thumbnail = "/images/Ice-Age-Scrat-icon.png"
                    }
                };
                Instructors.AddRange(instructors);
                SaveChanges();
            }

            if (Instructors.Count() < 2) return;

            #endregion Add Instructors if they don't already exist

            #region Add Courses if they don't already exist

            if (!Courses.Any())
            {
                var instructorId1 = Instructors.First().Id;
                var instructorId2 = Instructors.Skip(1).FirstOrDefault().Id;

                var courses = new List<Course>
                {
                    new Course
                    {
                        InstructorId = instructorId1,
                        Title = "Course 1",
                        Description = description,
                        ImageUrl = "/images/course1.jpg",
                        MarqueeImageUrl = "/images/laptop.jpg"
                    },
                    new Course
                    {
                        InstructorId = instructorId2,
                        Title = "Course 2",
                        Description = description,
                        ImageUrl = "/images/course2.jpg",
                        MarqueeImageUrl = "/images/laptop.jpg"
                    },
                    new Course
                    {
                        InstructorId = instructorId1,
                        Title = "Course 3",
                        Description = description,
                        ImageUrl = "/images/course3.jpg",
                        MarqueeImageUrl = "/images/laptop.jpg"
                    }
                };
                Courses.AddRange(courses);
                SaveChanges();
            }

            if (Courses.Count() < 3) return;

            #endregion Add Courses if they don't already exist

            #region Fetch Course ids if any courses exists

            var courseId1 = Courses.First().Id;
            var courseId2 = Courses.Skip(1).FirstOrDefault().Id;
            var courseId3 = Courses.Skip(2).FirstOrDefault().Id;

            #endregion Fetch Course ids if any courses exists

            #region Add UserCourses connections if they don't already exist

            if (!UserCourses.Any())
            {
                UserCourses.Add(new UserCourse { UserId = userId, CourseId = courseId1 });
                UserCourses.Add(new UserCourse { UserId = userId, CourseId = courseId2 });
                UserCourses.Add(new UserCourse { UserId = userId, CourseId = courseId3 });

                SaveChanges();
            }

            if (UserCourses.Count() < 3) return;

            #endregion Add UserCourses connections if they don't already exist

            #region Add Modules if they don't already exist

            if (!Modules.Any())
            {
                var modules = new List<Module>
                {
                    new Module {Course = Find<Course>(courseId1), Title = "Modeule 1"},
                    new Module {Course = Find<Course>(courseId1), Title = "Modeule 2"},
                    new Module {Course = Find<Course>(courseId2), Title = "Modeule 3"}
                };
                Modules.AddRange(modules);
                SaveChanges();
            }

            if (Modules.Count() < 3) return;

            #endregion Add Modules if they don't already exist

            #region Fetch Module ids if any modules exist

            var module1 = Modules.First();
            var module2 = Modules.Skip(1).FirstOrDefault();
            var module3 = Modules.Skip(2).FirstOrDefault();

            #endregion Fetch Module ids if any modules exist

            #region Add Videos if they don't already exist

            if (!Videos.Any())
            {
                var videos = new List<Video>
                {
                    new Video
                    {
                        ModuleId = module1.Id, CourseId = module1.CourseId,
                        Title = "Video 1 Title",
                        Description = description.Substring(1, 35),
                        Duration = 50, Thumbnail = "/images/video1.jpg",
                        Url = "https://www.youtube.com/embed/BJFyzpBcaCY"
                    },
                    new Video
                    {
                        ModuleId = module1.Id, CourseId = module1.CourseId,
                        Title = "Video 2 Title",
                        Description = description.Substring(5, 35),
                        Duration = 45, Thumbnail = "/images/video2.jpg",
                        Url = "https://www.youtube.com/embed/BJFyzpBcaCY"
                    },
                    new Video
                    {
                        ModuleId = module1.Id, CourseId = module1.CourseId,
                        Title = "Video 3 Title",
                        Description = description.Substring(10, 35),
                        Duration = 41, Thumbnail = "/images/video3.jpg",
                        Url = "https://www.youtube.com/watch?v=qv6ZflueASY"
                    },
                    new Video
                    {
                        ModuleId = module3.Id, CourseId = module3.CourseId,
                        Title = "Video 4 Title",
                        Description = description.Substring(15, 35),
                        Duration = 41, Thumbnail = "/images/video4.jpg",
                        Url = "https://www.youtube.com/embed/BJFyzpBcaCY"
                    },
                    new Video
                    {
                        ModuleId = module2.Id, CourseId = module2.CourseId,
                        Title = "Video 5 Title",
                        Description = description.Substring(20, 35),
                        Duration = 42, Thumbnail = "/images/video5.jpg",
                        Url = "https://www.youtube.com/embed/BJFyzpBcaCY"
                    }
                };
                Videos.AddRange(videos);
                SaveChanges();
            }

            #endregion Add Videos if they don't already exist

            #region Add Downloads if they don't already exist

            if (!Downloads.Any())
            {
                var downloads = new List<Download>
                {
                    new Download
                    {
                        ModuleId = module1.Id, CourseId = module1.CourseId,
                        Title = "ADO.NET 1 (PDF)", Url = "https://some-url"
                    },

                    new Download
                    {
                        ModuleId = module1.Id, CourseId = module1.CourseId,
                        Title = "ADO.NET 2 (PDF)", Url = "https://some-url"
                    },

                    new Download
                    {
                        ModuleId = module3.Id, CourseId = module3.CourseId,
                        Title = "ADO.NET 1 (PDF)", Url = "https://some-url"
                    }
                };

                Downloads.AddRange(downloads);
                SaveChanges();
            }

            #endregion Add Downloads if they don't already exist
        }
    }
}