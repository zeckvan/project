namespace StudentLearningHistory.Models.StuPublickFileHub.DB
{
    public class L01_std_public_filehub
    {
        public string complex_key { get; set; }
        public string class_name { get; set; }
        public int type_id { get; set; }
        public int number_id { get; set; }
        public string? file_name { get; set; }
        public string? file_extension { get; set; }
        public byte[]? file_blob { get; set; }
        public string? upd_name { get; set; }
        public string? upd_dt { get; set; }
        public string? content { get; set; }
        public string? token { get; set; }
        public string? file_md5 { get; set; }    
        public string? file_id { get; set; }
    }
}
