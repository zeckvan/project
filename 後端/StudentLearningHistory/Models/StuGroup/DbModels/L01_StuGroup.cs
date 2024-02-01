﻿namespace StudentLearningHistory.Models.StuGroup.DbModels
{
    public class L01_StuGroup
    {
        public string? sch_no { get; set; } //學校代碼
        public int year_id { get; set; }//年度
        public int sms_id { get; set; }//學期
        public string? std_no { get; set; }//學號
        public int? ser_id { get; set; }//序號
        public string time_id { get; set; }//時間類別
        public string? unit_name { get; set; }//辦理單位
        public string group_content { get; set; }//內容名稱
        public string startdate { get; set; }//開始時間
        public string enddate { get; set; }//結束時間
        public int hours { get; set; }//時數
        public string? content { get; set; }//內容簡述
        public string? is_sys { get; set; }//資料來源
        public string? upd_name { get; set; }//異動人
        public string? upd_dt { get; set; }//異動時間
        public int x_cnt { get; set; }//檔案數量
        public string? token { get; set; }
    }
}
