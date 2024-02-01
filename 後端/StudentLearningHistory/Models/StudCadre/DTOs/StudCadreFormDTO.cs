using System.ComponentModel.DataAnnotations;
using StudentLearningHistory.Models.StuPublickFileHub.DTO;

namespace StudentLearningHistory.Models.StudCadre.DTOs
{
    public class StudCadreFormDTO
    {
        public string std_no { get; set; }
        public int year_id { get; set; }
        public int sms_id { get; set; }
        public string unit_name { get; set; }
        public string position_name { get; set; }
        [StringLength(13)]
        public string? startdate { get; set; }
        [StringLength(13)]
        public string? enddate { get; set; }
        [StringLength(1)]
        public string? type_id { get; set; }
        [StringLength(1)]
        public string? is_sys { get; set; }
        public IEnumerable<L01_std_public_filehub_DTO> files { get; set; }
    }
}
