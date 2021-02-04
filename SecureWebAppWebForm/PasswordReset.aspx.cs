using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecureWebAppWebForm
{
    public partial class PasswordReset : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;



        protected void BTNResetSubmit_Click(object sender, EventArgs e)
        {
            string userid = HttpUtility.HtmlEncode(TBResetEmail.Text).ToString().Trim();
            string oldPassword = HttpUtility.HtmlEncode(TBResetOldPassword.Text).ToString().Trim();
            string newPassword = HttpUtility.HtmlEncode(TBResetNewPassword.Text).ToString().Trim();
            string confirmNewPassword = HttpUtility.HtmlEncode(TBResetNewPasswordConfirm.Text).ToString().Trim();



            SHA512Managed hashing = new SHA512Managed();
            string oldPasswordHash = getOldPasswordHash(userid);
            string oldPasswordSalt = getOldPasswordSalt(userid);

            int passwordChangeTime = getPasswordLastModified(userid) + 300;

            if (DateTimeOffset.Now.ToUnixTimeSeconds() > passwordChangeTime)
            {
                string pwdWithSalt = oldPassword + oldPasswordSalt;
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                string userHash = Convert.ToBase64String(hashWithSalt);

                if (userHash.Equals(oldPasswordHash))
                {
                    if (newPassword == confirmNewPassword)
                    {

                        string password1 = getPassword1Hash(userid);
                        string password1Salt = getPassword1Salt(userid);
                        string password2 = getPassword2Hash(userid);
                        string password2Salt = getPassword2Salt(userid);

                        string newPasswordSalt1 = newPassword + password1Salt;
                        string newPasswordSalt2 = newPassword + password2Salt;
                        byte[] newPasswordSalt1Hash = hashing.ComputeHash(Encoding.UTF8.GetBytes(newPasswordSalt1));
                        byte[] newPasswordSalt2Hash = hashing.ComputeHash(Encoding.UTF8.GetBytes(newPasswordSalt2));
                        string userNewPasswordSalt1Hash = Convert.ToBase64String(newPasswordSalt1Hash);
                        string userNewPasswordSalt2Hash = Convert.ToBase64String(newPasswordSalt2Hash);

                        if(userNewPasswordSalt1Hash.Equals(password1) || userNewPasswordSalt2Hash.Equals(password2))
                        {
                            resetErrorMsg.Text = "Please dont reuse your password";
                            resetErrorMsg.Visible = true;


                        } else
                        {

                            // Generate random "salt"
                            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                            byte[] saltByte = new byte[8];
                            rng.GetBytes(saltByte);
                            salt = Convert.ToBase64String(saltByte);


                            // Hashing with Salt
                            string passwordWithSalt = newPassword + salt;

                            byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
                            hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                            finalHash = Convert.ToBase64String(hashWithSalt);

                            RijndaelManaged cipher = new RijndaelManaged();
                            cipher.GenerateKey();
                            Key = cipher.Key;
                            IV = cipher.IV;
                            updateAccountPassword(userid);

                            Response.Redirect("~/Login.aspx");

                        }




                    }
                    else
                    {
                        resetErrorMsg.Text = "New passwords dont match";
                        resetErrorMsg.Visible = true;
                    }
                }
                else
                {
                    resetErrorMsg.Text = "Incorrect old password";
                    resetErrorMsg.Visible = true;
                }

            }
            else
            {
                resetErrorMsg.Text = "Cannot change password within 5mins from the last change of password";
                resetErrorMsg.Visible = true;
            }

        }

        public void updateAccountPassword(string userid)
        {
            try
            {

                string password1Hash = getPassword1Hash(userid);
                string password1Salt = getPassword1Salt(userid);

                string MYDBConnectionString =
         System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

                SqlConnection con = new SqlConnection(MYDBConnectionString);


                string sqlStmt = "Update Account Set passwordHash = @passwordHash, passwordSalt = @passwordSalt, passwordLastModified = @passwordLastModified," +
                    " passwordHistory2 = @passwordHistory2, passwordHistory2Salt = @passwordHistory2Salt," +
                    "passwordHistory1 = @passwordHistory1, passwordHistory1Salt = @passwordHistory1Salt Where email = @userid";
                SqlCommand cmd = new SqlCommand(sqlStmt, con);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@passwordHash", finalHash);
                cmd.Parameters.AddWithValue("@passwordSalt", salt);
                cmd.Parameters.AddWithValue("@passwordLastModified", DateTimeOffset.Now.ToUnixTimeSeconds());
                cmd.Parameters.AddWithValue("@userid", userid);


                cmd.Parameters.AddWithValue("@passwordHistory2", password1Hash);
                cmd.Parameters.AddWithValue("@passwordHistory2Salt", password1Salt);
                cmd.Parameters.AddWithValue("@passwordHistory1", finalHash);
                cmd.Parameters.AddWithValue("@passwordHistory1Salt", salt);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        protected int getPasswordLastModified(string userid)
        {
            int lastModified = 0;
            string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select passwordLastModified  FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["passwordLastModified"] != null)
                        {
                            if (reader["passwordLastModified"] != DBNull.Value)
                            {
                                lastModified = Convert.ToInt32(reader["passwordLastModified"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return lastModified;
        }

        protected string getOldPasswordHash(string userid)
        {
            string oldPasswordHash = null;
            string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select passwordHash  FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["passwordHash"] != null)
                        {
                            if (reader["passwordHash"] != DBNull.Value)
                            {
                                oldPasswordHash = reader["passwordHash"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return oldPasswordHash;
        }

        protected string getOldPasswordSalt(string userid)
        {
            string oldPasswordSalt = null;
            string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select passwordSalt  FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["passwordSalt"] != null)
                        {
                            if (reader["passwordSalt"] != DBNull.Value)
                            {
                                oldPasswordSalt = reader["passwordSalt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return oldPasswordSalt;
        }


        protected string getPassword1Hash(string userid)
        {
            string password1Hash = null;
            string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select passwordHistory1  FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["passwordHistory1"] != null)
                        {
                            if (reader["passwordHistory1"] != DBNull.Value)
                            {
                                password1Hash = reader["passwordHistory1"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return password1Hash;
        }

        protected string getPassword1Salt(string userid)
        {
            string password1Salt = null;
            string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select passwordHistory1Salt  FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["passwordHistory1Salt"] != null)
                        {
                            if (reader["passwordHistory1Salt"] != DBNull.Value)
                            {
                                password1Salt = reader["passwordHistory1Salt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return password1Salt;
        }

        protected string getPassword2Hash(string userid)
        {
            string password2Hash = null;
            string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select passwordHistory2  FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["passwordHistory2"] != null)
                        {
                            if (reader["passwordHistory2"] != DBNull.Value)
                            {
                                password2Hash = reader["passwordHistory2"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return password2Hash;
        }

        protected string getPassword2Salt(string userid)
        {
            string password2Salt = null;
            string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select passwordHistory2Salt  FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["passwordHistory2Salt"] != null)
                        {
                            if (reader["passwordHistory2Salt"] != DBNull.Value)
                            {
                                password2Salt = reader["passwordHistory2Salt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return password2Salt;
        }
    }
}