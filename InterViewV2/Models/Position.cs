namespace InterViewV2.Models
{
    public class Position
    {

        public Guid Id { get; set; }
        public string PositionEnName { get; set; }
        public string PositionAzName { get; set; }

        public virtual ICollection<EmploymentCv_Position_PIVOT> EmploymentCv_Position_PIVOTs { get; set; }

        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}