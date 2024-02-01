namespace StudentLearningHistory.Models.StuPublickFileHub.DTO
{
    public class L01_std_public_filehub_DTO
    {
        public int type_id { get; set; }
        public int number_id { get; set; }
        public string? file_name { get; set; }
        public string? file_extension { get; set; }
        public int file_class { get; set; }     
        public string upd_dt { get; set; }
        public string? attestation_file_yn { get; set; }
        public string complex_key { get; set; }
        public int x_file_center_cnt { get; set; } 
        public int x_centraldb { get; set; }
        public string? content { get; set; }
    }
}
