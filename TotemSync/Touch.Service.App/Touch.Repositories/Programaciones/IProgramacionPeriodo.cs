using Touch.Core.Attributes;
using Touch.Core.Programaciones;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Programaciones
{
    [EntityRepository("ProgramacionPeriodo")]
    public interface IProgramacionPeriodo : ISingleEntityComunRepository<ProgramacionPeriodoRepository> 
    {
    }
}
