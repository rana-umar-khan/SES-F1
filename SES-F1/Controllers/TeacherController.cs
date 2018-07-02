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
        //int teacherID;
        Teacher teacherObj;
        [HttpGet]
        public ActionResult ErrorPage()
        {
            return View();
        }

        // GET: Teacher
        public ActionResult Index()
        {
            AspNetUser u = db.AspNetUsers.Where(a => a.UserName == User.Identity.Name).FirstOrDefault();
            teacherObj = u.Teachers.FirstOrDefault();

            IEnumerable<Class> classes = teacherObj.Classes;
            return View(classes);
        }
        
        public ActionResult ViewStudents(int classID)
        {
            IEnumerable<Student> stdList = db.Students.Where(m => m.ClassID == classID);
            return View(stdList);
        }

        public ActionResult Attendance(int? classID)
        {
            if(classID==null)
            {
                ViewBag.Message = "You are not the class Incharge";
                return View("ErrorPage");
            }
            ViewBag.ClassID = classID;
            AspNetUser u = db.AspNetUsers.Find(User.Identity.GetUserId());
            Teacher t = u.Teachers.FirstOrDefault();
            if (t.ClassIncharge == null || t.ClassIncharge != classID)
            {
                ViewBag.Message = "You are not the class Incharge";
                return View("ErrorPage");
            }
            else
            {
                return View();
            }
        }
        //[HttpPost]
        public ActionResult getAttendanceSheet(int? classID, DateTime? AttendanceDate)
        {
            if (classID == null || AttendanceDate == null)
            {
                return RedirectToAction("Index");
            }
            //check if attendance is being done within three days of today
            
            if (chkdte(AttendanceDate.Value))
            {
                AspNetUser u = db.AspNetUsers.Find(User.Identity.GetUserId());
                Teacher t = u.Teachers.FirstOrDefault();
                if (t.ClassIncharge == null || t.ClassIncharge != classID)
                {
                    ViewBag.Message = "You are not the class Incharge";
                    return RedirectToAction("index", new { message = "You are not the class Incharge" });
                }
                else
                {
                    IEnumerable<Student> st_List = t.Classes.Where(c => c.ClassID == classID).FirstOrDefault().Students.AsEnumerable();
                    MarkAttendanceModel sheat = new MarkAttendanceModel((int)classID, (DateTime)AttendanceDate);
                    return View(sheat);
                }
            }
            else
            {
                ViewBag.Message="Invalid Date";
                return RedirectToAction( "Attendance",new { classID = classID });
            }
        }

        [HttpGet]
        public ActionResult checkDate(DateTime date)
        {
            Object data = new {
                valid = false
            };
            if(chkdte(date))
            {
                Object data1 = new
                {
                    valid = true
                };
                return Json(data1, JsonRequestBehavior.AllowGet);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private bool chkdte(DateTime d)
        {
            DateTime today = DateTime.Today;
            if (d <= today && (today.AddDays(-4) < d))
            {
                return true;
            }
            return false;
        }

        //[HttpPost]
        public string SaveAttendance(DateTime date, bool sms,int classID, bool[] statuslist, string[] RollNumbers)
        {
            for (int i = 0; i < statuslist.Length; i++)
            {
                Attendance at = new Attendance()
                {
                    Status = statuslist[i],
                    RollNumber = RollNumbers[i],
                    Date = date
                };
                if ((db.Attendances.Where(m => m.RollNumber == at.RollNumber && m.Date == date)).FirstOrDefault() != null)
                {
                    (db.Attendances.Where(m => m.RollNumber == at.RollNumber && m.Date == date)).FirstOrDefault().Status = statuslist[i];
                }
                else
                {
                    db.Attendances.Add(at);
                }
            }
            db.SaveChanges();
            return "Saved Attendance";
        }

        //RESULTS
        public ActionResult ResultSheets(int classID)
        {
            IEnumerable<Result> results = db.Results.Where(i => i.Exam.ClassID == classID);
            return View(results);
        }
        
        public ActionResult AddResult(int? resultID)
        {
            if (resultID == null)
            {

            }
            Result r = db.Results.Find(resultID);
            return View(r);
        }

        [HttpPost]
        public ActionResult AddResultSheet(int ExamID, int subID,int classID)
        {
            ResultSheetModel rsm = new ResultSheetModel(classID,subID,ExamID);
            return View(rsm);
        }
        [HttpPost]
        public ActionResult SaveResultSheet(int classID,int subID,int examID, string[] RollNumbers, int[] markslist)
        {
            try
            {
                for (int i = 0; i < RollNumbers.Length; i++)
                {
                    string r = RollNumbers[i];
                    ResultSheet rs1 = db.ResultSheets.Where(m => m.Subject == subID && m.RollNumber == r && m.Results.FirstOrDefault().ExamID == examID).FirstOrDefault();
                    if (rs1 == null)
                    {
                        ResultSheet r123 = new ResultSheet
                        {
                            MarksObtained = markslist[i],
                            RollNumber = RollNumbers[i],
                            Subject = subID
                        };
                        db.Results.Where(m => m.ExamID == examID).FirstOrDefault().ResultSheets.Add(r123);
                        //db.ResultSheets.Add(r123);
                    }
                    else
                    {
                        db.ResultSheets.Where(m => m.Subject == subID && m.RollNumber == r && m.Results.FirstOrDefault().ExamID == examID).FirstOrDefault().MarksObtained =markslist[i];
                    }
                }
                db.SaveChanges();
                int rID = db.Results.Where(m => m.ExamID == examID).First().Id;
                return RedirectToAction("AddResult",new { resultID = rID });
            } catch(Exception e)
            {
                int rID = db.Results.Where(m => m.ExamID == examID).First().Id;
                return RedirectToAction("AddResult", new { resultID = rID });
            }
        }
    }
}