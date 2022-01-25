using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NetCore5_Data.Daos
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction(int key);

        void Commit(int key);


        IDbConnection dbConnection { get; }

    }
}
