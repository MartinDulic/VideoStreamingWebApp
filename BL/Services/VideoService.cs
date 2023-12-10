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
    public class ImageService
    {
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public ImageService(RepositoryFactory repositoryFactory, IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BLImage>>? GetAll()
        {
            var dalEntities = await _repositoryFactory.ImageRepository.GetAsync();
            return _mapper.Map<IEnumerable<BLImage>>(dalEntities);
            
        }
        public async Task<BLImage> GetById(int id)
        {
            var dalEntities = await _repositoryFactory.ImageRepository.GetAsync();
            var dalEntity = dalEntities.FirstOrDefault(g => g.Id == id);
            return _mapper.Map<BLImage>(dalEntity);
        }

        public async Task Add(BLImage entity)
        {
            var dalEnitity = _mapper.Map<Image>(entity);
            await _repositoryFactory.ImageRepository.InsertAsync(dalEnitity);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Update(BLImage entity)
        {
            var existingEntity = await _repositoryFactory.ImageRepository.GetByIDAsync(entity.Id);

            if (existingEntity == null)
            {
                return;
            }

            _mapper.Map(entity, existingEntity);
            await _repositoryFactory.ImageRepository.UpdateAsync(existingEntity);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Delete(int id)
        {
            await _repositoryFactory.ImageRepository.DeleteAsync(id);
            await _repositoryFactory.SaveAsync();
        }
    }
}
