using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SES_F1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SES_F1.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        ApplicationDbContext context;
        private SESEntities db;
        public RoleManager<IdentityRole> roleManager;
        public UserManager<ApplicationUser> userManager;
        
        public AdminController()
        {
            context = new ApplicationDbContext();
            db = new SESEntities();
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        }


        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddStudent()
        {
            IEnumerable<Class> classes = db.Classes.ToList().AsEnumerable() ;
            
            List<SelectListItem> c = new List<SelectListItem>();
            
            foreach(Class cla in classes){
                c.Add(new SelectListItem
                {
                    Text = cla.ClassName,
                    Value = cla.ClassID.ToString()
                });
            }
            ViewBag.Classes = classes ;
            ViewBag.ReturnUrl = "ViewAllStudents";
            return View();
        }

        [HttpPost]
        public ActionResult AddStudent(Student s)
        {
            AdmissionRecord rec = new AdmissionRecord
            {
                AdmissionDate = DateTime.Today,
                PreviousSchool = "",
                RollNumber = s.RollNumber,
                Status = true
            };

            try
            {

                db.Students.Add(s);
                db.AdmissionRecords.Add(rec);
                db.SaveChanges();
            }catch(Exception e)
            {
                if(db.AdmissionRecords.Find(rec.RecordID)!=null)
                {
                    db.AdmissionRecords.Remove(db.AdmissionRecords.Find(rec.RecordID));
                }
                ViewBag.ErrorMessage ="Student couldn't be added\n"+ e.Data;
                return View("Error");
            }
            return RedirectToAction("ViewStudent", new { RollNumber = s.RollNumber });
        }
        //
        public ActionResult ViewAllStudent()
        {
            IEnumerable<Student> students= db.Students.AsEnumerable();
            
            return View(students);
        }

        public ActionResult ViewTeachers()
        {
            IEnumerable<Teacher> tList = db.Teachers.AsEnumerable();

            return View(tList);
        }

        [HttpGet]
        public ActionResult AddTeacher()
        {

            return View();
        }
        [HttpGet]
        public ActionResult TeacherDetails(int? TeacherId)
        {
            if (TeacherId == null)
            {
                return RedirectToAction("ViewTeachers");
            }
            Teacher t=db.Teachers.Find(TeacherId);
            return View(t);
        }
        [HttpPost]
        public ActionResult AddTeacher(Teacher t,HttpPostedFileBase photo)
        { 
            //Teacher t = new Teacher(ad);
            ApplicationUser user=new ApplicationUser();
            if (!ModelState.IsValid)
                return View("Error");
            var teacherRole = "teacher";
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
                
            }
            else
            {
                string username = generateusername(t.FirstName, t.LastName);
                user.UserName = username;
                
                var result = userManager.Create(user, "Password.12");
                if (!result.Succeeded)
                {
                    throw new System.Exception("Could not create applicaiton user for the teacher");
                }
                userManager.AddToRole(user.Id, teacherRole);
                t.AspNetUser = db.AspNetUsers.Find(user.Id);
            }
            try
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Teacher] ON");



                    t.Status = true;

                    db.Teachers.Add(t);
                    db.SaveChanges();

                    //db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Teacher] OFF");

                    transaction.Commit();
                }
            }catch(Exception e)
            {
                userManager.RemoveFromRole(user.Id, teacherRole);
                userManager.Delete(user);
                ViewBag.Message = "Error Occurred. Teacher not added"+e.Message;
            }
            SaveTeacherPhoto(photo, t.Teacherid);

            ViewBag.Message = "Teacher added successfully /n Username=" + t.AspNetUser.UserName+"/nPassword= Password.12";

            return RedirectToAction("ViewTeachers");
        }
        public bool SaveTeacherPhoto(HttpPostedFileBase file, int? TeacherId)
        {
            if (file != null)
            {
                Teacher t = db.Teachers.Find(TeacherId);
                string pic = System.IO.Path.GetFileName(file.FileName);

                string savePath = System.IO.Path.Combine(Server.MapPath("~/Content/Images"), t.AspNetUser.UserName + ".jpg");
                string path = "/Content/Images/" + t.AspNetUser.UserName + ".jpg";

                // file is uploaded
                file.SaveAs(savePath);
                t.photo = path;
                // save the image path path to the database or you can send imaxge 
                // directly to database
                // in-case if you want to store byte[] ie. for DB
                using (MemoryStream ms = new MemoryStream())
                {
                    //file.InputStream.CopyTo(ms);
                    //byte[] array = ms.GetBuffer();
                    //Image bigImage = new Bitmap(path);

                    //// Algorithm simplified for purpose of example.
                    //int height = bigImage.Height / 10;
                    //int width = bigImage.Width / 10;
                    // Now create a thumbnail
                    //using (Image smallImage = bigImage.GetThumbnailImage(120, 120, new Image.GetThumbnailImageAbort(Abort), IntPtr.Zero))
                    //{
                    //    //smallImage.Save("D:\\thumbnail.jpg", ImageFormat.Jpeg);
                    //    //MemoryStream tms = new MemoryStream();
                    //    //smallImage.Save(tms, ImageFormat.Jpeg);
                    //    //byte[] arr = tms.GetBuffer();
                    //    //t.thumb = arr;
                    //    //t.TeacherId = (int)TeacherId;
                    //    //t.Photo = array;
                    //}

                    db.SaveChanges();
                }

            }
            // after successfully uploading redirect the user
            return true;
        }
        [HttpGet]
        public ActionResult UploadTeacherPhoto()
        {
            IEnumerable<Teacher> teachers = db.Teachers.AsEnumerable();
            List<SelectListItem> c = new List<SelectListItem>();
            foreach (Teacher t in teachers)
            {
                c.Add(new SelectListItem
                {
                    Text = t.FirstName + " " + t.LastName,
                    Value = t.Teacherid.ToString()
                });
            }
            ViewBag.Teachers = c;
            return View();
        }

        [HttpPost]
        public ActionResult UploadTeacherPhoto(HttpPostedFileBase file,int? TeacherId)
        {
            if (file != null)
            {
                Teacher t = db.Teachers.Find(TeacherId);
                string pic = System.IO.Path.GetFileName(file.FileName);
                
                string savePath = System.IO.Path.Combine(Server.MapPath("~/Content/Images"),t.AspNetUser.UserName+".jpg");
                string path = "/Content/Images/"+ t.AspNetUser.UserName + ".jpg";

                // file is uploaded
                file.SaveAs(savePath);
                t.photo = path;
                // save the image path path to the database or you can send imaxge 
                // directly to database
                // in-case if you want to store byte[] ie. for DB
                using (MemoryStream ms = new MemoryStream())
                {
                    //file.InputStream.CopyTo(ms);
                    //byte[] array = ms.GetBuffer();
                    //Image bigImage = new Bitmap(path);

                    //// Algorithm simplified for purpose of example.
                    //int height = bigImage.Height / 10;
                    //int width = bigImage.Width / 10;
                    // Now create a thumbnail
                    //using (Image smallImage = bigImage.GetThumbnailImage(120, 120, new Image.GetThumbnailImageAbort(Abort), IntPtr.Zero))
                    //{
                    //    //smallImage.Save("D:\\thumbnail.jpg", ImageFormat.Jpeg);
                    //    //MemoryStream tms = new MemoryStream();
                    //    //smallImage.Save(tms, ImageFormat.Jpeg);
                    //    //byte[] arr = tms.GetBuffer();
                    //    //t.thumb = arr;
                    //    //t.TeacherId = (int)TeacherId;
                    //    //t.Photo = array;
                    //}
                    
                    db.SaveChanges();
                }
                 
            }
            // after successfully uploading redirect the user
            return RedirectToAction("ViewTeachers", "Admin");
        }

        [HttpGet]
        public ActionResult ViewStudent(string RollNumber)
        {
            Student s = db.Students.Find(RollNumber);
            if (s != null)
            {
                return View(s);
            }
            else
            {
                ViewBag.ErrorMessage = "Student not found";
                ViewBag.ReturnUrl = "~/ViewAllStudent/";
                return View();
            }
        }
        private string generateusername(string firstName, string lastName)
        {
            string uname = firstName + lastName;
            int n = db.AspNetUsers.Where(i => i.UserName == uname).Count();
            while (n > 0)
            {
                //implement some other functionality
                uname += n;
                n = db.AspNetUsers.Where(i => i.UserName == uname).Count();
            }
            return uname;
        }

        private void generateThumbnail(string photopath)
        {
            using (Image bigImage = new Bitmap(photopath))
            {
                // Algorithm simplified for purpose of example.
                int height = bigImage.Height / 10;
                int width = bigImage.Width / 10;

                // Now create a thumbnail
                using (Image smallImage = bigImage.GetThumbnailImage(120,120,new Image.GetThumbnailImageAbort(Abort), IntPtr.Zero))
                {
                    smallImage.Save("thumbnail.jpg", ImageFormat.Jpeg);
                    MemoryStream ms = new MemoryStream();
                    smallImage.Save(ms, ImageFormat.Jpeg);
                    byte[] arr = ms.GetBuffer();
                }
            }
        }

        private bool Abort()
        {
            throw new NotImplementedException();
        }




        //ASSIGN TEACHER A CLASS
        public ActionResult AssignClass()
        {
            IEnumerable<Teacher> teachers = db.Teachers.AsEnumerable();
           
            return View(teachers);
        }
        [HttpPost]
        public ActionResult AssignClass(string teacher,string Class)
        {
            IEnumerable<Teacher> teachers = db.Teachers.AsEnumerable();

            return View(teachers);
        }
    }
}