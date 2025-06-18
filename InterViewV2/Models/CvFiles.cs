using System.ComponentModel.DataAnnotations.Schema;

namespace InterViewV2.Models
{
    public class CvFiles
    {
        public Guid Id { get; set; }

        public Guid PositionId { get; set; }
        public Position Position { get; set; }
        public Guid? FilesId { get; set; }
        public Files Files { get; set; }
        [NotMapped]
        public List<File> FilesList { get; set; }
    }
}
