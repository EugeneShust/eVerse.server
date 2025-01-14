using FirebaseAdmin.Messaging;

namespace Core.Firebase
{
    public class FirebaseService
    {
        private readonly FirebaseMessaging _messaging;

        public FirebaseService(FirebaseMessaging messaging)
        {
            _messaging = messaging;
        }

        public async Task<string> SendNotificationAsync(string token, string title, string body)
        {
            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };

            try
            {
                var response = await _messaging.SendAsync(message);
                Console.WriteLine($"Successfully sent message: {response}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                throw;
            }
        }

        //public async Task<string> SendMulticastNotificationAsync(List<string> tokens, string title, string body)
        //{
        //    var message = new MulticastMessage
        //    {
        //        Tokens = tokens,
        //        Notification = new Notification
        //        {
        //            Title = title,
        //            Body = body
        //        }
        //    };

        //    try
        //    {
        //        var response = await _messaging.SendMulticastAsync(message);
        //        Console.WriteLine($"Successfully sent {response.SuccessCount} messages.");
        //        return $"Successfully sent {response.SuccessCount} messages.";
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error sending messages: {ex.Message}");
        //        throw;
        //    }
        //}
    }
}
