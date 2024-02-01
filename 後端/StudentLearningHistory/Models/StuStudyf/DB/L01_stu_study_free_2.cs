using System;
using System.Collections.Generic;

namespace StudentLearningHistory.Models.StuStudyf.DB;

public partial class L01_stu_study_free_2
{
    public string?sch_no { get; set; } 

    public int? year_id { get; set; }

    public int? sms_id { get; set; }

    public string? std_no { get; set; }

    public int? ser_id { get; set; }

    public string? type_id { get; set; }

    public string? open_name { get; set; }

    public string? open_unit { get; set; }

    public int? hours { get; set; }

    public int? weeks { get; set; }

    public string? content { get; set; }
    public string? is_sys { get; set; }
    public string? check_centraldb { get; set; }
    public string? upd_name { get; set; }
    public string? upd_dt { get; set; }
    public string? token { get; set; }
}
