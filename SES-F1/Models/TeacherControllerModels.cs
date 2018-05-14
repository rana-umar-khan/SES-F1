using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace SES_F1.Models
{
    public class MarkAttendanceModel
    {
        public List<string> RollNumber { get; set; }
        public List<string> Name { get; set; }
        public List<bool> TodayStatus { get; set; }
        public List<bool> YesterdayStatus { get; set; }
        public DateTime date { get; set; }
        public DateTime yDate { get; set; }


        SESEntities db = new SESEntities();

        public MarkAttendanceModel(int classID,DateTime onDate)
        {
            date = onDate;
            yDate = date.AddDays(-1);
            Class c=db.Classes.Find(classID);
            List<Student> st_List = c.Students.ToList();
            bool markedToday = isAttendanceMarked(classID, date);
            bool markedYesterday = isAttendanceMarked(classID,yDate );
            foreach (Student s in st_List)
            {
                RollNumber.Add(s.RollNumber);
                Name.Add(s.FirstName + s.LastName);
                if (markedToday)
                {
                    TodayStatus.Add(s.Attendances.Where(d => d.Date == date).FirstOrDefault().Status);
                }
                else
                {
                    TodayStatus.Add(false);
                }
                if (markedYesterday)
                {
                    YesterdayStatus.Add(s.Attendances.Where(d => d.Date == yDate).FirstOrDefault().Status);
                }
                else
                {
                    YesterdayStatus.Add(false);
                }
            }
        }

        private bool isAttendanceMarked(int classID, DateTime onDate)
        {
            if ((db.Classes.Find(classID).Students.FirstOrDefault().Attendances.Where(d => d.Date == onDate)).Count() > 0)
                return true;
            return false;
        }
    }
}