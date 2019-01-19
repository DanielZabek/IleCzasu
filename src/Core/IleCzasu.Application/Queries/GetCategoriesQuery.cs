using IleCzasu.Application.Models;
using IleCzasu.Data.Entities;
using IleCzasu.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IleCzasu.Application.Queries
{
    public class GetCategoriesQuery : IRequest<IEnumerable<Category>>
    {
    }

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<Category>>
    {
        private readonly ApplicationDbContext _context;

        public GetCategoriesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _context.Categories.Include(c => c.SubCategories).ToListAsync(cancellationToken);

            return categories;
        }
    }
}

