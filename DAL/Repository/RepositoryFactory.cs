using DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class RepositoryFactory : IDisposable
    {
        private readonly DbContext context = new RwaProjectDatabaseContext();
        private bool disposed = false;

        private GenericRepository<Video>? videoRepository;
        private GenericRepository<Genre>? genreRepository;
        private GenericRepository<Tag>? tagRepository;
        private GenericRepository<Notification>? notificationRepository;
        private GenericRepository<Country>? countryRepository;
        private GenericRepository<User>? userRepository;
        private GenericRepository<Image>? imageRepository;
        private GenericRepository<VideoTag>? videoTagRepository;

        public GenericRepository<Video> VideoRepository
        {
            get
            {
                if (videoRepository == null)
                {
                    videoRepository = new GenericRepository<Video>(context);
                }
                return videoRepository;
            }
        }

        public GenericRepository<Genre> GenreRepository
        {
            get 
            { 
                if (genreRepository == null)
                {
                    genreRepository = new GenericRepository<Genre>(context);
                }
                return genreRepository;
            }
        }

        public GenericRepository<Tag> TagRepository
        {
            get
            {
                if (tagRepository == null)
                {
                    tagRepository = new GenericRepository<Tag>(context);
                }
                return tagRepository;
            }
        }

        public GenericRepository<Notification> NotificationRepository
        {
            get
            {
                if (notificationRepository == null)
                {
                    notificationRepository = new GenericRepository<Notification>(context);
                }
                return notificationRepository;
            }
        }

        public GenericRepository<Country> CountryRepository
        {
            get
            {
                if (countryRepository == null)
                {
                    countryRepository = new GenericRepository<Country>(context);
                }
                return countryRepository;
            }
        }

        public GenericRepository<User> UserRepository
        {
            get 
            {
                if (userRepository == null)
                {
                        userRepository = new GenericRepository<User>(context);
                }
                return userRepository;
            }
        }

        public GenericRepository<Image> ImageRepository
        {
            get
            {
                if (imageRepository == null)
                {
                    imageRepository = new GenericRepository<Image>(context);
                }
                return imageRepository;
            }
        }

        public GenericRepository<VideoTag> VideoTagRepository
        {
            get
            {
                if (videoTagRepository == null)
                {
                    videoTagRepository = new GenericRepository<VideoTag>(context);
                }
                return videoTagRepository;
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await context?.SaveChangesAsync();

            }
            catch (DbUpdateException dbe)
            {
                await Console.Out.WriteLineAsync(dbe.InnerException.Message);
                throw;
            }        
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context?.Dispose();
                }
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
