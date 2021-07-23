using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Xtramile.Weather.Api.DataContext
{
    public class City
    {
        
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [StringLength(2)]
        public string CountryCode { get; set; }
        [StringLength(64)]
        public string Name { get; set; }
        public Country Country { get; set; }
    }
}