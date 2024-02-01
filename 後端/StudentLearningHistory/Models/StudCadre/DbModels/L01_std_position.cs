using System.ComponentModel.DataAnnotations;

namespace StudentLearningHistory.Models.StudCadre.DbModels
{
    public class L01_std_position
    {
        public string sch_no { get; set; }
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
        [StringLength(18)]
        public string? upd_name { get; set; }
        [StringLength(13)]
        public string? upd_dt { get; set; }

        public int x_cnt { get; set; }//檔案數量
        public int x_total { get; set; }
        public string? x_status { get; set; }
        public string? zz { get; set; }
    }
}
