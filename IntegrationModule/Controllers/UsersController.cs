using BL.Model;
using BLL.Services;
using DAL.Model;
using DAL.Requests;
using DAL.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntegrationModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;

        public UsersController(UserService userService, NotificationService notificationService)
        {
            _userService = userService;
            _notificationService = notificationService;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<BLUser>> RegisterUser([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var client = new SmtpClient("127.0.0.1", 25);
            var sender = "admin@my-webapi.com";

            string registrationSubject = "Account registration for RWA webapp";
            string registrationBody = "Folow this URL to validate your account: DUMMY URL";

            var allNotifications = await _notificationService.GetAll();
            var registrationNotifications =
                allNotifications.Where( notif => !notif.SentAt.HasValue && notif.Subject == registrationSubject);

            try
            {
                var newUser = await _userService.AddUser(request);

                var dalNotification = new BLNotification
                {
                    ReceiverEmail = request.Email,
                    Subject = registrationSubject,
                    Body = registrationBody,
                    CreatedAt = DateTime.Now
                };
                await _notificationService.Add(dalNotification);

                var mail = new MailMessage(new MailAddress(sender), new MailAddress(dalNotification.ReceiverEmail));
                mail.Subject = dalNotification.Subject;
                mail.Body = dalNotification.Body;
                client.Send(mail);

                dalNotification.SentAt = DateTime.UtcNow;
                _notificationService.SaveDataAsync();

                return Ok(new UserResponse
                {
                    Id = newUser.Id,
                    SecurityToken = newUser.SecurityToken
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("[action]")]
        public async Task<ActionResult> ValidateUserEmail([FromBody] EmailValidationRequest request)
        {
            try
            {
                await _userService.ConfirmEmail(request);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<Tokens>> JwtTokens([FromBody] JWTTokensRequest request)
        {
            try
            {
                return Ok(await _userService.GetJwtTokens(request));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
