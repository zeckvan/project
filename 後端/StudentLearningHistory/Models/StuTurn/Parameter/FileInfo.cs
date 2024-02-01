namespace StudentLearningHistory.Models.StuTurn.Parameter
{
    public class FileInfo
    {
        public IFormFileCollection? files { get; set; }
        public string? Password { get; set; }    
        public string? MD5 { get; set; }
    }
}
