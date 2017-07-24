using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

[WebService(Namespace = "http://microsoft.com/webservices/")]
//[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]

public class Service : System.Web.Services.WebService
{
    public static string constr = "Data Source=TOSHIBA-NTB; Initial Catalog=newDb; Integrated Security = SSPI";
    SqlConnection cnn = new SqlConnection(constr);


    public Service () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }


    //BU VEB SERVİS BAZADAN BÜTÜN İSTİFADƏÇİLƏRİ VƏ ONLARA AİD OLAN BÜTÜN MƏLUMATLARI SEÇİR
    [WebMethod]
    public DataTable SelectUser()
    {
        SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM [user]", cnn); //// u Join points p on u.id = u.user_id if needed add (Join with PK)!
        DataTable dt = new DataTable("[user]");
        adp.Fill(dt);
        return dt;
    }
    //-----------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------

    //BU VEB SERVİS İSTİFADƏÇİNİN GİRDİYİ MƏLUMATLARI BAZAYA YAZIR (ŞİFRƏNİ MD5 İLƏ ŞİFRƏLƏYİB SONRA BAZAYA YAZIR)
    [WebMethod]
    public void InsertUser(string user_name, string name, string surname, string email, string mobile, string password)
    {
        password = EncryptPassword(password);  // <--- ALDIĞIMIZ ŞİFRƏNİ MD5 İLƏ ŞİFRƏLƏDİK

        try
        {
            if (cnn.State == ConnectionState.Closed)
            {
                cnn.Open();
            }
            string request = ("INSERT into [user] (user_name, name, surname, email, mobile, password, registr_date) " + "values (N'" + user_name + "', N'" + name + "', N'" + surname + "' , N'" + email + "' , N'" + mobile + "' , N'" + password + "' , N'" + DateTime.Today + "') ");
            DateTime today = DateTime.Today;
            SqlDataAdapter adapter = new SqlDataAdapter(request, cnn);
            adapter.InsertCommand = new SqlCommand(request, cnn);
            SqlCommand newcmd = new SqlCommand(request, cnn);
            int a = newcmd.ExecuteNonQuery();
            cnn.Close();
            if (a == 0)
            {
                //ƏMRLƏR
            }
            else
            {
                //ƏMRLƏR
            }
        }
        catch (Exception ex)
        {
            //ƏMRLƏR
        }

    }
    //-----------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------
    [WebMethod]
    public int check_usernameAndPhonenumber (string username , string phone_number)
    {
        //username varsa 300
        //phone number varsa 301
        return 300;
    }
    //-----------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------

    //BU VEB SERVİS İSTİFADƏÇİNİN GİRDİYİ ŞİFRƏNİ MD5 İLƏ ŞİFRƏLƏYİR
    [WebMethod]
    public string ConvertPassToMD5(string password)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(password));
        byte[] result = md5.Hash;
        StringBuilder str = new StringBuilder();
        for (int i = 1; i < result.Length; i++)
        {
            str.Append(result[i].ToString("x2"));
        }
        return str.ToString();
    }

    //-----------------------------------------------------------------------------------------------------------------------
    //-----------------------------------------------------------------------------------------------------------------------
    [WebMethod]
    public void CheckUserNameAndPassword (string username, string password)
    {
        SqlCommand cmd = new SqlCommand("SELECT * from [user] where user_name like @username and password = @password;");
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", password);
        cmd.Connection = cnn;
        if (cnn.State == ConnectionState.Closed)
        {
            cnn.Open();
        }

        DataSet ds = new DataSet();
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(ds);
        cnn.Close();

        bool loginSuccessful = ((ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0));

        if (loginSuccessful)
        {
            Session["userSession"] = username;
            //Response.Redirect("~/UserProfile.aspx");
        }
        else
        {
            //Console.WriteLine("Invalid username or password");
            ////Response.Redirect("~/Login.aspx");
        }

    }

    //-------------------PASSWORD ENCRYPT AND DECRYPT DOODLES-----------------------------------
    //-------------------PASSWORD ENCRYPT AND DECRYPT DOODLES-----------------------------------
    //-------------------PASSWORD ENCRYPT AND DECRYPT DOODLES-----------------------------------
    //-------------------PASSWORD ENCRYPT AND DECRYPT DOODLES-----------------------------------
    //-------------------PASSWORD ENCRYPT AND DECRYPT DOODLES-----------------------------------


    [WebMethod]
    public string EncryptPassword(string password)
    {
        string hash = "alakbar";
        byte[] data = UTF8Encoding.UTF8.GetBytes(password);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using (TripleDESCryptoServiceProvider tripDes =
                  new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                ICryptoTransform transform = tripDes.CreateEncryptor();
                byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                password = Convert.ToBase64String(results, 0, results.Length);
            }
        }
        return password;
    }

    [WebMethod]
    public string DecryptPassword(string password)
    {
        string hash = "alakbar";
        byte[] data = Convert.FromBase64String(password);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using (TripleDESCryptoServiceProvider tripDes =
                  new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                ICryptoTransform transform = tripDes.CreateDecryptor();
                byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                password = UTF8Encoding.UTF8.GetString(results);
            }
        }
        return password;
    }










}






