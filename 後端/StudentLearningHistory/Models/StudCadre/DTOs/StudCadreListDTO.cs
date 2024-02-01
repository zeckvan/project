using StudentLearningHistory.Models.StuPublickFileHub.DTO;

namespace StudentLearningHistory.Models.StudCadre.DTOs
{
    public class StudCadreListDTO
    {
        public string a { get; set; }
        public string b { get; set; }
        public int c { get; set; }
        public int d { get; set; }
        public string e { get; set; }
        public string f { get; set; }
        public string g { get; set; }
        public string h { get; set; }
        public string i { get; set; }
        public string j { get; set; }
        public IEnumerable<L01_std_public_filehub_DTO> files { get; set; }
        public int x_cnt { get; set; }//檔案數量
        public int x_total { get; set; }
        public string? zz { get; set; }
        public string? x_status { get; set; }
    }
}
