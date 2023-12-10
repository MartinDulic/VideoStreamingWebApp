using AutoMapper;
using BL.Model;
using DAL.Model;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CountryService
    {
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public CountryService(RepositoryFactory repositoryFactory, IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BLCountry>>? GetAll()
        {
            var dalEntities = await _repositoryFactory.CountryRepository.GetAsync();
            return _mapper.Map<IEnumerable<BLCountry>>(dalEntities);
            
        }
        public async Task<BLCountry> GetById(int id)
        {
            var dalEntities = await _repositoryFactory.CountryRepository.GetAsync();
            var dalEntity = dalEntities.FirstOrDefault(g => g.Id == id);
            return _mapper.Map<BLCountry>(dalEntity);
        }

        public async Task Add(BLCountry entity)
        {
            var dalEnitity = _mapper.Map<Country>(entity);
            await _repositoryFactory.CountryRepository.InsertAsync(dalEnitity);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Update(BLCountry entity)
        {
            var existingCountry = await _repositoryFactory.CountryRepository.GetByIDAsync(entity.Id);

            if (existingCountry == null)
            {
                return;
            }

            _mapper.Map(entity, existingCountry);
            await _repositoryFactory.CountryRepository.UpdateAsync(existingCountry);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Delete(int id)
        {
            await _repositoryFactory.CountryRepository.DeleteAsync(id);
            await _repositoryFactory.SaveAsync();
        }
    }
}
