using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SES_F1.Models
{
    public class AddTeacherModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Date of Birth")]
        public System.DateTime DOB { get; set; }
        [Required]
        [Display(Name = "CNIC")]
        public decimal CNIC { get; set; }
        [Required]
        [Display(Name = "Phone")]
        public decimal PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Joining Date")]
        public System.DateTime JoiningDate { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Required]
        [Display(Name = "Father Name")]
        public string FatherName { get; set; }
        [Required]
        [Display(Name = "Qualification")]
        public string Qualification { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        public HttpPostedFileBase photo;
    }

    public class ChallanModel
    {
        public Student s;
        public int monthlyFee;
        public int Arears;
        public int admissionFee;
        public int AnnualCharges;
        public ChallanModel(string rollNumber, bool adm, bool annual)
        {
            s = new SESEntities().Students.Find(rollNumber);
            monthlyFee = s.Class.MonthlyFee;
            if (adm && annual)
            {
                admissionFee = s.Class.AdmissionFee;
                AnnualCharges = s.Class.AnnualCharges;
            }
            else if(annual)
            {
                AnnualCharges = s.Class.AnnualCharges;
            }
        }
    }
}