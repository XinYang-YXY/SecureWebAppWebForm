using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecureWebAppWebForm
{
    public partial class Success : System.Web.UI.Page
    {
        string MYDBConnectionString =
       System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        byte[] creditCardNum = null;
        byte[] creditCardPin = null;
        byte[] creditCardExpireDate = null;
        string userID = null;



        protected void Page_Load(object sender, EventArgs e)
        {
         
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("login.aspx", false);
                }
                else
                {
                    userID = (string)Session["LoggedIn"];
                    displayUserProfile(userID);
                }
            }
            else
            {
                Response.Redirect("login.aspx", false);
            }

        }

        protected void displayUserProfile(string userid)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT * FROM Account WHERE email=@userId";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userId", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if(reader["creditCardNum"] != DBNull.Value)
                        {
                            creditCardNum = Convert.FromBase64String(reader["creditCardNum"].ToString());
                        }
                        if(reader["creditCardPin"] != DBNull.Value)
                        {
                            creditCardPin = Convert.FromBase64String(reader["creditCardPin"].ToString());
                        }
                        if(reader["creditCardExpireDate"] != DBNull.Value)
                        {
                            creditCardExpireDate = Convert.FromBase64String(reader["creditCardExpireDate"].ToString());
                        }
                        if(reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }
                        if(reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }
                    }
                    cardNum.Text = decryptData(creditCardNum);
                    cardPin.Text = decryptData(creditCardPin);
                    cardDate.Text = decryptData(creditCardExpireDate);
                }
            }//try
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                       using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }


            //

            catch (Exception ex)
            {
                    throw new Exception(ex.ToString());
            }
                finally { }
                return plainText;


        }
    }
}
