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
    public class NotificationService
    {
        private readonly RepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public NotificationService(RepositoryFactory repositoryFactory, IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BLNotification>> GetAll()
        {
            var dalEntities = await _repositoryFactory.NotificationRepository.GetAsync();
            return _mapper.Map<IEnumerable<BLNotification>>(dalEntities);

        }
        public async Task<BLNotification> GetById(int id)
        {
            var dalEntities = await _repositoryFactory.NotificationRepository.GetAsync();
            var dalEntity = dalEntities.FirstOrDefault(g => g.Id == id);
            return _mapper.Map<BLNotification>(dalEntity);
        }

        public async Task Add(BLNotification entity)
        {
            var dalEnitity = _mapper.Map<Notification>(entity);
            await _repositoryFactory.NotificationRepository.InsertAsync(dalEnitity);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Update(BLNotification entity)
        {
            var existingEntity = await _repositoryFactory.NotificationRepository.GetByIDAsync(entity.Id);

            if (existingEntity == null)
            {
                return;
            }

            _mapper.Map(entity, existingEntity);
            await _repositoryFactory.NotificationRepository.UpdateAsync(existingEntity);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Delete(int id)
        {
            await _repositoryFactory.NotificationRepository.DeleteAsync(id);
            await _repositoryFactory.SaveAsync();
        }
        public async void SaveDataAsync() => await _repositoryFactory.SaveAsync();
    }
}
