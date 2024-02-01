namespace StudentLearningHistory.Models.CentralDBofLearningHistory.Std
{
    public class StdFeedBackListDTO
    {
        public short year_id {  get; set; }
        public short sms_id {  get; set; }
        public int ser_id {  get; set; }
        public string kind {  get; set; }
        public string cls {  get; set; }
        public string std_no {  get; set; }
        public int? error_code {  get; set; }
        public string? name {  get; set; }
        public string? std_feedback {  get; set; }
        public string? answer {  get; set; }
        public int total { get; set; } = 0;
    }
}
