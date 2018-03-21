using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SES_F1.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher
        public ActionResult Index()
        {
            SESEntities a=new SESEntities();
            Teacher t=(a.AspNetUsers.Find("c1b01a11-bcba-404f-8fa9-6037606ed252")).Teacher;
            Teacher t1 =(Teacher) t;
            return View(t);
        }
    }
}