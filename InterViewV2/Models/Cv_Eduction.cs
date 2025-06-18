namespace InterViewV2.Models
{
    public class Cv_Eduction
    {
        public int Eduction_ID { get; set; }
        public string? EducationalInstitution { get; set; }
        public string? EducationDegree { get; set; }
        public string? EducationSpecialization { get; set; }
        public DateTime? EducationStartDate { get; set; }
        public DateTime? EducationEndDate { get; set; }

        // Relation
        public Guid EmploymentCvId { get; set; }
        public virtual EmploymentCv? EmploymentCv { get; set; }
    }
}
