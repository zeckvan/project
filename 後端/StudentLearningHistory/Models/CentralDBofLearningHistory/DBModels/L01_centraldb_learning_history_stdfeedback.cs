using System;
using System.Collections.Generic;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.DBModels;

public partial class L01_centraldb_learning_history_stdfeedback
{
    public string? sch_no { get; set; }

    public short year_id { get; set; }

    public short sms_id { get; set; }

    public int ser_id { get; set; }

    public string kind { get; set; } = null!;

    public string cls { get; set; } = null!;

    public string? std_no { get; set; } = null!;

    public int? error_code { get; set; }
    public string? std_feedback { get; set; }

    public string? answer { get; set; }

    public string? upd_name { get; set; }

    public string? upd_dt { get; set; }
    public string? create_name { get; set; }
    public string? create_dt { get; set; }
    public string? token { get; set; }
}
