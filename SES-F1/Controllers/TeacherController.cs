using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SES_F1.Controllers
{
    [Authorize(Roles = "teacher") ]
    public class TeacherController : Controller
    {
        SESEntities db = new SESEntities();


        // GET: Teacher
        
        public ActionResult Index()
        {
            AspNetUser u = db.AspNetUsers.Where(a=>a.UserName== User.Identity.Name).FirstOrDefault();
            Teacher t= u.Teachers.FirstOrDefault();
            return View(t);
        }
        
    }
}