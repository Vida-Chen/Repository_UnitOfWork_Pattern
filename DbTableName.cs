using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
	public class DbTableName : Attribute
	{
		string tbName;
		public DbTableName(string tbName)
		{
			this.tbName = tbName;
		}
		public string GetName()
		{
			return tbName;
		}
	}
}
