using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Xtramile.Weather.Api.DataContext
{
    public class Country
    {
        [Key]
        [StringLength(2)]
        public string Code { get; set; }
        [StringLength(64)]
        public string Name { get; set; }

        public IEnumerable<City> Cities { get; set; }
    }
}