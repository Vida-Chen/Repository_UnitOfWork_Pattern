using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppLibrary
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private static string conStr = null;
		private DBHelper dbSelector;
		private StringBuilder strSql;

		public Repository(string sDBAliasName)
		{
			dbSelector = new DBHelper();
			conStr = dbSelector.GetConnectionString(sDBAliasName);
			strSql = new StringBuilder();
		}

		public void Delete(T entity)
		{
			strSql.Clear();

			try
			{
				using (var cn = new SQLiteConnection(conStr))
				{
					cn.Open();
					SQLiteCommand cmd = new SQLiteCommand(strSql.ToString(), cn);
					Type myLoadClass = entity.GetType();
					string tbName = dbSelector.GetTbName(myLoadClass);
					string keyCondition = dbSelector.GetKeyCondition(entity);

					var keys = entity.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(KeyAttribute), false));
					if (keys.Count() == 0)
					{
						strSql.AppendLine("必須設定Key值");
						throw new Exception(strSql.ToString());
					}
					else
					{
						foreach (var key in keys)
						{
							var keyName = key.Name;
							var keyValue = entity.GetType().GetProperty(keyName).GetValue(entity, null);
							if (keyValue == null)
							{
								strSql.Append("Key值: ").Append(keyName).Append(" 不得為空").AppendLine();
								throw new Exception(strSql.ToString());
							}
							cmd.Parameters.Add(new SQLiteParameter(keyName, keyValue));
						}
						strSql.Append("delete from ").Append(tbName).Append(" where 1=1 ").Append(keyCondition).AppendLine();
						cmd.CommandText = strSql.ToString();
						cmd.ExecuteNonQuery();
						cmd.Parameters.Clear();
					}
				}
			}
			catch (Exception e)
			{
				throw new Exception(strSql.ToString(), e);
			}
		}

		public void Insert(T entity)
		{
			strSql.Clear();
			try
			{
				using (var cn = new SQLiteConnection(conStr))
				{
					cn.Open();
					SQLiteCommand cmd = new SQLiteCommand(strSql.ToString(), cn);
					Type myLoadClass = entity.GetType();
					string tbName = dbSelector.GetTbName(myLoadClass);
					PropertyInfo[] props = myLoadClass.GetProperties();

					string colName = "";
					string colValue = "";
					foreach (var prop in props)
					{
						string tName = prop.Name;
						object tValue = prop.GetValue(entity, null);
						colName += tName + ",";
						colValue += "@" + tName + ",";
						if (tValue != null)
						{
							var vType = tValue.GetType();
							cmd.Parameters.Add(new SQLiteParameter(tName, tValue));
						}
						else
						{
							cmd.Parameters.Add(new SQLiteParameter(tName, DBNull.Value));
						}
					}
					strSql.Append("insert into ").Append(tbName).Append(" (").Append(colName.TrimEnd(',')).Append(") values (").Append(colValue.TrimEnd(',')).Append(")").AppendLine();
					cmd.CommandText = strSql.ToString();
					cmd.ExecuteNonQuery();
					cmd.Parameters.Clear();
				}
			}
			catch (Exception e)
			{
				throw new Exception(strSql.ToString(), e);
			}
		}

		public IList<T> Select(T entity)
		{
			strSql.Clear();
			try
			{
				IList<T> result = new List<T>();
				using (var cn = new SQLiteConnection(conStr))
				{
					cn.Open();
					SQLiteCommand cmd = new SQLiteCommand(strSql.ToString(), cn);
					Type myLoadClass = entity.GetType();
					string tbName = dbSelector.GetTbName(myLoadClass);
					PropertyInfo[] props = myLoadClass.GetProperties();
					string condition = "";
					foreach (var prop in props)
					{
						string tName = prop.Name;
						object tValue = prop.GetValue(entity, null);
						if (tValue != null)
						{
							condition += " and " + tName + " =@" + tName + " ";
							//var vType = tValue.GetType();
							cmd.Parameters.Add(new SQLiteParameter(tName, tValue));
						}
					}

					strSql.Append("select * from ").Append(tbName).Append(" where 1=1 ").Append(condition).AppendLine();
					cmd.CommandText = strSql.ToString();

					List<object> t1 = new List<object>();
					var dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						Dictionary<string, object> tmpDic = new Dictionary<string, object>();
						for (int i = 0; i < dr.FieldCount; i++)
						{
							var val = dr.GetValue(i);
							string key = dr.GetName(i).ToString();
							tmpDic.Add(key, val);
						}
						object tmpObj = Activator.CreateInstance(myLoadClass);
						foreach (var prop in props)
						{
							string tName = prop.Name.ToUpper();
							if (tmpDic.ContainsKey(tName))
							{
								var type = tmpDic[tName].GetType();
								var value = type.Name.Equals("DBNull") ? null : tmpDic[tName];
								prop.SetValue(tmpObj, value);
							}
						}

						if (tmpObj is T)
						{
							var d = (T)Convert.ChangeType(tmpObj, entity.GetType());
							result.Add(d);
						}
					}
					return result;
				}
			}
			catch (Exception e)
			{
				throw new Exception(strSql.ToString(), e);
			}
		}

		public void Update(T entity)
		{
			strSql.Clear();
			try
			{
				using (var cn = new SQLiteConnection(conStr))
				{
					cn.Open();
					SQLiteCommand cmd = new SQLiteCommand(strSql.ToString(), cn);
					Type myLoadClass = entity.GetType();
					string keyCondition = dbSelector.GetKeyCondition(entity);
					string tbName = dbSelector.GetTbName(myLoadClass);
					PropertyInfo[] props = myLoadClass.GetProperties();
					var keys = entity.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(KeyAttribute), false));
					if (keys.Count() == 0)
					{
						strSql.AppendLine("必須設定Key值").AppendLine();
						throw new Exception(strSql.ToString());
					}
					else
					{
						foreach (var key in keys)
						{
							var keyName = key.Name;
							var keyValue = entity.GetType().GetProperty(keyName).GetValue(entity, null);
							if (keyValue == null)
							{
								strSql.Append("Key值: ").Append(keyName).Append(" 不得為空").AppendLine();
								throw new Exception(strSql.ToString());
							}
						}
						string colName = "";
						string colValue = "";
						string updateValue = "";
						foreach (var prop in props)
						{
							string tName = prop.Name;
							object tValue = prop.GetValue(entity, null) == null ? DBNull.Value : prop.GetValue(entity, null);

							updateValue += tName + "=@" + tName + ",";
							colName += tName + ",";
							colValue += "@" + tName + ",";
							cmd.Parameters.Add(new SQLiteParameter(tName, tValue));
						}
						strSql.Append("update ").Append(tbName).Append(" set ").Append(updateValue.Trim(',')).Append(" where 1=1 ").Append(keyCondition).AppendLine();
						cmd.CommandText = strSql.ToString();
						cmd.ExecuteNonQuery();
						cmd.Parameters.Clear();
					}
				}
			}
			catch (Exception e)
			{
				throw new Exception(strSql.ToString(), e);
			}
		}
	}
}
