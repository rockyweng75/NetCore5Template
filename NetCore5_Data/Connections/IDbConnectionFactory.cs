using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NetCore5_Data.Daos
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetDbConnection();
    }
}
