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
    public class GenreService
    {
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public GenreService(RepositoryFactory repositoryFactory, IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BLGenre>>? GetAll()
        {
            var dalEntities = await _repositoryFactory.GenreRepository.GetAsync();
            return _mapper.Map<IEnumerable<BLGenre>>(dalEntities);
            
        }
        public async Task<BLGenre> GetById(int id)
        {
            var dalEntities = await _repositoryFactory.GenreRepository.GetAsync();
            var dalEntity = dalEntities.FirstOrDefault(g => g.Id == id);
            return _mapper.Map<BLGenre>(dalEntity);

        }

        public async Task<BLGenre> GetByName(string name)
        {
            var dalEnitities = await _repositoryFactory.GenreRepository.GetAsync();
            var dalEntity = dalEnitities.FirstOrDefault(g => g.Name == name);
            return _mapper.Map<BLGenre>(dalEntity);
        }

        public async Task Add(BLGenre entity)
        {
            var dalEntity = _mapper.Map<Genre>(entity);
            await _repositoryFactory.GenreRepository.InsertAsync(dalEntity);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Update(BLGenre entity)
        {
            var existingEntity = await _repositoryFactory.GenreRepository.GetByIDAsync(entity.Id);

            if (existingEntity == null)
            {
                return;
            }

            _mapper.Map(entity, existingEntity);
            await _repositoryFactory.GenreRepository.UpdateAsync(existingEntity);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Delete(int id)
        {
            await _repositoryFactory.GenreRepository.DeleteAsync(id);
            await _repositoryFactory.SaveAsync();
        }
    }
}
