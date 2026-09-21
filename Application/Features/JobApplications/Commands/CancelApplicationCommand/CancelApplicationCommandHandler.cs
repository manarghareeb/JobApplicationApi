using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Enums;
using MediatR;

namespace Application.Features.JobApplications.Commands.CancelApplicationCommand
{
    internal class CancelApplicationCommandHandler(IUnitOfWork _unitOfWork, ICurrentUserService _currentUserService) : IRequestHandler<CancelApplicationCommand>
    {
        public async Task Handle(CancelApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _unitOfWork.GetRepository<Domain.Entities.JobApplication>().GetByIdAsync(request.Id);
            if (application is null)
                throw new NotFoundException("Application not found.");
            if (application.CandidateId != _currentUserService.CandidateId)
                throw new ForbiddenException("You are not the owner of this application.");
            if (application.JobApplicationStatus != JobApplicationStatus.Applied &&
                application.JobApplicationStatus != JobApplicationStatus.UnderReview)
                throw new BadRequestException("The application cannot be cancelled in its current status.");
            application.JobApplicationStatus = JobApplicationStatus.Cancelled;
            application.CancelledAt = DateTime.UtcNow;
            application.StatusUpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepository<Domain.Entities.JobApplication>().Update(application);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
