using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
	public class DBHelper
	{
		public static string TESTDB = "test.sqlite";
		public static string PROJECTDB = "projtest.sqlite";

		public void initFirst()
		{
			using (var cn = new SQLiteConnection(this.GetConnectionString(DBHelper.PROJECTDB)))
			{
				cn.Open();
				StringBuilder sql = new StringBuilder();
				SQLiteCommand cmd = new SQLiteCommand(sql.ToString(), cn);

				//Create Project table
				string[] colNames = new string[] { "PID", "PROJ_NO", "PROJ_NAME", "PROJ_DESC", "PROJ_OWNER", "PROJ_BENEFIT" };
				string[] colTypes = new string[] { "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT" };
				string tbName = "tb_Project";
				sql.Append("CREATE TABLE IF NOT EXISTS ").Append(tbName).Append("( ").Append(colNames[0]).Append(" ").Append(colTypes[0]);
				for (int idx = 1; idx < colNames.Length; idx++)
				{
					sql.Append(", ").Append(colNames[idx]).Append(" ").Append(colTypes[idx]);
				}
				sql.Append(" )");
				cmd.CommandText = sql.ToString();
				cmd.ExecuteNonQuery();

				//Create Milestone table
				sql.Clear();
				colNames = new string[] { "MID", "MILESTONE_NAME", "MILESTONE_OWNER", "OUTPUT", "STARTTIME", "ENDTIME", "REF_PID", "REF_PROJ_NO" };
				colTypes = new string[] { "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT" };
				tbName = "tb_Milestone";
				sql.Append("CREATE TABLE IF NOT EXISTS ").Append(tbName).Append("( ").Append(colNames[0]).Append(" ").Append(colTypes[0]);
				for (int idx = 1; idx < colNames.Length; idx++)
				{
					sql.Append(", ").Append(colNames[idx]).Append(" ").Append(colTypes[idx]);
				}
				sql.Append(" )");
				cmd.CommandText = sql.ToString();
				cmd.ExecuteNonQuery();
			}
		}
		public string GetConnectionString(string sDBAliasName)
		{
			string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sDBAliasName);
			string connStr = string.Format("data source = {0}", dbPath);
			return connStr;
		}
		public string GetTbName(Type objType)
		{
			string tbName = "";

			DbTableName dbTbName = (DbTableName)Attribute.GetCustomAttribute(objType, typeof(DbTableName));
			if (dbTbName != null && string.IsNullOrEmpty(dbTbName.GetName()) == false)
			{
				tbName = dbTbName.GetName();
			}
			else
			{
				throw new Exception(objType.FullName + " DbTableName is undefined!");
			}
			return tbName;
		}
		public string GetKeyCondition(object entity)
		{
			string keyCondition = "";

			var keys = entity.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(KeyAttribute), false));
			foreach (var item in keys)
			{
				var key = item.Name;
				var value = item.GetValue(entity);
				keyCondition += " and " + key + "=@" + key;
			}
			return keyCondition;
		}
	}
}
