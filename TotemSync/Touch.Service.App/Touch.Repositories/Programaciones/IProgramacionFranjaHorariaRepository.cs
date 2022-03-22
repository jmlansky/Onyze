using Touch.Core.Attributes;
using Touch.Core.Programaciones;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Programaciones
{
    [EntityRepository("ProgramacionFranjaHoraria")]
    public interface IProgramacionFranjaHorariaRepository : ISingleEntityComunRepository<ProgramacionFranjaHoraria>
    {
    }
}
