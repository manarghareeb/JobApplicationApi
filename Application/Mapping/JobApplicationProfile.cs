using Application.DTOs.JobApplicationDto;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class JobApplicationProfile : Profile
    {
        public JobApplicationProfile()
        {
            CreateMap<JobApplication, JobApplicationDto>();
        }
    }
}
