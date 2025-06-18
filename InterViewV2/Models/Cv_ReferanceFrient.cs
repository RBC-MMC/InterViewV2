namespace InterViewV2.Models
{
    public class Cv_ReferanceFrient
    {
        public int ReferanceFrient_ID { get; set; }
        public string? FriendReferenceName { get; set; }
        public string? FriendReferenceSurname { get; set; }
        public string? FriendReferencePhone { get; set; }

        // Relation
        public Guid EmploymentCvId { get; set; }

        public virtual EmploymentCv? EmploymentCv { get; set; }
    }
}
