using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Clientes;
using Touch.Core.Comun;

namespace Touch.Core.Sucursales
{
    [TableName("sucursal")]
    public class Sucursal : ComunEntity
    {
        [ColumnName("id_cliente")]
        public long IdCliente { get; set; }

        [ColumnName("calle")]
        public string Calle { get; set; }

        [ColumnName("altura")]
        public string Altura { get; set; }

        [ColumnName("codigo_postal")]
        public string CodigoPostal { get; set; }

        [ColumnName("telefono")]
        public string Telefono { get; set; }

        [ColumnName("nombre_referente")]
        public string NombreReferente { get; set; }

        [ColumnName("telefono_referente")]
        public string TelefonoReferente { get; set; }

        [ColumnName("cargo_referente")]
        public string RargoReferente { get; set; }

        [ColumnName("casa_central")]
        public bool CasaCentral { get; set; }


        [ColumnName("id_barrio")]
        public long? IdBarrio { get; set; }

        [ColumnName("id_localidad")]
        public long? IdLocalidad { get; set; }

        [ColumnName("id_provincia")]
        public long? IdProvincia { get; set; }

        [ColumnName("id_region")]
        public long? IdRegion { get; set; }

        [ColumnName("id_zona")]
        public long? IdZona { get; set; }


        public Barrio Barrio { get; set; }

        public Localidad Localidad { get; set; }
        public Provincia Provincia { get; set; }

        public Cliente Cliente { get; set; }

        public Zona Zona { get; set; }

        public Region Region { get; set; }
    }
}
