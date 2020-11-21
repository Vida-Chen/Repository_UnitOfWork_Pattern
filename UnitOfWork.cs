using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
	public class UnitOfWork : IUnitOfWork
	{
		private static string conStr = null;
		public SQLiteTransaction transaction { get; set; }
		public SQLiteConnection connection { get; set; }
		public UnitOfWork(string sDBAliasName)
		{
			conStr = new DBHelper().GetConnectionString(sDBAliasName);
			connection = new SQLiteConnection(conStr);
		}
		public void BeginTransaction()
		{
			connection.Open();
			transaction = connection.BeginTransaction();
		}

		public void Commit()
		{
			transaction.Commit();
			connection.Dispose();
		}

		public void Rollback()
		{
			transaction.Rollback();
			connection.Dispose();
		}
	}
}
