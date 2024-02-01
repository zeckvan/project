namespace StudentLearningHistory.Models.StuTurn.Parameter
{
    public class GetFileInfo
    {
        public int? id { get; set; }
        public string? file_name { get; set; }
        public string? file_extension { get; set; }
        public byte[]? file_blob { get; set; }
        public string? content { get; set; }
        public string? file_md5 { get; set; }
        public string? zipcode { get; set; }
        public string? stulist { get; set; }
        public string? upd_name { get; set; }
        public string? upd_dt { get; set; }
    }
}
