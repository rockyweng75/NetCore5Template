using Microsoft.AspNetCore.Http;
using NetCore5_Domain.Models;
using NetCore5_MVC.Extensions;

namespace NetCore5_MVC.Wappers
{
    public interface ISessionWapper
    {
        string SessionId { get; }

        UserInfo User { get; set; }

        void ClearSession();

        string SharedSecret { get; set; }


    }

    public class SessionWapper : ISessionWapper
    {
        private static readonly string _userKey = "session.user";
        private static readonly string _sharedSecretKey = "session.shared.secret";

        public SessionWapper(ISession session)
        {
            Session = session;
        }

        private ISession Session
        {
            get; set;
        }

        public string SessionId => Session.Id;

        public void ClearSession()
        {
            Session.Clear();
        }

        public UserInfo User
        {
            get
            {
                return Session.GetObject<UserInfo>(_userKey);
            }
            set
            {
                Session.SetObject(_userKey, value);
            }
        }


        public string SharedSecret
        {
            get
            {
                return Session.GetObject<string>(_sharedSecretKey);
            }
            set
            {
                Session.SetObject(_sharedSecretKey, value);
            }
        }
    }
}
