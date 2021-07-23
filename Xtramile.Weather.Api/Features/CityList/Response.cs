using System.Collections;
using System.Collections.Generic;

namespace Xtramile.Weather.Api.Features.CityList
{
    public class Response : BaseResponse
    {
        public IEnumerable<Item> Data { get; set; }
    }
    
    public class Item
    {
        public string CountryCode { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }
}