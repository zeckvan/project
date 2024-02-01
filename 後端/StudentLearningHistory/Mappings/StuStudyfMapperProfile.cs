using AutoMapper;
using StudentLearningHistory.Models.StuStudyf.DB;
using StudentLearningHistory.Models.StuStudyf.DTO;
using StudentLearningHistory.Models.StuStudyf.Parameter;
using StudentLearningHistory.Models.StuStudyf.Parameters;

namespace StudentLearningHistory.Mappings
{
    public class StuStudyfMapperProfile: Profile
    {
        public StuStudyfMapperProfile()
        {
            #region List
            CreateMap<L01_stu_study_free, StuStudyFreeListDTO>()
                .ForMember(dest => dest.a, opt => opt.MapFrom(o => o.sch_no))
                .ForMember(dest => dest.b, opt => opt.MapFrom(o => o.year_id))
                .ForMember(dest => dest.c, opt => opt.MapFrom(o => o.sms_id))
                .ForMember(dest => dest.d, opt => opt.MapFrom(o => o.std_no))
                .ForMember(dest => dest.e, opt => opt.MapFrom(o => o.ser_id))
                .ForMember(dest => dest.f, opt => opt.MapFrom((o, _) => {
                    string rt = o.type_id ?? "";

                    switch (rt)
                    {
                        case "1":
                            rt = "校級幹部";
                            break;
                        case "2":
                            rt = "班級幹部";
                            break;
                        case "3":
                            rt = "社團幹部";
                            break;
                        case "4":
                            rt = "實習幹部";
                            break;
                        case "5":
                            rt = "校外自治組織團體";
                            break;
                        case "9":
                            rt = "其他幹部";
                            break;
                    }

                    return rt;
                }))
                .ForMember(dest => dest.g, opt => opt.MapFrom(o => o.open_name))
                .ForMember(dest => dest.h, opt => opt.MapFrom(o => o.open_unit))
                .ForMember(dest => dest.i, opt => opt.MapFrom(o => o.hours))
                .ForMember(dest => dest.j, opt => opt.MapFrom(o => o.weeks))
                .ForMember(dest => dest.k, opt => opt.MapFrom(o => o.content))
                .ForMember(dest => dest.l, opt => opt.MapFrom(o => o.is_sys))
                ;
            #endregion

            #region inser or update
            CreateMap<L01_stu_study_free_DTO, L01_stu_study_free>().ReverseMap();
            #endregion

            #region Parameters
            CreateMap<L01_stu_study_free_DTO, StuStudyFreeParameter_DB>();
            CreateMap<StuStudyFreeFile_insertDTO, StuStudyFreeParameter_DB>();
            CreateMap<StuStudyFreeFile_updateDTO, StuStudyFreeParameter_DB>();

            //main
            CreateMap<StuStudyFreeParameter, StuStudyFreeParameter_DB>();
            CreateMap<StuStudyFreeParameterList, StuStudyFreeParameter_DB>();

            //file
            CreateMap<StuStudyFreeFileParameter, StuStudyFreeParameter_DB>();
            CreateMap<StuStudyFreeFileParameterList, StuStudyFreeParameter_DB>();

            CreateMap<StuStudyFreeListDTO, StuStudyFreeParameter_DB>()
                .ForMember(dest => dest.sch_no, opt => opt.MapFrom(o => o.a))
                .ForMember(dest => dest.year_id, opt => opt.MapFrom(o => o.b))
                .ForMember(dest => dest.sms_id, opt => opt.MapFrom(o => o.c))
                .ForMember(dest => dest.std_no, opt => opt.MapFrom(o => o.d))
                .ForMember(dest => dest.ser_id, opt => opt.MapFrom(o => o.e))
                .ForMember(dest => dest.x_cnt, opt => opt.MapFrom(o => o.x_cnt))
                ;
            #endregion
        }
    }
}
