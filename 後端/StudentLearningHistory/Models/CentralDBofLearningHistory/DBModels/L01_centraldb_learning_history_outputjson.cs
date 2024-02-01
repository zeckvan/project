namespace StudentLearningHistory.Models.CentralDBofLearningHistory.DBModels
{
    public class L01_centraldb_learning_history_outputjson
    {
        public string kind { get; set; }
        public string cls { get; set; }
        public string json_head { get; set; }
        public string json_content { get; set; }
        public string zip_name { get; set; }
        public string upd_name { get; set; }
        public string s_dt
        {
            get
            {
                return _s_dt?.ToString("yyyy/MM/dd HH:mm") ?? "";
            }
        }
        public string e_dt
        {
            get
            {
                return _e_dt?.ToString("yyyy/MM/dd HH:mm") ?? "";
            }
        }
        public DateTime? _s_dt { get; set; }
        public DateTime? _e_dt { get; set; }
    }
}
