using Application.Interfaces.Services;
using Hangfire;
using System.Linq.Expressions;

namespace Application.Services
{
    public class HangfireBackgroundJobScheduler(IBackgroundJobClient _backgroundJobClient, IRecurringJobManager _recurringJobManager) 
        : IBackgroundJobScheduler
    {
        public void AddOrUpdate<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression)
        {
            _recurringJobManager.AddOrUpdate(recurringJobId, methodCall, cronExpression);
        }

        public void Enqueue<T>(Expression<Action<T>> methodCall)
        {
            _backgroundJobClient.Enqueue<T>(methodCall);
        }
        public void Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
        {
            _backgroundJobClient.Schedule<T>(methodCall, delay);
        }
    }
}