using System.ComponentModel.DataAnnotations;

namespace InterViewV2.Models
{
    public class File
    {

        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Extension { get; set; }

        public Guid? FilesId { get; set; }
        public Files Files { get; set; }
    }
}
