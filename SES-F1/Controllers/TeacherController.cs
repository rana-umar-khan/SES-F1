using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SES_F1.Models;

namespace SES_F1.Controllers
{
    [Authorize(Roles = "teacher")]
    public class TeacherController : Controller
    {
        SESEntities db = new SESEntities();



        // GET: Teacher

        public ActionResult Index()
        {
            AspNetUser u = db.AspNetUsers.Where(a => a.UserName == User.Identity.Name).FirstOrDefault();
            Teacher t = u.Teachers.FirstOrDefault();
            IEnumerable<Class> classes = t.Classes;
            return View(classes);
        }


        public ActionResult Attendance(int classID)
        {
            ViewBag.ClassID = classID;
            AspNetUser u = db.AspNetUsers.Find(User.Identity.GetUserId());
            Teacher t = u.Teachers.FirstOrDefault();
            if (t.ClassIncharge == null || t.ClassIncharge != classID)
            {
                ViewBag.Message = "You are not the class Incharge";
                return RedirectToAction("index", new { message = "You are not the class Incharge" });
            }
            else
            {
                return View();
            }
        }

        public ActionResult MarkAttendance(int classID, DateTime date)
        {
            AspNetUser u = db.AspNetUsers.Find(User.Identity.GetUserId());
            Teacher t = u.Teachers.FirstOrDefault();
            if (t.ClassIncharge == null || t.ClassIncharge != classID )
            {
                ViewBag.Message = "You are not the class Incharge";
                return RedirectToAction("index",new {message= "You are not the class Incharge"});
            }
            else
            {
                IEnumerable<Student>st_List= t.Classes.Where(c => c.ClassID == classID).FirstOrDefault().Students.AsEnumerable();
                MarkAttendanceModel sheat=new MarkAttendanceModel(classID, date);
                return View(sheat);
            }
        }
        


        //RESULTS
        public ActionResult ResultSheets(int classID)
        {
            
            return View();
        }
    }
}