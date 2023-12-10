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
    public class TagService
    {
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public TagService(RepositoryFactory repositoryFactory, IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BLTag>>? GetAll()
        {
            var dalEntities = await _repositoryFactory.TagRepository.GetAsync();
            return _mapper.Map<IEnumerable<BLTag>>(dalEntities);
            
        }
        public async Task<BLTag> GetById(int id)
        {
            var dalEntities = await _repositoryFactory.TagRepository.GetAsync();
            var dalEntity = dalEntities.FirstOrDefault(g => g.Id == id);
            return _mapper.Map<BLTag>(dalEntity);

        }

        public async Task<BLTag> GetByName(string name)
        {
            var dalEnitities = await _repositoryFactory.TagRepository.GetAsync();
            var dalEntity = dalEnitities.FirstOrDefault(g => g.Name == name);
            return _mapper.Map<BLTag>(dalEntity);
        }

        public async Task Add(BLTag entity)
        {
            var dalEntity = _mapper.Map<Tag>(entity);
            await _repositoryFactory.TagRepository.InsertAsync(dalEntity);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Update(BLTag entity)
        {
            var existingEntity = await _repositoryFactory.TagRepository.GetByIDAsync(entity.Id);

            if (existingEntity == null)
            {
                return;
            }

            _mapper.Map(entity, existingEntity);
            await _repositoryFactory.TagRepository.UpdateAsync(existingEntity);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Delete(int id)
        {
            await _repositoryFactory.TagRepository.DeleteAsync(id);
            await _repositoryFactory.SaveAsync();
        }
    }
}
