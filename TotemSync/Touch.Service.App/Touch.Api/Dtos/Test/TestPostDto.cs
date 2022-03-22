using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Test
{
    public class TestPostDto
    {
        [Required]
        public string Nombre { get; set; }
    }
}
