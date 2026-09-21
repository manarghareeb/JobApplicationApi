//using Application.DTOs.Jobs;
//using Application.Exceptions;
//using Application.Interfaces;
//using Application.Interfaces.Services;
//using AutoMapper;
//using Domain.Entities;

//namespace Application.Services
//{
//    public class JobService(IUnitOfWork _unitOfWork, IMapper _mapper) : IJobService
//    {
//        public async Task<JobDto> CreateJobAsync(CreateJobDto createJobDto, int recruiterId)
//        {
//            var job = _mapper.Map<Job>(createJobDto);
//            job.RecruiterId = recruiterId;
//            job.IsActive = true;
//            job.CreatedAt = DateTime.UtcNow;
//            await _unitOfWork.GetRepository<Job>().AddAsync(job);
//            await _unitOfWork.SaveChangesAsync();
//            return _mapper.Map<JobDto>(job);
//        }

//        public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
//        {
//            var jobs = _unitOfWork.GetRepository<Job>().GetAll().Where(j => j.IsActive).ToList();
//            return _mapper.Map<IEnumerable<JobDto>>(jobs);
//        }

//        public async Task<JobDto> GetJobByIdAsync(int jobId)
//        {
//            var job = await _unitOfWork.GetRepository<Job>().GetByIdAsync(jobId);
//            if (job is null)
//                throw new NotFoundException("Job not found.");
//            return _mapper.Map<JobDto>(job);
//        }

//        public async Task<JobDto> UpdateJobAsync(int jobId, UpdateJobDto dto, int recruiterId)
//        {
//            var job = await _unitOfWork.GetRepository<Job>().GetByIdAsync(jobId);
//            if (job is null)
//                throw new NotFoundException("Job not found.");
//            if (job.RecruiterId != recruiterId)
//                throw new ForbiddenException(
//                    "You are not the owner of this job.");
//            if (!job.IsActive)
//                throw new BadRequestException(
//                    "A closed job cannot be updated.");
//            if (dto.Title is not null)
//                job.Title = dto.Title;
//            if (dto.Description is not null)
//                job.Description = dto.Description;
//            _unitOfWork.GetRepository<Job>().Update(job);
//            await _unitOfWork.SaveChangesAsync();
//            return _mapper.Map<JobDto>(job);
//        }

//        public async Task CloseJobAsync(int jobId, int recruiterId)
//        {
//            var job = await _unitOfWork.GetRepository<Job>().GetByIdAsync(jobId);
//            if (job is null)
//                throw new NotFoundException("Job not found.");
//            if (job.RecruiterId != recruiterId)
//                throw new ForbiddenException(
//                    "You are not the owner of this job.");
//            if (!job.IsActive)
//                throw new BadRequestException("Job is already closed.");
//            job.IsActive = false;
//            job.ClosedAt = DateTime.UtcNow;
//            _unitOfWork.GetRepository<Job>().Update(job);
//            await _unitOfWork.SaveChangesAsync();
//        }
//    }
//}