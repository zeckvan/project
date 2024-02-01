using AutoMapper;
using StudentLearningHistory.Models.StudentInfo.DbModels;
using StudentLearningHistory.Models.StudentInfo.DTOs;
using System.Text;

namespace StudentLearningHistory.Mappings
{
    public class StudentInfoMapperProfile : Profile
    {
        public StudentInfoMapperProfile()
        {
            CreateMap<V_StuphrRecord, StuphrRecordDTO>()
                .ForMember(dest => dest.msp_h_dt, opt => opt.MapFrom(o => (o.msp_h_dt.HasValue ? o.msp_h_dt.Value.ToString("yyyy/MM/dd") : "" )))
                .ForMember(dest => dest.reward, opt => opt.MapFrom((o, _) =>
                {
                    StringBuilder sb = new StringBuilder();
                    if (o.mpw_kind != null)
                    {
                        if ((o.msp_bgb ?? 0) > 0)
                        {
                            sb.Append( $"{ (o.mpw_kind == "A" ? "大功":"大過") }{o.msp_bgb}支" );
                        }
                        if((o.msp_lgb ?? 0) > 0)
                        {
                            sb.Append($"{(o.mpw_kind == "A" ? "小功" : "小過")}{o.msp_lgb}支");
                        }
                        if((o.msp_cw ?? 0) > 0)
                        {
                            sb.Append($"{(o.mpw_kind == "A" ? "嘉獎" : "警告")}{o.msp_cw}支");
                        }
                    }

                    return sb.ToString();
                }))
            ;
        }
    }
}
