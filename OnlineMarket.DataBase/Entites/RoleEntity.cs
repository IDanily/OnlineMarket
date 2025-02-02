namespace OnlineMarket.DataBase.Entites
{
    public class RoleEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UsersEntity> Users { get; set; }
        public RoleEntity()
        {
            Users = new List<UsersEntity>();
        }
    }
}
