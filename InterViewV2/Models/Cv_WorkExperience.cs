namespace InterViewV2.Models
{
    public class Cv_WorkExperience
    {
        public int WorkExperience_ID { get; set; }
        public string? CompanyName { get; set; }
        public string? LatestPosition { get; set; }
        public string? JobResponsibilities { get; set; }
        public DateTime? WorkStartDate { get; set; }
        public DateTime? WorkEndtDate { get; set; }
        public string? IAmCurrentlyWorking { get; set; }
        public string? ReasonForLeavingWork { get; set; }

        // Relation
        public Guid EmploymentCvId { get; set; }
        public virtual EmploymentCv? EmploymentCv { get; set; }
    }
}
