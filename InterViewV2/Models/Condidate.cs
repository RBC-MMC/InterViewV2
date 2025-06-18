using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InterViewV2.Models
{
    public class Condidate
    {
        public Guid Id { get; set; }
        [MaxLength(30)]
        public string FinCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; }
        public string PhoneCode { get; set; }
        public string PhoneCountryCode { get; set; }
        [NotMapped]
        public string FullPhone
        {
            get
            {
                return $"{PhoneCode} {Phone}";
            }
        }
        public string CommonComment { get; set; }
        public List<Interview> Interview { get; set; }
        public Guid? CvId { get; set; }
        public EmploymentCv Cv { get; set; }
    }
}
