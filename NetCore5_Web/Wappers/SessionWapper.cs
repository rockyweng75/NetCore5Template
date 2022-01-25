using Microsoft.AspNetCore.Http;
using NetCore5_Domain.Models;
using NetCore5_Web.Extensions;

namespace NetCore5_Web.Wappers
{
    public interface ISessionWapper
    {
        string SessionId { get; }

        UserInfo User { get; set; }

        void ClearSession();

    }

    public class SessionWapper : ISessionWapper
    {
        private static readonly string _userKey = "session.user";
        private static readonly string _testUserKey = "session.test.user";

        //private readonly IHttpContextAccessor _httpContextAccessor;

        //public SessionWapper(IHttpContextAccessor httpContextAccessor)
        //{
        //    _httpContextAccessor = httpContextAccessor;
        //}

        public SessionWapper(ISession session)
        {
            Session = session;
        }

        private ISession Session
        {
            //get
            //{
            //    return _httpContextAccessor.HttpContext.Session;
            //}
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

    }
}
