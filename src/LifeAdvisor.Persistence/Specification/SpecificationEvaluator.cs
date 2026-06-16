using LifeAdvisor.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LifeAdvisor.Persistence.Specification
{
    public static class SpecificationEvaluator<TEntity> where TEntity : class
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;

            if (spec.IsNoTracking)
            {
                query = query.AsNoTracking();
            }

            // Filter
            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            // Includes
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

            // Order By
            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            // Paging
            if (spec.IsPagingEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            return query;
        }
    }
}
