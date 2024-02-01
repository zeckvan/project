using AutoMapper;
using StudentLearningHistory.Models.StudCadre.DbModels;
using StudentLearningHistory.Models.StudCadre.DTOs;
using StudentLearningHistory.Models.StudCadre.Parameters;
using StudentLearningHistory.Models.StuStudyf.Parameter;

namespace StudentLearningHistory.Mappings
{
    public class StudCadreMapperProfile: Profile
    {
        public StudCadreMapperProfile()
        {
            #region List
            CreateMap<L01_std_position, StudCadreListDTO>()
                .ForMember(dest => dest.a, opt => opt.MapFrom(o => o.sch_no))
                .ForMember(dest => dest.b, opt => opt.MapFrom(o => o.std_no))
                .ForMember(dest => dest.c, opt => opt.MapFrom(o => o.year_id))
                .ForMember(dest => dest.d, opt => opt.MapFrom(o => o.sms_id))
                .ForMember(dest => dest.e, opt => opt.MapFrom(o => o.unit_name))
                .ForMember(dest => dest.f, opt => opt.MapFrom((o, _) => {
                    string rt = o.startdate ?? "";

                    if (!string.IsNullOrWhiteSpace(rt))
                    {
                        rt = $"{rt.Substring(0, 4)}/{rt.Substring(4, 2)}/{rt.Substring(6, 2)}";
                    }

                    return rt;
                }))
                .ForMember(dest => dest.g, opt => opt.MapFrom((o, _) => {
                    string rt = o.enddate ?? "";

                    if (!string.IsNullOrWhiteSpace(rt))
                    {
                        rt = $"{rt.Substring(0, 4)}/{rt.Substring(4, 2)}/{rt.Substring(6, 2)}";
                    }

                    return rt;
                }))
                .ForMember(dest => dest.h, opt => opt.MapFrom(o => o.position_name))
                .ForMember(dest => dest.i, opt => opt.MapFrom((o, _) => {
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
                .ForMember(dest => dest.j, opt => opt.MapFrom(o => o.is_sys))
                ;
            #endregion

            #region form
            CreateMap<L01_std_position, StudCadreFormDTO>();
            #endregion

            #region inser or update
            CreateMap<L01_std_position_DTO, L01_std_position >();
            #endregion

            #region Parameters
            CreateMap<L01_std_position_DTO, StudCadreParameter_DB>();
            CreateMap<L01_std_File_insertDTO, StudCadreParameter_DB>();
            CreateMap<L01_std_File_updateDTO, StudCadreParameter_DB>();

            CreateMap<StudCadreParameters, StudCadreParameter_DB>();
            CreateMap<StudCadreParameter, StudCadreParameter_DB>();
            CreateMap<StudCadreParameter_DEL, StudCadreParameter_DB>();

            CreateMap<StudCadreFileParameter, StudCadreParameter_DB>();
            CreateMap<StudCadreFilesParameter, StudCadreParameter_DB>();
            CreateMap<StudCadreFileParameter_DEL, StudCadreParameter_DB>();

            CreateMap<StudCadreListDTO, StudCadreParameter_DB>()
                .ForMember(dest => dest.sch_no, opt => opt.MapFrom(o => o.a))
                .ForMember(dest => dest.std_no, opt => opt.MapFrom(o => o.b))
                .ForMember(dest => dest.year_id, opt => opt.MapFrom(o => o.c))
                .ForMember(dest => dest.sms_id, opt => opt.MapFrom(o => o.d))
                .ForMember(dest => dest.unit_name, opt => opt.MapFrom(o => o.e))
                .ForMember(dest => dest.position_name, opt => opt.MapFrom(o => o.h))
                .ForMember(dest => dest.x_cnt, opt => opt.MapFrom(o => o.x_cnt))
                ;
            #endregion
        }
    }
}
