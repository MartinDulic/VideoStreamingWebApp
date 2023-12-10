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
    public class VideoService
    {
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public VideoService(RepositoryFactory repositoryFactory, IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BLVideo>>? GetAllWithoutProperties()
        {
            var dalEntities = await _repositoryFactory.VideoRepository.GetAsync();
            return _mapper.Map<IEnumerable<BLVideo>>(dalEntities);
        }
        public async Task<IEnumerable<BLVideo>>? GetAll()
        {
            var dalEntities = await _repositoryFactory.VideoRepository.GetAsync(includeProperties: "Genre,Image,VideoTags,VideoTags.Tag");
            return _mapper.Map<IEnumerable<BLVideo>>(dalEntities);
        }
        public async Task<BLVideo> GetById(int id)
        {
            var dalEntities = await _repositoryFactory.VideoRepository.GetAsync(includeProperties: "Genre,Image,VideoTags,VideoTags.Tag");
            var dalEntity = dalEntities.FirstOrDefault(g => g.Id == id);
            return _mapper.Map<BLVideo>(dalEntity);
        }

        public async Task Add(BLVideo entity)
        {
            var dalEnitity = _mapper.Map<Video>(entity);
            await _repositoryFactory.VideoRepository.InsertAsync(dalEnitity);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Update(BLVideo entity)
        {
            var existingEntity = await _repositoryFactory.VideoRepository.GetByIDAsync(entity.Id);

            if (existingEntity == null)
            {
                return;
            }

            _mapper.Map(entity, existingEntity);
            await _repositoryFactory.VideoRepository.UpdateAsync(existingEntity);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Delete(int id)
        {
            await _repositoryFactory.VideoRepository.DeleteAsync(id);
            await _repositoryFactory.SaveAsync();
        }
    }
}
