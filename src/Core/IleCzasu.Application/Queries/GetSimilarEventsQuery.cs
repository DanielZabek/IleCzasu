using IleCzasu.Application.Models;
using IleCzasu.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IleCzasu.Application.Queries
{
    public class GetSimilarEventsQuery : IRequest<List<SimilarEventModel>>
    {
        public int PublicEventId { get; set; }
    }
    public class GetSimilarEventsQueryHandler : IRequestHandler<GetSimilarEventsQuery, List<SimilarEventModel>>
    {
        private readonly ApplicationDbContext _context;

        public GetSimilarEventsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SimilarEventModel>> Handle(GetSimilarEventsQuery request, CancellationToken cancellationToken)
        {
            var tagIdList = _context.TagEvents.Where(te => te.PublicEventId == request.PublicEventId).Select(te => te.TagId).ToList();
            var similarEvents = _context.TagEvents
                .Include(p => p.Event)
                .Where(t => tagIdList.Contains(t.TagId) && t.PublicEventId != request.PublicEventId && t.Event.Date >= DateTime.Today).Select(e => e.Event)
                .GroupBy(pe => pe)
                .Select(s => new SimilarEventModel
                {
                    PublicEventId = s.Key.PublicEventId,
                    Name = s.Key.Name,
                    Description = s.Key.Description,
                    Date = s.Key.Date,
                    ImagePath = s.Key.ImagePath,
                    SimilarPoints = s.Count()
                }).OrderByDescending(p => p.SimilarPoints).Take(5).ToList();

            //var publicEvent =  _context.PublicEvents
            //    .Where(p => p.EventId == request.EventId)
            //    .Select(PublicEventDTO.Projection).SingleOrDefault();

            return similarEvents;
        }
    }
}

