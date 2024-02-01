using System;
using System.Collections.Generic;

namespace StudentLearningHistory.Models.CentralDBofLearningHistory.DateSetupDTO;

public partial class Datetime_setupDTO
{
    public short check { get; set; }
    public short year_id { get; set; }

    public short sms_id { get; set; }

    public string kind { get; set; } = null!;

    public string? s_dt { get; set; }

    public string? e_dt { get; set; }
    public string? zip_name { get; set; }
    public string? token { get; set; }
}
