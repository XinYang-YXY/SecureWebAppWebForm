using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text.RegularExpressions; // for Regular expression
using System.Drawing; // for change of color

// For Password Hashing
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace SecureWebAppWebForm
{
    public partial class _Default : Page
    {
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        public string success { get; set; }
        public List<string> ErrorMessage { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // Part One
        private int PasswordScoring(string password)
        {
            int score = 0;
            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }
            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }
            if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                score++;
            }
            return score;
        }

        protected void RegisterSubmitBtn_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                int scores = PasswordScoring(HttpUtility.HtmlEncode(TBRegisterPassword.Text));

                string status = "";
                switch (scores)
                {
                    case 1:
                        status = "Very Weak";
                        break;
                    case 2:
                        status = "Weak";
                        break;
                    case 3:
                        status = "Medium";
                        break;
                    case 4:
                        status = "Strong";
                        break;
                    case 5:
                        status = "Excellent";
                        break;
                    default:
                        break;
                }
                LBLRegisterPasswordAlert.Text = "Status: " + status;
                if (scores < 4)
                {
                    LBLRegisterPasswordAlert.CssClass = "text-danger";
                    return;
                }
                LBLRegisterPasswordAlert.CssClass = "text-success";

                // Part Two - Password Hashing & Credit Card Info Encryption
                if (HttpUtility.HtmlEncode(TBRegisterPassword.Text).ToString() == HttpUtility.HtmlEncode(TBRegisterConfirmPassword.Text).ToString())
                {
                    string password = HttpUtility.HtmlEncode(TBRegisterPassword.Text).ToString().Trim();

                    // Generate random "salt"
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];
                    rng.GetBytes(saltByte);
                    salt = Convert.ToBase64String(saltByte);

                    // Hashing Instance
                    SHA512Managed hashing = new SHA512Managed();

                    // Hashing with Salt
                    string passwordWithSalt = password + salt;

                    byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(password));
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                    finalHash = Convert.ToBase64String(hashWithSalt);

                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.GenerateKey();
                    Key = cipher.Key;
                    IV = cipher.IV;
                    createAccount();
                    Response.Redirect("~/Login.aspx");
                }
                //else
                //{
                //    Response.Redirect("~/Registration.aspx");
                //}
            }
        }

        // Part Two - Password Hashing & Credit Card Info Encryption
        public void createAccount()
        {
            try
            {
                string MYDBConnectionString =
         System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;

                SqlConnection con = new SqlConnection(MYDBConnectionString);


                string sqlStmt = "INSERT INTO Account VALUES(@firstName, @lastName, @creditCardNum," +
                    " @email, @passwordHash, @passwordSalt, @dob, @creditCardPin, @creditCardExpireDate, @IV, @Key, @failedLoginAttempt, @passwordHistory1, @passwordHistory1Salt, @passwordHistory2, @passwordHistory2Salt, @passwordLastModified, @recoveryGUID)";
                SqlCommand cmd = new SqlCommand(sqlStmt, con);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@firstName", HttpUtility.HtmlEncode(TBRegisterFirstName.Text.Trim()));
                cmd.Parameters.AddWithValue("@lastName", HttpUtility.HtmlEncode(TBRegisterLastName.Text.Trim()));
                cmd.Parameters.AddWithValue("@creditCardNum", Convert.ToBase64String(encryptData(HttpUtility.HtmlEncode(TBRegisterCreditCardNum.Text).Trim())));
                cmd.Parameters.AddWithValue("@email", HttpUtility.HtmlEncode(TBRegisterEmail.Text).Trim());
                cmd.Parameters.AddWithValue("@passwordHash", finalHash);
                cmd.Parameters.AddWithValue("@passwordSalt", salt);
                cmd.Parameters.AddWithValue("@dob", HttpUtility.HtmlEncode(TBRegisterDOB.Text.Trim()));
                cmd.Parameters.AddWithValue("@creditCardPin", Convert.ToBase64String(encryptData(HttpUtility.HtmlEncode(TBRegisterCVC.Text.Trim()))));
                cmd.Parameters.AddWithValue("@creditCardExpireDate", Convert.ToBase64String(encryptData(HttpUtility.HtmlEncode(TBRegisterCreditCardDate.Text.Trim()))));
                cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                cmd.Parameters.AddWithValue("@failedLoginAttempt", 0);
                cmd.Parameters.AddWithValue("@passwordHistory1", finalHash);
                cmd.Parameters.AddWithValue("@passwordHistory1Salt", salt);
                cmd.Parameters.AddWithValue("@passwordHistory2", finalHash);
                cmd.Parameters.AddWithValue("@passwordHistory2Salt", salt);
                cmd.Parameters.AddWithValue("@passwordLastModified", DateTimeOffset.Now.ToUnixTimeSeconds());
                cmd.Parameters.AddWithValue("@recoveryGUID", "");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();











            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data); cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex) { throw new Exception(ex.ToString()); }
            finally { }
            return cipherText;
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response-register"];


            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6Le3TkgaAAAAANFBp8eqzBnCc3EWc5k5TXAshYCJ &response=" + captchaResponse);
            // HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LfjzOQZAAAAANyilUp0Bi6fDVGRevu1q6ERjOHu &response=" + captchaResponse);
            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        registerCaptcha.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        _Default jsonObject = js.Deserialize<_Default>(jsonResponse);

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




    }


}