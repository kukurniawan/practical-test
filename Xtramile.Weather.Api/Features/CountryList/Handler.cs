using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Xtramile.Weather.Api.DataContext;
using Xtramile.Weather.Api.Helpers;

namespace Xtramile.Weather.Api.Features.CountryList
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
            var items = await _context.Countries.Select(x=> new Item
            {
                Code = x.Code, Name = x.Name
            }).ToListAsync(cancellationToken);
            if (items.Count == 0) throw new ApiException($"Country data is empty", "0404", 404);
            return new Response
            {
                Data = items
            };
        }
    }
}