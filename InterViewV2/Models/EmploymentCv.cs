using System.ComponentModel.DataAnnotations.Schema;

namespace InterViewV2.Models
{
    public class EmploymentCv
    {
        public Guid Id { get; set; }//+
        public string AppID { get; set; }//+
        public string? FamilyName { get; set; }//+
        public string? FirstName { get; set; }//+
        public string? FatherName { get; set; } //+
        public string? Gender { get; set; }//+
        public DateTime DateOfBirth { get; set; }//+
        public string? Nationality { get; set; }//+
        public string? MaritalStatus { get; set; }//+
                                                  // public string? hmYesNo { get; set; }  //add
        public string? HomeAddress { get; set; }//+
        public string? PhysicalAddress { get; set; } //+
        public string? EmailAddress { get; set; }//+
        public string? Phone { get; set; }//+
        public string? PhoneCode { get; set; }//+
        public string? PhoneCountryCode { get; set; }//+
        [NotMapped]
        public string FullPhone
        {
            get
            {
                return $"{PhoneCode} {Phone}";
            }
        }

        public string? DescribeIllness { get; set; }//+
        public string? DuringBrief { get; set; }//+
        public string? EnLanguage { get; set; }  //add+
        public string? AzeLanguage { get; set; }  //add+
        public string? RuLanguage { get; set; }  //add+
        public string? OtherLanguage { get; set; }  //add+

        public string? WorkExperienceYES { get; set; }  //add+
        public string? AnyOtherSkillsYouHave { get; set; }  //add+
        public string? YourSkillsThatCanSupportYourApplication { get; set; }  //add+
        public string? WorkingAtSeaYesNo { get; set; }  //add+
                                                        //  public string? DriverLicenseYesNo { get; set; }  //add
        public string? ImgName { get; set; } //add+
        public string? ImgPath { get; set; } //add+
        public string? CandidatePositions { get; set; }//+
        public Guid? PositionId { get; set; }//+
        public Position Position { get; set; }
        public Guid? DepartmentId { get; set; }//+
        public Department Department { get; set; }

        // Relationships
        public virtual ICollection<Cv_Eduction> Cv_Eductions { get; set; }
        public virtual ICollection<Cv_ReferanceFrient> Cv_ReferanceFrients { get; set; }
        public virtual ICollection<Cv_WorkExperience> Cv_WorkExperiences { get; set; }
        public virtual ICollection<Cv_OtherFile> Cv_OtherFiles { get; set; }
        public virtual ICollection<EmploymentCv_Position_PIVOT> EmploymentCv_Position_PIVOTs { get; set; }
        public virtual Files? Files { get; set; }
        public DateTime CreatedDate { get; set; }//+
        //=================================
        public string? FinCode { get; set; }//+
        public string? Education { get; set; }//+
        public string? CompanyName { get; set; }//+
        public string? LatestPosition { get; set; }//+
        public string? CompanyAddress { get; set; }//+
        public string? BusinessType { get; set; }//+
        public DateTime? StartDate { get; set; }//+
        public DateTime? EndDate { get; set; }//+
        public string? CompanyPosition { get; set; }//+
        public string? ReasonLeaving { get; set; }//+
        public string? LanguageSkill { get; set; }//+
        public Guid? FilesId { get; set; }//+
        public string? BusinessType2 { get; set; }//+
        public string? BusinessType3 { get; set; }//+
        public string? CompanyAddress2 { get; set; }//+
        public string? CompanyAddress3 { get; set; }//+
        public string? CompanyName2 { get; set; }//+
        public string? CompanyName3 { get; set; }//+
        public string? CompanyPosition2 { get; set; }//+
        public string? CompanyPosition3 { get; set; }//+
        public DateTime? EndDate2 { get; set; }//+
        public DateTime? EndDate3 { get; set; }//+
        public string? LatestPosition2 { get; set; }//+
        public string? LatestPosition3 { get; set; }//+
        public string? ReasonLeaving2 { get; set; }//+
        public string? ReasonLeaving3 { get; set; }//+
        public DateTime? StartDate2 { get; set; }//+
        public DateTime? StartDate3 { get; set; }//+
        public string? AttendTraining { get; set; }//+
        public string? HaveValid { get; set; }//+
        public string? PerformJob { get; set; }//+
        public string? ShipyardOffshore { get; set; }//+
        public string? WriteEmployer { get; set; }//+
        public string? WriteExperience { get; set; }//+
        public string? WriteAddInfo { get; set; }//+
        public string? CV_Interview_Process_Status { get; set; }
        public string? Interview_Application_Status { get; set; }
    }
}
