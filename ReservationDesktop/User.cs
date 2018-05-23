using System.Security.Cryptography;
using System.Text;

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

        public User(User user)
        {
            Login = user.Login;
            Password = user.Password;
        }

        public static bool operator ==(User user1, User user2)
        {
            return user1?.Login == user2?.Login && user1?.Password == user2?.Password;
        }

        public static bool operator !=(User user1, User user2)
        {
            return user1?.Login != user2?.Login || user1?.Password != user2?.Password;
        }

        private string EncryptWord(string word)
        {
            MD5 md5Hasher = MD5.Create();
            var dataHash = md5Hasher.ComputeHash(Encoding.Default.GetBytes(word));
            var dataHashLength = dataHash.Length;
            var sBuilder = new StringBuilder();
            for (var i = 0; i < dataHashLength; i++)
            {
                sBuilder.Append(dataHash[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public User EncryptPassword()
        {
            this.Password = this.EncryptWord(this.Password);
            return this;
        }
    }
}