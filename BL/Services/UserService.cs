using AutoMapper;
using BL.Model;
using DAL.Model;
using DAL.Repository;
using DAL.Requests;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserService
    {

        private readonly RepositoryFactory _repositoryFactory;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserService(RepositoryFactory repositoryFactory, IConfiguration configuration, IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _config = configuration;
            _mapper = mapper;
        }


        public async Task<IEnumerable<BLUser>> GetAll()
        {
            var dalUsers = await _repositoryFactory.UserRepository.GetAsync(includeProperties: "CountryOfResidence");
            var blUsers = _mapper.Map<IEnumerable<BLUser>>(dalUsers);
            return blUsers;
        }

        public async Task<BLUser> GetById(int id)
        {
            var dalUsers = await _repositoryFactory.UserRepository.GetAsync(includeProperties: "CountryOfResidence");
            var dalUser = dalUsers.FirstOrDefault(u => u.Id == id);
            var blUser = _mapper.Map<BLUser>(dalUser);
            return blUser;
        }


        public async Task<BLUser> Add(UserRequest request)
        {
            var dalUsers = await _repositoryFactory.UserRepository.GetAsync();
            var blUsers = _mapper.Map<IEnumerable<BLUser>>(dalUsers);

            var username = request.Username.ToLower().Trim();
            if (blUsers.Any(u => u.Username.Equals(username))) 
                throw new InvalidOperationException("Username already exists");

            // Generate salt (random numbers that help protect against rainbow table attacks)
            byte[] passwordSalt = RandomNumberGenerator.GetBytes(16);
            string base64Salt = Convert.ToBase64String(passwordSalt);

            // Generate hash (password + salt crypted with hash function)
            byte[] passwordHash =
                KeyDerivation.Pbkdf2(
                    password: request.Password,
                    salt: passwordSalt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 32);
            string base64Hash = Convert.ToBase64String(passwordHash);



            // Generates security token (a random 32 bit number)
            byte[] securityToken = RandomNumberGenerator.GetBytes(32);
            string base64SecToken = Convert.ToBase64String(securityToken);

            int nextId = 1;
            if (blUsers.Any())
            {
                nextId = blUsers.Max(u => u.Id) + 1;
            }

            var newUser = new User
            {
                CreatedAt = DateTime.Now,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email,
                Phone = request.Phone,
                IsConfirmed = false,
                SecurityToken = base64SecToken,
                PwdSalt = base64Salt,
                PwdHash = base64Hash,
                CountryOfResidenceId = request.Country
            };


            await _repositoryFactory.UserRepository.InsertAsync(newUser);
            await _repositoryFactory.SaveAsync();
            var user = _mapper.Map<BLUser>(newUser);

            return user;
        }
        public async Task ConfirmEmail(string email, string securityToken)
        {
            var allUsers = await _repositoryFactory.UserRepository.GetAsync();
            var targetUser = allUsers.FirstOrDefault(user => user.Email == email && user.SecurityToken == securityToken);

            if (targetUser == null) throw new InvalidOperationException("Authentication failed");

            targetUser.IsConfirmed = true;
            await _repositoryFactory.UserRepository.UpdateAsync(targetUser);
            await _repositoryFactory.SaveAsync();
        }

        public async Task ConfirmEmail(EmailValidationRequest request)
        {
            var allUsers = await _repositoryFactory.UserRepository.GetAsync();
            var targetUser = allUsers.FirstOrDefault(user =>
                user.Username == request.Username && user.SecurityToken == request.Base64SecurityToken);

            if (targetUser == null) throw new InvalidOperationException("Authentication failed");
                        
            targetUser.IsConfirmed = true;
            await _repositoryFactory.UserRepository.UpdateAsync(targetUser);
            await _repositoryFactory.SaveAsync();
        }

        public async Task<Tokens> GetJwtTokens(JWTTokensRequest request)
        {
            var isAuthenticated = await Authenticate(request?.Username, request?.Password);
            if (!isAuthenticated) throw new InvalidOperationException("Authentication failed");

            // Create byte array from defined key 
            var jwtKeyBytes = Encoding.UTF8.GetBytes(_config["JWT:Key"]);

            // Create a token descriptor (token Template)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                }),
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(jwtKeyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // Create token using that descriptor, serialize it and return it
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);

            return new Tokens
            {
                Token = serializedToken
            };
        }

        public async Task Update(BLUser freshUser)
        {
            var dalUsers = await _repositoryFactory.UserRepository.GetAsync(includeProperties: "CountryOfResidence");
            var existingUser = dalUsers.FirstOrDefault(u => u.Id == freshUser.Id);

            if (existingUser == null) return;

            var countryExists = await _repositoryFactory.CountryRepository.GetByIDAsync(freshUser.CountryOfResidenceId);
            if (countryExists == null) throw new Exception($"Country with ID {freshUser.CountryOfResidenceId} does not exist");

            existingUser.Id = freshUser.Id;
            existingUser.Username = freshUser.Username;
            existingUser.FirstName = freshUser.FirstName;
            existingUser.LastName = freshUser.LastName;
            existingUser.Email = freshUser.Email;
            existingUser.Phone = freshUser.Phone;
            existingUser.IsConfirmed = freshUser.IsConfirmed;
            existingUser.CountryOfResidenceId = freshUser.CountryOfResidenceId;
            existingUser.DeletedAt = freshUser.DeletedAt;

            await _repositoryFactory.UserRepository.UpdateAsync(existingUser);
            await _repositoryFactory.SaveAsync();
        }

        public async Task Delete(int id)
        {
            await _repositoryFactory.UserRepository.DeleteAsync(id);
            await _repositoryFactory.SaveAsync();
        }


        public async Task ChangePassword(string username, string newPassword)
        {
            var allUsers = await _repositoryFactory.UserRepository.GetAsync();
            var targetUser = allUsers.FirstOrDefault(user => user.Username == username &&
                !user.DeletedAt.HasValue);

            if (targetUser == null) throw new InvalidOperationException($"User with username '{username}' doesent exist");

            var newPasswordSalt = RandomNumberGenerator.GetBytes(16);
            var base64NewPasswordSalt = Convert.ToBase64String(newPasswordSalt);

            byte[] newPasswordHash =
                KeyDerivation.Pbkdf2(
                    password: newPassword,
                    salt: newPasswordSalt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 32);
            string base64NewPasswordHash = Convert.ToBase64String(newPasswordHash);

            targetUser.PwdHash = base64NewPasswordHash;
            targetUser.PwdSalt = base64NewPasswordSalt;

            await _repositoryFactory.SaveAsync();
        }

        public async Task<BLUser?> GetConfirmedUser(string username, string password)
        {
            var dalUsers = await _repositoryFactory.UserRepository.GetAsync(includeProperties: "CountryOfResidence");
            var dalUser = dalUsers.FirstOrDefault(user => user.Username == username && user.IsConfirmed == true);

            if (dalUser == null) return null;

            var salt = Convert.FromBase64String(dalUser.PwdSalt);
            byte[] calculatedPasswordHash =
                           KeyDerivation.Pbkdf2(
                               password: password,
                               salt: salt,
                               prf: KeyDerivationPrf.HMACSHA256,
                               iterationCount: 100000,
                               numBytesRequested: 32);

            string base64PasswordHash = Convert.ToBase64String(calculatedPasswordHash);

            if (dalUser.PwdHash != base64PasswordHash) return null;

            var blUser = _mapper.Map<BLUser>(dalUser);
            return blUser;
        }

        private async Task<bool> Authenticate(string username, string password)
        {
            try
            {
                var allUsers = await _repositoryFactory.UserRepository.GetAsync();
                var targetUser = allUsers.Single(u => u.Username == username);

                if (!targetUser.IsConfirmed) return false;

                byte[] storedPasswordSalt = Convert.FromBase64String(targetUser.PwdSalt);
                byte[] storedPasswordHash = Convert.FromBase64String(targetUser.PwdHash);

                // Calculate hase based on passed password
                byte[] calculatedPasswordHash =
                    KeyDerivation.Pbkdf2(
                        password: password,
                        salt: storedPasswordSalt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 32);

                return storedPasswordHash.SequenceEqual(calculatedPasswordHash);
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
