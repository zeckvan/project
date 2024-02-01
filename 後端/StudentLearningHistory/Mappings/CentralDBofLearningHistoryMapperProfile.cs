using AutoMapper;
using StudentLearningHistory.Models.CentralDBofLearningHistory.DateSetupDTO;
using StudentLearningHistory.Models.CentralDBofLearningHistory.DBModels;

namespace StudentLearningHistory.Mappings
{
    public class CentralDBofLearningHistoryMapperProfile:Profile
    {
        public CentralDBofLearningHistoryMapperProfile()
        {
            CreateMap<Datetime_setupDTO, L01_centraldb_learning_history_datetime_setup>();
        }
    }
}
