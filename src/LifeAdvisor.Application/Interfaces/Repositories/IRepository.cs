using LifeAdvisor.Application.Models;
using LifeAdvisor.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LifeAdvisor.Application.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken ct = default);
        Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken ct = default);
        Task<int> CountAsync(ISpecification<T> spec, CancellationToken ct = default);
        IQueryable<T> ApplySpecification(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<PaginatedList<T>> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            ISpecification<T> spec,
            CancellationToken ct = default);
        Task<T> AddAsync(T entity, CancellationToken ct = default);
        void Update(T entity);
        void Delete(T entity);
    }
}
