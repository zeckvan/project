namespace StudentLearningHistory.Models.CentralDBofLearningHistory.FeedBackDTO
{
    public class FeedBackListDTO
    {
        public int ser_id {  get; set; }
        public string kind {  get; set; }
        public string cls {  get; set; }
        public string? cls_abr {  get; set; }
        public string? std_name {  get; set; }
        public short? sit_num {  get; set; }
        public string? name {  get; set; }
        public string? std_feedback {  get; set; }
        public string? answer {  get; set; }
        public string createDt { 
            get
            {
                if (string.IsNullOrWhiteSpace(create_dt))
                {
                    return "";
                }
                return $"{create_dt.Substring(0,3)}-{create_dt.Substring(3, 2)}-{create_dt.Substring(5, 2)} {create_dt.Substring(7, 2)}:{create_dt.Substring(9, 2)}:{create_dt.Substring(11, 2)}";
            }
        }
        public string? create_dt {  get; set; }
        public int total { get; set; } = 0;
    }
}
