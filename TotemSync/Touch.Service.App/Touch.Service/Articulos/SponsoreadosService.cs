using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;
using Touch.Service.Comun;

namespace Touch.Service.Articulos
{
    public class SponsoreadosService : SingleEntityComunService<Sponsoreo>, ISponsoreadosService
    {       
        private readonly ISponsoreoRepository sponsoreoRepository;
        public SponsoreadosService(ISingleEntityComunRepository<Sponsoreo> singleSponsoreoRepository,ISponsoreoRepository sponsoreoRepository) : base (singleSponsoreoRepository)
        {           
            this.sponsoreoRepository = sponsoreoRepository;
        }

        public async Task<IEnumerable<Sponsoreo>> GetFromArticulo(long idArticulo)
        {
            return await sponsoreoRepository.Get(idArticulo);
        }

        public Task<IEnumerable<Sponsoreo>> Get(long idArticulo, DateTime fechaVigente)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Sponsoreo>> Get(long idArticulo, DateTime fechaDesde, DateTime fechaHasta)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Sponsoreo>> Get(long idArticulo, DateTime fechaDesde, DateTime fechaHasta, DateTime horaDesde, DateTime horaHasta)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Sponsoreo>> Get(DateTime fechaVigente)
        {
            return await sponsoreoRepository.Get(fechaVigente);
        }

        public override Task<ServiceResult> Insert(Sponsoreo entity)
        {
            return base.OnInsert(entity);
        }
    }
}
