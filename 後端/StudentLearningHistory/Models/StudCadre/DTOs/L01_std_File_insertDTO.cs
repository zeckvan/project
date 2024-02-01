using System.ComponentModel.DataAnnotations;

namespace StudentLearningHistory.Models.StudCadre.DTOs
{
    public class L01_std_File_insertDTO
    {
        public string std_no { get; set; }
        public int year_id { get; set; }
        public int sms_id { get; set; }
        public string unit_name { get; set; }
        public string position_name { get; set; }
        public IFormFileCollection files { get; set; }
        public string? class_name { get; set; } = "StudCadre";
        public string? content{ get; set; }
}
}
