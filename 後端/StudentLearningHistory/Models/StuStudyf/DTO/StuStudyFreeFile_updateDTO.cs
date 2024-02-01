

namespace StudentLearningHistory.Models.StuStudyf.DTO
{
    public class StuStudyFreeFile_updateDTO
    {
        public string std_no { get; set; }
        public int year_id { get; set; }
        public int sms_id { get; set; }
        public int ser_id { get; set; }
        public int number_id { get; set; }
        public IFormFile file { get; set; }
        public string? class_name { get; set; } = "StuStudyf";
        public int x_cnt { get; set; }//檔案數量
    }
}
