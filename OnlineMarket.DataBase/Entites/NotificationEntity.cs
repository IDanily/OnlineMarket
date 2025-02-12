namespace OnlineMarket.DataBase.Entites
{
    public class NotificationEntity
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public UsersEntity User { get; set; }
    }

}
