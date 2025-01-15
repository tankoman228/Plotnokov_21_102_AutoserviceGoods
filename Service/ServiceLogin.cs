using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plotnokov_21_102_AutoserviceGoods.DB;

namespace Plotnokov_21_102_AutoserviceGoods.Service
{
    internal class ServiceLogin
    {
        /// <summary>
        /// Current user, if null => guest mode
        /// </summary>
        public static User CurrentUser { get; set; }

        public static bool Login(string login, string password)
        {
            using (var db = new DB.DB())
            {
                var usr = db.User.Where(x => x.UserLogin == login).FirstOrDefault();
                if (usr == null) return false;
                if (usr.UserPassword != password) return false;

                CurrentUser = usr;
            }

            return true;
        }
    }
}
