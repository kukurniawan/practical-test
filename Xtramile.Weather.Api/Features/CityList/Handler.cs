using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xtramile.Weather.Api.DataContext;
using Xtramile.Weather.Api.Helpers;

namespace Xtramile.Weather.Api.Features.CityList
{
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly CountryContext _context;

        public Handler(CountryContext context)
        {
            _context = context;
        }
        
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var items = await _context.Cities
                .Where(x => x.CountryCode == request.CountryCode)
                .Select(x=> new Item
                {
                    Name = x.Name,
                    CountryCode = x.CountryCode,
                    Id = x.Id
                })
                .ToListAsync(cancellationToken);
            if (items.Count == 0) throw new ApiException($"There is no city data for country code {request.CountryCode}", "1404", 404);
            return new Response
            {
                Data = items
            };
        }
    }
}