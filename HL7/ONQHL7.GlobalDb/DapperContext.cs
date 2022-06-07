using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ONQHL7.GlobalDb
{
	public class DapperContext
	{
		private readonly IConfiguration m_configuration;
		private readonly string m_connectionString;

		public DapperContext(IConfiguration a_mConfiguration)
		{
			m_configuration = a_mConfiguration;
			m_connectionString = m_configuration.GetConnectionString("SqlOhConnection");
		}

		public IDbConnection CreateConnection() => new SqlConnection(m_connectionString);
	}
}
