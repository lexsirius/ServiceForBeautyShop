namespace ReservationDesktop
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public User()
        {

        }

        public User(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public static bool operator ==(User user1, User user2)
        {
            return user1?.Login == user2?.Login && user1?.Password == user2?.Password;
        }

        public static bool operator !=(User user1, User user2)
        {
            return user1?.Login != user2?.Login || user1?.Password != user2?.Password;
        }
    }
}