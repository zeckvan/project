using System;
using System.Collections.Generic;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.DBModels;

public partial class L01_centraldb_learning_history_error_code
{
    public int id { get; set; }

    public string? name { get; set; }

    public string? is_useing { get; set; }

    public string? upd_name { get; set; }

    public string? upd_dt { get; set; }
}
