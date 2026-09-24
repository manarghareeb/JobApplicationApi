using System.Linq.Expressions;

namespace Application.Interfaces.Services
{
    public interface IBackgroundJobScheduler
    {
        void Enqueue<T>(Expression<Action<T>> methodCall);
        void Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);
        void AddOrUpdate<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression);
    }
}
