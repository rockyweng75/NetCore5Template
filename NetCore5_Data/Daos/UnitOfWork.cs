using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;

namespace NetCore5_Data.Daos
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbConnection dbConnection;
        private TransactionScope dbTransaction;
        private IList<int> tree = new List<int>();


        public UnitOfWork(IDbConnectionFactory dbConnectionFactory)
        {
            dbConnection = dbConnectionFactory.GetDbConnection();
        }


        public void BeginTransaction(int key)
        {
            if (dbTransaction == null)
            {
                dbTransaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            }

            tree.Add(key);
        }

        public void Commit(int key)
        {
            if (tree.Count > 0)
            {
                if (tree.FirstOrDefault() == key)
                {
                    dbTransaction.Complete();
                }
            }
        }

        //public void RollBack(int index)
        //{
        //    dbTransaction.Dispose();
        //}

        IDbConnection IUnitOfWork.dbConnection => dbConnection;

        private bool disposed = false;




        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (dbTransaction != null)
                    {
                        this.dbTransaction.Dispose();
                        this.dbTransaction = null;
                    }
                    if (dbConnection != null)
                    {
                        this.dbConnection.Dispose();
                        this.dbConnection = null;
                    }
                }
            }
            this.disposed = true;
        }


    }
}
