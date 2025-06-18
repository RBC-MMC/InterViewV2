namespace InterViewV2.Models
{
    public class Cv_OtherFile
    {
        public Guid OtherFile_ID { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }

        // Relation
        public Guid EmploymentCvId { get; set; }
        public virtual EmploymentCv? EmploymentCv { get; set; }
    }
}
