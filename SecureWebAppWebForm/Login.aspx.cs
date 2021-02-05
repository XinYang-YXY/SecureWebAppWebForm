using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace SecureWebAppWebForm
{
    public partial class Login : System.Web.UI.Page
    {

        public string success { get; set; }
        public List<string> ErrorMessage { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LoginSubmitBtn_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {


                string pwd = HttpUtility.HtmlEncode(TBLoginPassword.Text).ToString().Trim();
                string userid = HttpUtility.HtmlEncode(TBLoginEmail.Text).ToString().Trim();

                SHA512Managed hashing = new SHA512Managed();
                string dhHash = getDBHash(userid);
                string dbSalt = getDBSalt(userid);
                int failedAttempts = getFailedAttempts(userid);
                int passwordExpiredTime = getPasswordLastModified(userid) + 900;
                int lockoutTime = getLockoutTime(userid) + 10;

                string errorMsg = "";

                if (failedAttempts < 3 || DateTimeOffset.Now.ToUnixTimeSeconds() > lockoutTime)
                {
                    if (failedAttempts >= 3 || DateTimeOffset.Now.ToUnixTimeSeconds()>lockoutTime)
                    {
                        string MYDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
                        SqlConnection connection = new SqlConnection(MYDBConnectionString);
                        string sqlstmt = "Update Account Set failedLoginAttempt= @failedAttempts Where email = @email";
                        SqlCommand sqlCmd = new SqlCommand(sqlstmt, connection);
                        sqlCmd.Parameters.AddWithValue("@failedAttempts", 0);
                        sqlCmd.Parameters.AddWithValue("@email", userid);
                        connection.Open();
                        sqlCmd.ExecuteNonQuery();
                        connection.Close();

                    }


                    try
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dhHash != null & dhHash.Length > 0)
                        {
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);

                            if (userHash.Equals(dhHash))
                            {
                                if (DateTimeOffset.Now.ToUnixTimeSeconds() > passwordExpiredTime)
                                {
                                    errorMsg = "Password has expired, please change";
                                    loginErrorMsg.Text = errorMsg;
                                    loginErrorMsg.Visible = true;
                                    GoToResetPasswordBtn.Visible = true;
                                }
                                else
                                {

                                    // Session Fixation
                                    Session["LoggedIn"] = userid;
                                    string guid = Guid.NewGuid().ToString();
                                    Session["AuthToken"] = guid;
                                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));


                                    Response.Redirect("~/Success.aspx");
                                }
                            }
                            else
                            {
                                if (failedAttempts == 2)
                                {
                                    errorMsg = "Userid or password is not valid. Please try again. " + "Last try before account lockout";
                                }
                                else
                                {
                                    errorMsg = "Userid or password is not valid. Please try again. " + (2 - failedAttempts).ToString() + " tries left";
                                }
                                loginErrorMsg.Text = errorMsg;
                                loginErrorMsg.Visible = true;
                                string MYDBConnectionString =
            System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
                                SqlConnection connection = new SqlConnection(MYDBConnectionString);
                                string sqlstmt = "Update Account Set failedLoginAttempt= @failedAttempts, lockoutTime = @lockoutTime Where email = @email";
                                SqlCommand sqlCmd = new SqlCommand(sqlstmt, connection);
                                sqlCmd.Parameters.AddWithValue("@failedAttempts", failedAttempts + 1);
                                sqlCmd.Parameters.AddWithValue("@email", userid);
                                sqlCmd.Parameters.AddWithValue("@lockoutTime", DateTimeOffset.Now.ToUnixTimeSeconds());
                                connection.Open();
                                sqlCmd.ExecuteNonQuery();
                                connection.Close();

                                // Response.Redirect("Login.aspx", false); 
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    finally { }
                }
                else
                {
                    loginErrorMsg.Text = "Your account has been lock out!";
                    loginErrorMsg.Visible = true;
                }
            }

        }
        protected string getDBHash(string userid)
        {
            string h = null;
            string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PasswordHash FROM Account WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                h = reader["PasswordHash"].ToString();
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
            return h;
        }
        protected string getDBSalt(string userid)
        {
            string s = null;
            string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PASSWORDSALT FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PASSWORDSALT"] != null)
                        {
                            if (reader["PASSWORDSALT"] != DBNull.Value)
                            {
                                s = reader["PASSWORDSALT"].ToString();
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
            return s;
        }

        protected int getFailedAttempts(string userid)
        {
            int failedAttempts = 0;
            string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select failedLoginAttempt FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["failedLoginAttempt"] != null)
                        {
                            if (reader["failedLoginAttempt"] != DBNull.Value)
                            {
                                failedAttempts = Convert.ToInt32(reader["failedLoginAttempt"].ToString());
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
            return failedAttempts;
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

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6Le3TkgaAAAAANFBp8eqzBnCc3EWc5k5TXAshYCJ &response=" + captchaResponse);
            // HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LfjzOQZAAAAANyilUp0Bi6fDVGRevu1q6ERjOHu &response=" + captchaResponse);
            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        loginCaptcha.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        Login jsonObject = js.Deserialize<Login>(jsonResponse);

                        result = Convert.ToBoolean(jsonObject.success);
                    }

                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }

            return result;
        }

        protected void GoToResetPasswordBtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/PasswordReset.aspx");
        }

        protected int getLockoutTime(string userid)
        {
            int lockoutTime = 0;
            string MYDBConnectionString =
        System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select lockoutTime  FROM ACCOUNT WHERE Email=@USERID";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@USERID", userid);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["lockoutTime"] != null)
                        {
                            if (reader["lockoutTime"] != DBNull.Value)
                            {
                                lockoutTime = Convert.ToInt32(reader["lockoutTime"].ToString());
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
            return lockoutTime;
        }
    }
}