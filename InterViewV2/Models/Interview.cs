using System.ComponentModel.DataAnnotations.Schema;

namespace InterViewV2.Models
{
    public class Interview
    {
        public Guid Id { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? Interview1Date { get; set; }
        public bool? Interview1Result { get; set; }
        public string? Interview1Comment { get; set; }
        public DateTime? Interview2Date { get; set; }
        public bool? Interview2Result { get; set; }
        public string? Interview2Comment { get; set; }
        public DateTime? Interview3Date { get; set; }
        public bool? Interview3Result { get; set; }
        public string? Interview3Comment { get; set; }
        public int? Age { get; set; }
        public string? FamilySituation { get; set; }
        public string? Children { get; set; }
        public string? ChildrenSum { get; set; }
        public string? MilitaryStatus { get; set; }
        public string? Education { get; set; }
        public string? Qualification { get; set; }
        public string? ProfessionalLiscences { get; set; }
        public string? PreviousEmployer1 { get; set; }
        public string? AssignedDuties1 { get; set; }
        public string? PreviousEmployer2 { get; set; }
        public string? AssignedDuties2 { get; set; }
        public string? PreviousEmployer3 { get; set; }
        public string? AssignedDuties3 { get; set; }
        public string? CDCReference { get; set; }
        //public string? RequiredPosition { get; set; }
        public string? CDCKnow { get; set; }
        public string? ApplyingPosition { get; set; }
        public string? ConsideringPosition { get; set; }
        public string? OffshoreExperience { get; set; }
        public string? DescribeOther { get; set; }
        public string? SafetyExperience { get; set; }
        public string? StopSmart { get; set; }
        public string? CurrentLast { get; set; }
        public string? EmergencySituation { get; set; }
        public string? GreatestStrength { get; set; }
        public string? CurrentJob { get; set; }
        public string? DescribeDecision { get; set; }
        public string? TeamPlayer { get; set; }
        public string? MinimumEnglish { get; set; }
        public string? WrittenEnglish { get; set; }
        public string? CareerGoals { get; set; }
        public string? CDCHire { get; set; }
        //public string? PolicyAcross { get; set; }
        public string? Hobbies { get; set; }
        public string? AnyQuestions { get; set; }
        public string? Presentation { get; set; }
        public string? Attitude { get; set; }
        public int? Intelligence { get; set; }
        public int? Suitable { get; set; }
        public int? Intelligence1 { get; set; }
        public int? Suitable1 { get; set; }
        public int? Intelligence2 { get; set; }
        public int? Suitable2 { get; set; }
        public int? Intelligence3 { get; set; }
        public int? Suitable3 { get; set; }
        public string? ResultComment { get; set; }
        public string? Interviewer1ChatAdmin { get; set; }
        public string? Interviewer2ChatAdmin { get; set; }
        public string? Interviewer3ChatAdmin { get; set; }
        public string? InterviewExam { get; set; }
        public double? ExamScore { get; set; }
        public string? ExamComment { get; set; }
        public string? HireComment { get; set; }
        public string? HireReason { get; set; }
        public string? LastSalary { get; set; }
        public string? InterviewWaive { get; set; }
        public List<InterviewPosition>? Positions { get; set; }
        [NotMapped]
        public List<Guid> SelectedPositions { get; set; }
        public int? ExamUserId { get; set; }
        public User ExamUser { get; set; }
        public int? User1Id { get; set; }
        public User? User1 { get; set; }
        public int? User2Id { get; set; }
        public User? User2 { get; set; }
        public int? User3Id { get; set; }
        public User? User3 { get; set; }
        public int? HireUserId { get; set; }
        public User? HireUser { get; set; }
        public Guid CondidateId { get; set; }
        public Condidate Condidate { get; set; }
        public Guid? PositionId { get; set; }
        public Position Position { get; set; }
        public Guid? DepartmentId { get; set; }
        public Department Department { get; set; }
        public Guid? DepartmentIntwId { get; set; }
        public Department DepartmentIntw { get; set; }
        public Guid? FilesId { get; set; }
        public Files Files { get; set; }
        [NotMapped]
        public List<File> FilesList { get; set; }
        public Guid EmploymentCvId { get; set; }
        public string? CV_Interview_Process_Status { get; set; }
        public string? Interview_Application_Status { get; set; }
    }
}