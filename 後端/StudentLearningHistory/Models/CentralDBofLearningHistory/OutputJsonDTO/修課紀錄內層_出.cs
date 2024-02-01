using StudentLearningHistory.Attributes;
using System.Text.Json.Serialization;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.OutputJsonDTO
{
    public class 修課紀錄內層_出
    {
        
        public List<string> 學期成績 { get; set; }

        
        public List<string> 補修成績 { get; set; }

        
        public List<string> 轉學轉科成績 { get; set; }

        
        public List<string> 重修成績 { get; set; }

        
        public List<string> 重讀成績 { get; set; }

        
        public List<string> 進修部學期成績 { get; set; }

        
        public List<string> 進修部補考成績 { get; set; }

        
        public List<string> 進修部轉學轉科成績 { get; set; }
    }
}
