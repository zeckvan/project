
namespace StudentLearningHistory.Models.StudCadre.Parameters
{
    public class StudCadreParameter_DB
    {
        public string sch_no { get; set; }
        public string std_no { get; set; }
        public int year_id { get; set; }
        public int sms_id { get; set; }
        public string unit_name { get; set; }
        public string position_name { get; set; }
        public int? sRowNun { get; set; } = 0;
        public int? eRowNun { get; set; } = 9999;

        public string complex_key
        {
            get { return $"{sch_no}_{std_no}_{year_id}_{sms_id}_{unit_name}_{position_name}"; }
        }
        public int number_id { get; set; }
        public string? class_name { get; set; } = "StudCadre";

        public int? x_cnt { get; set; }//檔案數量
        public string? content { get; set; }

        public string? check_centraldb { get; set; }
        public string? is_sys { get; set; }
    }
}
