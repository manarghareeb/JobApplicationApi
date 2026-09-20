using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class JobRepository(ApplicationDbContext _context) : GenericRepository<Job>(_context), IJobRepository
    {
    }
}
