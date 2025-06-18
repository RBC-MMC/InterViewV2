namespace InterViewV2.Models
{
    public class EmploymentCv_Position_PIVOT
    {
        public int Id { get; set; }
        public Guid EmploymentCvId { get; set; }
        public virtual EmploymentCv EmploymentCv { get; set; }
        public Guid PositionId { get; set; }
        public virtual Position Position { get; set; }
    }
}
