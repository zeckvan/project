using StudentLearningHistory.Models.StuPublickFileHub.DTO;

namespace StudentLearningHistory.Models.StuStudyf.DTO
{
    public class StuStudyFreeListDTO
    {
        public string a { get; set; }
        public int b { get; set; }
        public int c { get; set; }
        public string d { get; set; }
        public int e { get; set; }
        public string f { get; set; }
        public string g { get; set; }
        public string h { get; set; }
        public int i { get; set; }
        public int j { get; set; }
        public string k { get; set; }
        public string l { get; set; }
        public IEnumerable<L01_std_public_filehub_DTO> files { get; set; }
        public int x_cnt { get; set; }//檔案數量
        public string? zz { get; set; }
        public string? x_status { get; set; }
    }
}
