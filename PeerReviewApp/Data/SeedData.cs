using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PeerReviewApp.Models;
using Microsoft.AspNetCore.Identity;

namespace PeerReviewApp.Data
{
    public static class SeedData
    {
        public static async Task SeedDefaultUsers(IServiceProvider serviceProvider)
        {
            const int INSTITUTION_ID = 1;

            //Seed Admin User
            const string adminId = "ADMIN1";
            const string adminUserName = "admin";
            const string adminFirstName = "Default";
            const string adminLastName = "Admin";
            const string adminEmail = "admin@admin.com";
            const string adminPassword = "admin123!";
            const string adminRoleName = "Admin";

            //Seed Default Instructor
            const string instructorId = "INSTRUCTOR1";
            const string instructorUserName = "seedInstructor";
            const string instructorFirstName = "Default";
            const string instructorLastName = "Instructor";
            const string instructorEmail = "instructor@admin.com";
            const string instructorPassword = "instructor123!";
            const string instructorRoleName = "Instructor";

            //Seed Default Student
            const string studentId = "STUDENT1";
            const string studentUserName = "seedStudent";
            const string studentFirstName = "Default";
            const string studentLastName = "Student";
            const string studentEmail = "student@admin.com";
            const string studentPassword = "student123!";
            const string studentRoleName = "Student";

            UserManager<AppUser> userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            //Create Admin User
            if (await roleManager.FindByNameAsync(adminRoleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new AppUser()
                {
                    Id = adminId,
                    UserName = adminUserName,
                    FirstName = adminFirstName,
                    LastName = adminLastName,
                    Email = adminEmail,
                    InstitutionId = INSTITUTION_ID
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, adminRoleName);
                }
            }
            //Create Default Instructor
            if (await roleManager.FindByNameAsync(instructorRoleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(instructorRoleName));
            }

            if (await userManager.FindByEmailAsync(instructorEmail) == null)
            {
                var instructor = new AppUser()
                {
                    Id = instructorId,
                    UserName = instructorUserName,
                    FirstName = instructorFirstName,
                    LastName = instructorLastName,
                    Email = instructorEmail,
                    InstitutionId = INSTITUTION_ID
                };

                var result = await userManager.CreateAsync(instructor, instructorPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(instructor, instructorRoleName);
                }
            }
            //Create Default Student
            if (await roleManager.FindByNameAsync(studentRoleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(studentRoleName));
            }

            if (await userManager.FindByEmailAsync(studentEmail) == null)
            {
                var student = new AppUser()
                {
                    Id = studentId,
                    UserName = studentUserName,
                    FirstName = studentFirstName,
                    LastName = studentLastName,
                    Email = studentEmail,
                    InstitutionId = INSTITUTION_ID
                };

                var result = await userManager.CreateAsync(student, studentPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(student, studentRoleName);
                }
            }
        }

        private const string ID1 = "A";
        private const string ID2 = "B";
        private const string ID3 = "C";
        private const string instructorId = "INSTRUCTOR1A";
        private const string studentId = "STUDENT1A";


        public static void Seed(this ModelBuilder modelBuilder)
        {
            var password1 = new PasswordHasher<AppUser>();
            

            var user1 = new AppUser()
            {
                Id = ID1,
                FirstName = "Jared",
                LastName = "Turner",
                Password = "userPassword",
                Email = "testemail1@email.com",
                InstitutionId = 1,
                UserName = "JaredT"

   
            };
            var user2 = new AppUser()
            {
                Id = ID2,
                FirstName = "Steve",
                LastName = "Jones",
                Password = "userPassword123!",
                Email = "testemail2@email.com",
                InstitutionId = 1,
                UserName = "SteveJ"
            };
            var user3 = new AppUser()
            {
                Id = ID3,
                FirstName = "Russel",
                LastName = "Nelson",
                Password = "userPassword",
                Email = "testemail3@email.com",
                InstitutionId = 1,
                UserName = "RusselN"
            };
            var user4 = new AppUser()
            {
                Id = instructorId,
                FirstName = "Deedeck",
                LastName = "Needles",
                Password = "userPassword",
                Email = "testemail3@email.com",
                InstitutionId = 1,
                UserName = "Deedeck"
            };
            var user5 = new AppUser()
            {
                Id = studentId,
                FirstName = "Tyler",
                LastName = "Cyler",
                Password = "userPassword",
                Email = "testemail3@email.com",
                InstitutionId = 1,
                UserName = "TylerC"
            };

            modelBuilder.Entity<AppUser>().HasData(user1,user2, user3, user4, user5);

            Institution institution1 = new Institution()
            {
                InstitutionId = 1,
                InstitutionName = "Lane Community College",
                InstructorCode = 1111
            };

            Institution institution2 = new Institution()
            {
                InstitutionId = 2,
                InstitutionName = "University of Oregon",
                InstructorCode = 1111
            };
            Institution institution3 = new Institution()
            {
                InstitutionId = 3,
                InstitutionName = "Oregon State University",
                InstructorCode = 1111
            };
            Institution institution4 = new Institution()
            {
                InstitutionId = 4,
                InstitutionName = "Willamette University",
                InstructorCode = 1111
            };
            Institution institution5 = new Institution()
            {
                InstitutionId = 5,
                InstitutionName = "Linn-Benton Community College",
                InstructorCode = 1111
            };

            modelBuilder.Entity<Institution>().HasData(institution1, institution2, institution3, institution4, institution5);

            Department department1 = new Department()
            {
                DepartmentId = 1,
                DepartmentName = "Computer Science",
                InstitutionId = institution1.InstitutionId
            };
            Department department2 = new Department()
            {
                DepartmentId = 2,
                DepartmentName = "Mathematics",
                InstitutionId = institution1.InstitutionId
            };
            Department department3 = new Department()
            {
                DepartmentId = 3,
                DepartmentName = "English",
                InstitutionId = institution1.InstitutionId
            };
            modelBuilder.Entity<Department>().HasData(department1, department2, department3);

            Course course1 = new Course()
            {
                CourseId = 1,
                CourseName = "CS 297 - Programming Capstone",
                DepartmentId = department1.DepartmentId
            };
            Course course2 = new Course()
            {
                CourseId = 2,
                CourseName = "MATH 203 - Integral Calculus",
                DepartmentId = department2.DepartmentId
            };
            Course course3 = new Course()
            {
                CourseId = 3,
                CourseName = "CS 275 - Basic Database SQL",
                DepartmentId = department1.DepartmentId
            };
            modelBuilder.Entity<Course>().HasData(course1, course2, course3);

            Term term1 = new Term()
            {
                TermId = 1,
                TermName = "Spring 2022",
                InstitutionId = institution1.InstitutionId,
                TermBeginDate = DateTime.Parse("03/28/2022"),
                TermEndDate = DateTime.Parse("06/11/2022")
            };

            Term term2 = new Term()
            {
                TermId = 2,
                TermName = "Summer 2022",
                InstitutionId = institution1.InstitutionId,
                TermBeginDate = DateTime.Parse("06/20/2022"),
                TermEndDate = DateTime.Parse("10/10/2022")
            };
            modelBuilder.Entity<Term>().HasData(term1, term2);

            Class class1 = new Class()
            {
                ClassId = 1,
                ClassName = course1.CourseName + " - " + term1.TermName,
                CourseId = course1.CourseId,
                TermId = term1.TermId,
                InstructorId = instructorId
            };
            Class class2 = new Class()
            {
                ClassId = 2,
                ClassName = course2.CourseName + " - " + term1.TermName,
                CourseId = course2.CourseId,
                TermId = term1.TermId,
                InstructorId = instructorId
            };
            Class class3 = new Class()
            {
                ClassId = 3,
                ClassName = course3.CourseName + " - " + term1.TermName,
                CourseId = course3.CourseId,
                TermId = term1.TermId,
                InstructorId = instructorId
            };
            Class class4 = new Class()
            {
                ClassId = 4,
                ClassName = course3.CourseName + " - " + term2.TermName,
                CourseId = course3.CourseId,
                TermId = term2.TermId,
                InstructorId = instructorId
            };
            Class class5 = new Class()
            {
                ClassId = 5,
                ClassName = course2.CourseName + " - " + term2.TermName,
                CourseId = course2.CourseId,
                TermId = term2.TermId,
                InstructorId = instructorId
            };
            modelBuilder.Entity<Class>().HasData(class1, class2, class3, class4, class5);

            Assignment assignment1 = new Assignment()
            {
                AssignmentId = 1,
                AssignmentName = "Lab 1",
                AssignmentInstructions = "",
                AssignmentDueDate = DateTime.Now,
                PeerReviewDueDate = DateTime.Now,
                ClassId = class1.ClassId
            };
            Assignment assignment2 = new Assignment()
            {
                AssignmentId = 2,
                AssignmentName = "Lab 2",
                AssignmentInstructions = "",
                AssignmentDueDate = DateTime.Now,
                PeerReviewDueDate = DateTime.Now,
                ClassId = class1.ClassId
            };
            Assignment assignment3 = new Assignment()
            {
                AssignmentId = 3,
                AssignmentName = "Lab 3",
                AssignmentInstructions = "",
                AssignmentDueDate = DateTime.Now,
                PeerReviewDueDate = DateTime.Now,
                ClassId = class1.ClassId
            };
            modelBuilder.Entity<Assignment>().HasData(assignment1, assignment2, assignment3);

            Enrollment enrollment1 = new Enrollment()
            {
                EnrollmentId = 1,
                StudentId = studentId,
                ClassId = class1.ClassId
            };
            Enrollment enrollment2 = new Enrollment()
            {
                EnrollmentId = 2,
                StudentId = studentId,
                ClassId = class2.ClassId
            };
            Enrollment enrollment3 = new Enrollment()
            {
                EnrollmentId = 3,
                StudentId = studentId,
                ClassId = class3.ClassId
            };
            Enrollment enrollment4 = new Enrollment()
            {
                EnrollmentId = 4,
                StudentId = studentId,
                ClassId = class4.ClassId
            };
            modelBuilder.Entity<Enrollment>().HasData(enrollment1, enrollment2, enrollment3, enrollment4);
        }
    }
}

