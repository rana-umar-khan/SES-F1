using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace SES_F1.Models
{
    public class MarkAttendanceModel
    {
        public List<string> RollNumber{ get; set; }
        public List<string> Name { get; set; }
        public List<bool> TodayStatus { get; set; }
        public List<bool> YesterdayStatus { get; set; }
        public DateTime date { get; set; }
        public DateTime yDate { get; set; }
        public SESEntities Db { get => db; set => db = value; }
        public int ClassID { get; set; }

        SESEntities db = new SESEntities();

        public MarkAttendanceModel(int classID,DateTime onDate)
        {
            ClassID = classID;
            RollNumber = new List<string>();
            Name = new List<string>();
            TodayStatus = new List<bool>();
            YesterdayStatus = new List<bool>();
            date = onDate;
            yDate = date.AddDays(-1);
            Class c=Db.Classes.Find(classID);
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
            if ((Db.Classes.Find(classID).Students.FirstOrDefault().Attendances.Where(d => d.Date == onDate)).Count() > 0)
                return true;
            return false;
        }
    }

    public class ResultSheetModel
    {
        public List<string> students;
        public List<string> studentsName;
        public int classID;
        public int subjectID;
        public int examID;
        public List<int> markslist;
        public ResultSheetModel(int cID,int subID,int eID)
        {
            SESEntities s = new SESEntities();
            classID = cID;
            examID = eID;
            subjectID = subID;
            List<Student> st = s.Students.Where(m => m.ClassID == classID).ToList();
            students = new List<string>(st.Count);
            studentsName = new List<string>(st.Count);
            markslist = new List<int>(st.Count);

            foreach (var item in st)
            {
                students.Add(item.RollNumber);
                studentsName.Add(item.FirstName + " " + item.LastName);
                markslist.Add(0);
            }
            List<ResultSheet> rs= (s.Results.Where(m => m.ExamID == examID).First().ResultSheets.ToList()).Where(m => m.Subject == subjectID).ToList();
            if (rs.Count > 0)
            {
                foreach (var item in rs)
                {
                    string roll = item.RollNumber;
                    int mark = item.MarksObtained;
                    int ind = students.IndexOf(roll);
                    markslist.Insert(ind, mark);
                }
            }
        }
    }
}