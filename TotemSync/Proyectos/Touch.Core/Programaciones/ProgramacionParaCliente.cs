using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Programaciones
{
    [TableName("programaciones_clientes")]
    public class ProgramacionParaCliente : ComunEntity
    {
        [ColumnName("id_cliente")]
        public long IdCliente { get; set; }

        [ColumnName("id_programacion")]
        public long IdProgramacion { get; set; }



        public string Calle { get; set; }


        public string Numero { get; set; }


        public string Codigo { get; set; }


        public string Piso { get; set; }

        public string Depto { get; set; }


        public string Telefono { get; set; }


        public string Mail { get; set; }


        public string NombreReferente { get; set; }


        public string CargoReferente { get; set; }

        public Barrio Barrio { get; set; }

        public Localidad Localidad { get; set; }
    }


}
