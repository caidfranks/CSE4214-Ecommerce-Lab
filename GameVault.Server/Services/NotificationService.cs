using GameVault.Server.Models.Firestore;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Google.Cloud.Firestore;

namespace GameVault.Server.Services
{
    public class NotificationService
    {
        private readonly IFirestoreService _firestore;

        public NotificationService(IFirestoreService firestore)
        {
            _firestore = firestore;
        }

        public async Task CreateNotifAsync(string userId, string title, string message)
        {
            var notif = new FirestoreNotification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            await _firestore.AddDocumentAsync("notifications", notif);
        }
    }
}
