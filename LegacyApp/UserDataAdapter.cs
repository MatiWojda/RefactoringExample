namespace LegacyApp
{
    public class UserDataAdapter : IUserRepository
    {
        public void AddUser(User user)
        {
            UserDataAccess.AddUser(user);
        }
    }
}