using DocumentFormat.OpenXml.VariantTypes;

namespace InterViewV2.Models
{
    public class Vacancy
    {
        public int? Id { get; set; }
        public string? VacancyCode { get; set; }
        public DateTime CreateDate { get; set; }
        public int Position_Id { get; set; }
        public bool? Status { get; set; }
        public int? AvailableVacantCount { get; set; }
        // Relation
        public Guid? EmploymentCvId { get; set; }
        public virtual EmploymentCv? EmploymentCv { get; set; }
        public Guid? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public int? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
