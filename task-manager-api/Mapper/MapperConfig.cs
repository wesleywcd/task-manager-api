using AutoMapper;
using TaskAPI.Data.Entities;
using TaskAPI.Views;

namespace TaskAPI.Mapper;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<TaskModel, TaskView>().ReverseMap();
    }
}