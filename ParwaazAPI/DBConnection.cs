using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ParwaazAPI
{
    public class DBConnection
    {
        public SqlConnection GetDBConnection()
        {
            string strConn = "";

            strConn = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection conn = new SqlConnection(strConn);
            return conn;
        }
    }
}