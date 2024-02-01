using System;
using System.Collections.Generic;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.DBModels;

public partial class L01_centraldb_learning_history
{
    public string sch_no { get; set; } = null!;

    public short year_id { get; set; }

    public short sms_id { get; set; }

    public string kind { get; set; } = null!;

    public string cls { get; set; } = null!;

    public string idno { get; set; } = null!;
    public int ser_id { get; set; }

    public string? birthday { get; set; }

    public string? json_head { get; set; }

    public string? json_content { get; set; }

    public string? upd_name { get; set; }

    public string? upd_dt { get; set; }
    public string? zip_name { get; set; }
    public string? std_freeback { get; set; }
    public string? remark { get; set; }
    public string? is_check { get; set; }
    public int import_ser { get; set; }
}
