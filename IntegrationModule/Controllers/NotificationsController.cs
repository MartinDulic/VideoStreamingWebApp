using BL.Model;
using BLL.Services;
using DAL.Requests;
using DAL.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IntegrationModule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/<NotificationsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationResponse>>> GetAllNotifications()
        {
            try
            {
                var allNotifications = await _notificationService.GetAll();

                return Ok(allNotifications.Select(dbNotification =>
                    new NotificationResponse
                    {
                        Id = dbNotification.Id,
                        ReceiverEmail = dbNotification.ReceiverEmail,
                        Subject = dbNotification.Subject,
                        Body = dbNotification.Body

                    }
                ));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET api/<NotificationsController>/5
        [HttpGet("[action]")]
        public async Task<ActionResult<NotificationResponse>> GetNotification(int id)
        {
            try
            {
                var dalNotification = await _notificationService.GetById(id);

                if (dalNotification == null) return NotFound();

                return Ok(new NotificationResponse
                {
                    Id = dalNotification.Id,
                    ReceiverEmail = dalNotification.ReceiverEmail,
                    Subject = dalNotification.Subject,
                    Body = dalNotification.Body
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<NotificationResponse>> CreateNotification(NotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var dalNotification = new BLNotification
                {
                    ReceiverEmail = request.ReceiverEmail,
                    Subject = request.Subject,
                    Body = request.Body,
                    CreatedAt = DateTime.Now
                };
                await _notificationService.Add(dalNotification);

                return Ok(new NotificationResponse
                {
                    Id = dalNotification.Id,
                    ReceiverEmail = dalNotification.ReceiverEmail,
                    Subject = dalNotification.Subject,
                    Body = dalNotification.Body
                });

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // PUT api/<NotificationsController>/5
        [HttpPut("[action]")]
        public async Task<ActionResult<NotificationResponse>> ModifyNotification(int id, [FromBody] NotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var dalNotification = await _notificationService.GetById(id);
                if (dalNotification == null) return NotFound();

                dalNotification.ReceiverEmail = request.ReceiverEmail;
                dalNotification.Subject = request.Subject;
                dalNotification.Body = request.Body;

                await _notificationService.Update(dalNotification);
                return Ok(new NotificationResponse
                {
                    Id = dalNotification.Id,
                    ReceiverEmail = dalNotification.ReceiverEmail,
                    Subject = dalNotification.Subject,
                    Body = dalNotification.Body
                });

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        // DELETE api/<NotificationsController>/5
        [HttpDelete("[action]")]
        public async Task<ActionResult<NotificationResponse>> RemoveNotification(int id)
        {
            try
            {
                var dalNotification = await _notificationService.GetById(id);
                if (dalNotification == null) return NotFound();

                await _notificationService.Delete(id);
                return Ok(new NotificationResponse
                {
                    Id = dalNotification.Id,
                    ReceiverEmail = dalNotification.ReceiverEmail,
                    Subject = dalNotification.Subject,
                    Body = dalNotification.Body
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> SendAllNotifications()
        {
            var client = new SmtpClient("127.0.0.1", 25);
            var sender = "admin@my-webapi.com";

            try
            {
                var allNotifications = await _notificationService.GetAll();
                var unsentNotifications = allNotifications.Where(x => !x.SentAt.HasValue);

                foreach (var notification in unsentNotifications)
                {
                    try
                    {
                        var mail = new MailMessage(new MailAddress(sender), new MailAddress(notification.ReceiverEmail));

                        mail.Subject = notification.Subject;
                        mail.Body = notification.Body;

                        client.Send(mail);

                        notification.SentAt = DateTime.UtcNow;

                        // Save the updated notification to the database
                        await _notificationService.Update(notification);

                    }
                    catch (Exception e)
                    {
                        await Console.Out.WriteLineAsync(e.ToString());
                    }
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetNumberOfUnsentNotifications()
        {
            int unsent = 0;
            var allNotification = await _notificationService.GetAll();

            foreach (var item in allNotification)
            {
                if (item.SentAt is null)
                {
                    unsent++;
                }
            }

            return Ok(unsent);
        }
    }
}