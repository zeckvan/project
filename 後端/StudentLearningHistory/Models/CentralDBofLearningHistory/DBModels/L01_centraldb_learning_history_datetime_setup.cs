using System;
using System.Collections.Generic;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.DBModels;

public partial class L01_centraldb_learning_history_datetime_setup
{
    public string sch_no { get; set; } = null!;

    public short year_id { get; set; }

    public short sms_id { get; set; }

    public string kind { get; set; } = null!;

    public DateTime? s_dt { get; set; }

    public DateTime? e_dt { get; set; }

    public string? upd_name { get; set; }

    public string? upd_dt { get; set; }
}
