using StudentLearningHistory.Attributes;
using System.Text.Json.Serialization;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.OutputJsonDTO
{
    public class 課程學習成果內層_出
    {
        
        public List<string> 學期課程學習成果 { get; set; }

        
        public List<string> 補修課程學習成果 { get; set; }

        
        public List<string> 重修課程學習成果 { get; set; }

        
        public List<string> 重讀課程學習成果 { get; set; }

        
        public List<string> 轉學轉科課程學習成果 { get; set; }

        
        public List<string> 借讀課程學習成果 { get; set; }

        
        public List<string> 進修部學期課程學習成果 { get; set; }

        
        public List<string> 進修部轉學 { get; set; }

        
        public List<string> 進修部借讀課程學習成果 { get; set; }
    }
}
