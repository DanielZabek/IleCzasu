using System.Collections.Generic;
using MediatR;
using IleCzasu.Application.Models;
using System.Threading;
using System.Threading.Tasks;
using IleCzasu.Infrastructure;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace IleCzasu.Application.Events.Queries
{
    public class GetPublicEventByIdQuery : IRequest<PublicEventDTO>
    {
        public int PublicEventId { get; set; }
        public string UserId { get; set; }
    }

    public class GetPublicEventByIdQueryHandler : IRequestHandler<GetPublicEventByIdQuery, PublicEventDTO>
    {
        private readonly ApplicationDbContext _context;

        public GetPublicEventByIdQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PublicEventDTO> Handle(GetPublicEventByIdQuery request, CancellationToken cancellationToken)
        {
            var data = _context.PublicEvents
                .Include(c => c.Comments).ThenInclude(u => u.User)
                .Include(c => c.Category)
                .Include(te => te.TagEvents)
                .ThenInclude(t => t.Tag)
                .ThenInclude(t => t.TagType)
                 .Where(p => p.PublicEventId == request.PublicEventId).ToList();
            var publicEvent = data.AsQueryable()
                .Select(PublicEventDTO.Projection).SingleOrDefault();

            if (!string.IsNullOrEmpty(request.UserId))
            {
                var user = _context.Users
                    .Include(f => f.UserFollows).SingleOrDefault(u => u.Id == request.UserId);

                if (user.UserFollows.Select(e => e.PublicEventId).Contains(publicEvent.PublicEventId))
                {
                    publicEvent.IsFollowed = true;
                }
            }

            return publicEvent;
        }
    }
}

