namespace StudentLearningHistory.Models.TeaConsult.DbModels
{
    public class SubHeaderList
    {
        public string a { get; set; } //班級
        public string b { get; set; }//學號
        public string c { get; set; }//姓名 
        public string d { get; set; }//座號

        public Boolean x_status { get; set; } = false;
        public int x_cnt { get; set; }
    }
}
