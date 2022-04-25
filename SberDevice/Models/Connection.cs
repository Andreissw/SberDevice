using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SberDevice.Models
{
    public class Connection
    {
        public static object SelectString(string cmd) //Достает с базы PSIGMA FLAT строковые значения и числовые
        {

            SqlConnection sqlcon = new SqlConnection(@"Data Source=WS150002\SQLEXPRESS; Initial Catalog= FAS; integrated security=True;");
            //SqlConnection sqlcon = new SqlConnection(@"Data Source=traceability\flat; Initial Catalog= FAS; user id=CTS_SOFT;password=wnc_ghju;");
            SqlCommand c = new SqlCommand();
            SqlDataReader r;
            string k = "";
            c = sqlcon.CreateCommand();
            c.CommandType = CommandType.Text;
            c.CommandText = cmd;
            try
            {
                sqlcon.Open();
                r = c.ExecuteReader();
                if (r.Read())
                {
                    k = r.GetString(0);
                    r.Close();
                }

                sqlcon.Dispose();
                return k;
            }


            catch (Exception e)
            {
                return null;
            }

        }
    }
}