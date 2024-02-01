
namespace StudentLearningHistory.Models.StuStudyf.DTO;

public partial class L01_stu_study_free_DTO
{
    public string? sch_no { get; set; }

    public short year_id { get; set; }

    public short sms_id { get; set; }

    public string? std_no { get; set; } = null!;

    public short ser_id { get; set; }

    public string is_sys { get; set; }

    public string type_id { get; set; }

    public string open_name { get; set; }

    public string? open_unit { get; set; }

    public short hours { get; set; }

    public short weeks { get; set; }

    public string? content { get; set; }
    public string? check_centraldb { get; set; }
    public string? token { get; set; }
}
