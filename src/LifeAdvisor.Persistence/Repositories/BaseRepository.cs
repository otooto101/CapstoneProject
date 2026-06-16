using LifeAdvisor.Application.Interfaces.Repositories;
using LifeAdvisor.Application.Models;
using LifeAdvisor.Domain.Specifications;
using LifeAdvisor.Persistence.Specification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LifeAdvisor.Persistence.Repositories
{
    public abstract class BaseRepository<T, TContext>(TContext context) : IRepository<T>
        where T : class
        where TContext : DbContext
    {
        protected readonly TContext _context = context;

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _context.Set<T>().FindAsync(new object[] { id }, ct);

        // --- Specification Implementation ---
        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken ct = default)
            => await ApplySpecification(spec).ToListAsync(ct);

        public async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken ct = default)
            => await ApplySpecification(spec).FirstOrDefaultAsync(ct);

        public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken ct = default)
        {
            var query = _context.Set<T>().AsQueryable();

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            return await query.CountAsync(ct);
        }

        public IQueryable<T> ApplySpecification(ISpecification<T> spec)
            => SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);

        // --- Standard CRUD ---
        public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _context.Set<T>().Where(predicate).ToListAsync(ct);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _context.Set<T>().AnyAsync(predicate, ct);

        public async Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            await _context.Set<T>().AddAsync(entity, ct);
            return entity;
        }

        public void Update(T entity)
            => _context.Entry(entity).State = EntityState.Modified;

        public void Delete(T entity)
            => _context.Set<T>().Remove(entity);

        public async Task<PaginatedList<T>> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            ISpecification<T> spec,
            CancellationToken ct = default)
        {
            var totalCount = await CountAsync(spec, ct);

            var query = ApplySpecification(spec);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PaginatedList<T>(items, totalCount, pageNumber, pageSize);
        }
    }
}
