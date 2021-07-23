using System.Collections.Generic;

namespace Xtramile.Weather.Api.Features.CountryList
{
    public class Response : BaseResponse
    {
        public IEnumerable<Item> Data { get; set; }
    }

    public class Item
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}