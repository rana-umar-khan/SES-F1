using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SES_F1.Models;
using System.Linq;
using System;

[assembly: OwinStartupAttribute(typeof(SES_F1.Startup))]
namespace SES_F1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            addRoles();
        }
        public void addRoles()
        {
           var context = new ApplicationDbContext();
            var db = new SESEntities();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var teacherRole = "teacher";
            var adminRole = "admin";
            if (!roleManager.RoleExists(teacherRole))
            {
                var role = new IdentityRole
                {
                    Name = teacherRole
                };
                var result = roleManager.Create(role);
                if (!result.Succeeded)
                {
                    throw new System.Exception("Could not create the teacher role");
                }
                var applicationUser = new ApplicationUser
                {
                    UserName = "umar",
                    Email = "umarkhan1@gmail.com"
                };
                result = userManager.Create(applicationUser, "Updating@1234");
                if (!result.Succeeded)
                {
                    //throw new System.Exception("Could not create applicaiton user for the teacher");
                }
                var teacher = new Teacher
                {
                    FirstName = "Aslam",
                    LastName = "Ali",
                    FatherName = "Shaukat Ali",
                    DOB = new DateTime(1990, 10, 4),
                    JoiningDate = new DateTime(2017, 8, 15),
                    CNIC = 3530212345675,
                    PhoneNumber=03042672145,
                    Gender="Male",
                    Qualification="B.Sc",
                    
                    AspNetUser = db.AspNetUsers.Find(applicationUser.Id)
                };
                using (var transaction = db.Database.BeginTransaction())
                {
                    

                    db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Teacher] ON");

                    db.Teachers.Add(teacher);
                    db.SaveChanges();

                    db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Teacher] OFF");

                    transaction.Commit();
                }
                result = userManager.AddToRole(applicationUser.Id, teacherRole);
                if (!result.Succeeded)
                {
                    throw new System.Exception("Could not add teacher to the teacher role");
                }
            }

            if (!roleManager.RoleExists(adminRole))
            {
                var role = new IdentityRole
                {
                    Name = adminRole
                };
                roleManager.Create(role);
                var user = new ApplicationUser
                {
                    UserName = "Principal",
                    Email = "umerkhan11@yahoo.com"
                };
                string userPwd = "Password.12";
                var status=userManager.Create(user, userPwd);
                if (!status.Succeeded)
                {
                    throw new System.Exception("Admin User can not be added");
                }
                Admin ad = new Admin
                {
                    Name = "Abdullah",
                    AspNetUser = db.AspNetUsers.Find(user.Id)
                };
                db.Admins.Add(ad);
                db.SaveChanges();
                var result = userManager.AddToRole(user.Id, adminRole);
                if (!result.Succeeded)
                {
                    throw new System.Exception("Admin user can not be linked to admin role");
                }
            }
        }

        public void addteacher()
        {
            var context = new ApplicationDbContext();
            var db = new SESEntities();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var teacherRole = "teacher";
            var role = new IdentityRole
            {
                Name = teacherRole
            };
            var applicationUser = new ApplicationUser
            {
                UserName = "umar",
                Email = "umarkhan1@gmail.com"
            };
            var result = userManager.Create(applicationUser, "Password.12");
            if (!result.Succeeded)
            {
                throw new System.Exception("Could not create applicaiton user for the teacher");
            }
            var teacher = new Teacher
            {
                FirstName = "Aslam",
                LastName = "Ali",
                FatherName = "Shaukat Ali",
                DOB = new DateTime(1990, 10, 4),
                JoiningDate = new DateTime(2017, 8, 15),
                CNIC = 3530212345675,
                PhoneNumber = 03042672145,
                Gender = "Male",
                Qualification = "B.Sc",
                Status = true,
                AspNetUser = db.AspNetUsers.Find(applicationUser.Id)
            };
            using (var transaction = db.Database.BeginTransaction())
            {


                db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Teacher] ON");

                db.Teachers.Add(teacher);
                db.SaveChanges();

                db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Teacher] OFF");

                transaction.Commit();
            }
            result = userManager.AddToRole(applicationUser.Id, teacherRole);
            if (!result.Succeeded)
            {
                throw new System.Exception("Could not add teacher to the teacher role");
            }
        }
    }
}
