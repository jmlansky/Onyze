using Framework.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Totems
{
    [TableName("programaciones_franjas_horarias")]
    public class FranjaHoraria: ComunEntity
    {
        [ColumnName("hora_desde")]
        public string HoraDesde { get; set; }

        [ColumnName("hora_hasta")]
        public string HoraHasta { get; set; }
    }
}