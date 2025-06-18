namespace InterViewV2.Models
{
    public class InterviewPosition
    {
        public Guid Id { get; set; }

        public Guid InterviewId { get; set; }
        public Interview Interview { get; set; }
        public Guid PositionId { get; set; }
        public Position Position { get; set; }
    }
}
