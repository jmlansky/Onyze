using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Programaciones;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Programaciones
{
    public class ProgramacionFranjaHorariaRepository : SingleEntityComunRepository<ProgramacionFranjaHoraria>, IProgramacionFranjaHorariaRepository
    {
        private readonly string[] columnsToIgnore = { "Tipo" };
        public ProgramacionFranjaHorariaRepository(IConfiguration configuration) : base(configuration)
        {
        }

     

 
    }
}
