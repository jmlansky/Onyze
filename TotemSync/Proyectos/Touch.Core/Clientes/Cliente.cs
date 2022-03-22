using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;
using Touch.Core.Sucursales;
using Touch.Core.Usuarios;

namespace Touch.Core.Clientes
{
    [TableName("cliente")]
    public class Cliente: ComunEntity
    {

        [ColumnName("calle")]
        public string Calle { get; set; }

        [ColumnName("numero")]
        public string Numero { get; set; }

        [ColumnName("codigo")]
        public string Codigo { get; set; }

        [ColumnName("piso")]
        public string Piso { get; set; }

        [ColumnName("depto")]
        public string Depto { get; set; }

        [ColumnName("telefono")]
        public string Telefono { get; set; }

        [ColumnName("mail")]
        public string Mail { get; set; }

        [ColumnName("nombre_referente")]
        public string NombreReferente { get; set; }

        [ColumnName("cargo_referente")]
        public string CargoReferente { get; set; }



        [ColumnName("id_barrio")]
        public long? IdBarrio { get; set; }

        [ColumnName("id_localidad")]
        public long? idLocalidad { get; set; }

        [ColumnName("id_provincia")]
        public long? idProvincia { get; set; }

        public Barrio Barrio { get; set; }

        public Localidad Localidad { get; set; }

        public Provincia Provincia { get; set; }

        public List<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public List<Sucursal> Sucursales { get; set; } = new List<Sucursal>();
    }
}
