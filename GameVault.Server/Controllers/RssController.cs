using GameVault.Server.Models;
using GameVault.Server.Models.Firestore;
using GameVault.Server.Services;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace GameVault.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IFirestoreService _firestore;
        private readonly IConfiguration _configuration;

        public NotificationController(IFirestoreService firestore, IConfiguration configuration)
        {
            _firestore = firestore;
            _configuration = configuration;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ListResponse<NotificationDTO>>> GetUserFeed(string userId)
        {
            var items = await _firestore.QueryDocumentsAsync<FirestoreNotification>("notifications", "UserId", userId);

            var feedItems = items
            .OrderByDescending(i => i.Timestamp)
            .Take(50)
            .Select(i =>
                new NotificationDTO {
                    Title = i.Title,
                    Message = i.Message,
                    Timestamp = i.Timestamp
                }).ToList();

            return new ListResponse<NotificationDTO>
            {
                Message = $"Retrieved {feedItems.Count} notifications",
                List = feedItems
            };
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class RssController : ControllerBase
    {
        private readonly IFirestoreService _firestore;

        public RssController(IFirestoreService firestore)
        {
            _firestore = firestore;
        }

        [HttpGet("{userId}")]
        [ResponseCache(Duration = 300)]
        public async Task<IActionResult> Get(string userId)
        {
            var items = await _firestore.QueryDocumentsAsyncWithId<Notification>(
                "notifications", "UserId", userId);

            var feedItems = items
                .OrderByDescending(i => i.Timestamp)
                .Select(i =>
                    new XElement("item",
                        new XElement("title", i.Title),
                        new XElement("description", i.Message),
                        new XElement("pubDate", i.Timestamp.ToUniversalTime().ToString("R")),
                        new XElement("guid", i.Id)
                    ));

            var doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("rss",
                    new XAttribute("version", "2.0"),
                    new XElement("channel",
                        new XElement("title", $"GameVault Notifications for {userId}"),
                        new XElement("link", "https://gamevault.com"),
                        new XElement("description", "User notification RSS feed"),
                        feedItems
                    )));

            return Content(doc.ToString(SaveOptions.DisableFormatting), "application/rss+xml; charset=utf-8");
        }
    }
}