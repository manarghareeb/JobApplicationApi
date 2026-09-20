using Application.DTOs.Jobs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class JobProfile : Profile
    {
        public JobProfile()
        {
            CreateMap<Job, JobDto>();
            CreateMap<CreateJobDto, Job>();
            CreateMap<UpdateJobDto, Job>().ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember is not null));
        }
    }
}
