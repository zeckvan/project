namespace StudentLearningHistory.Models.ExportExcel.Parameters
{
    public class Parameter
    {
        public short year_id {  get; set; }
        //public short sms_id {  get; set; }
        public int grd_id {  get; set; }
        public int type {  get; set; }
        public string? token{ get; set; }
    }
}
