using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FM132API.Models;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace FM132API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {

        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult Post(User user)

        {
            string[] userInfo = new string[4];
            try
            {


                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "utcain.com"))
                {
                    // validate the credentials
                    bool isValid = pc.ValidateCredentials(user.UserName, user.Password);

                    if (!isValid)
                    {
                        userInfo[0] = "false";
                        return new JsonResult(userInfo);

                    }
                    else
                    {

                        string name = "";


                        using (var context = new PrincipalContext(ContextType.Domain))
                        {
                            var usr = UserPrincipal.FindByIdentity(context, user.UserName);
                            if (usr != null)
                            {
                                userInfo[0] = usr.EmailAddress;
                                userInfo[1] = usr.GivenName;
                                userInfo[2] = usr.GivenName + ' ' + usr.Surname;
                            }
                        }


                        // userInfo[3] = IsAdminUser(userInfo[0]).ToString();



                        return new JsonResult(userInfo);

                    }
                }
            }
            catch
            {
                userInfo[0] = "false";
                return new JsonResult(userInfo[0]);
            }

        }

        [Route("[action]/{Mail}")]
        [HttpGet]
        public JsonResult GetUserRequestData(string mail)
        {

            DataTable table = new DataTable();
            //SqlDataReader dataReader;

            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            SqlDataReader dataReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();
                using (SqlCommand sqlCommand = new SqlCommand("p_select_user_request_data", myCon))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@UserMail", mail);



                    dataReader = sqlCommand.ExecuteReader();
                    table.Load(dataReader);
                    dataReader.Close();
                    myCon.Close();

                }



            };


            return new JsonResult(table);
        }


        [Route("[action]/{Mail}")]
        [HttpGet]
        public JsonResult GetUserLastApprovals (string mail)
        {

            DataTable table = new DataTable();
            //SqlDataReader dataReader;

            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            SqlDataReader dataReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();
                using (SqlCommand sqlCommand = new SqlCommand("p_select_user_get_last_approvals", myCon))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@UserMail", mail);



                    dataReader = sqlCommand.ExecuteReader();
                    table.Load(dataReader);
                    dataReader.Close();
                    myCon.Close();

                }



            };


            return new JsonResult(table);
        }


        [Route("[action]/{Mail}")]
        [HttpGet]
        public JsonResult GetUserPrivilages(string mail)
        {

            DataTable table = new DataTable();
            //SqlDataReader dataReader;

            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            SqlDataReader dataReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();
                using (SqlCommand sqlCommand = new SqlCommand("p_administrator_select_by_mail", myCon))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Mail", mail);



                    dataReader = sqlCommand.ExecuteReader();
                    table.Load(dataReader);
                    dataReader.Close();
                    myCon.Close();

                }



            };

            if (table.Rows.Count > 0)
                return new JsonResult("true");
            else
                return new JsonResult("false");


        }


        [Route("[action]")]
        [HttpGet]
        public JsonResult GetAdmins()

        {

            DataTable table = new DataTable();


            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            SqlDataReader dataReader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();


                using (SqlCommand sqlCommand = new SqlCommand("p_administrators_select", myCon))
                {



                    dataReader = sqlCommand.ExecuteReader();
                    table.Load(dataReader);
                    dataReader.Close();
                    myCon.Close();




                }


            }

            return new JsonResult(table);
        }


        [Route("[action]")]
        [HttpGet]
        public JsonResult GetUsers()

        {

            DataTable table = new DataTable();


            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            SqlDataReader dataReader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();


                using (SqlCommand sqlCommand = new SqlCommand("p_user_select", myCon))
                {



                    dataReader = sqlCommand.ExecuteReader();
                    table.Load(dataReader);
                    dataReader.Close();
                    myCon.Close();




                }


            }

            return new JsonResult(table);
        }

        [Route("[action]/{AdminMail}")]
        [HttpPost]
        public JsonResult AddAdministrator(string adminMail)
        {
            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            int rowAffected = 0;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();

                using (SqlCommand sqlCommand = new SqlCommand("p_administrator_insert", myCon))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;


                    sqlCommand.Parameters.AddWithValue("@AdministratorMail", adminMail);



                    rowAffected = Convert.ToInt32(sqlCommand.ExecuteScalar());

                    myCon.Close();
                }
            };

            if (rowAffected > 0)
            {

                return new JsonResult("" + rowAffected);

            }
            else
            {
                return new JsonResult("false");
            }
        }

        [Route("[action]/{Id}/{Mail}/{Enable}")]
        [HttpPost]
        public JsonResult UpdateAdministrator(int id,
                                string mail,
                                string enable
                               )
        {
            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("AppCon");


            SqlDataReader dataReader;

            int rowAffected = 0;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand sqlCommand = new SqlCommand("p_administrator_update", myCon))
                {

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Id", id);
                    sqlCommand.Parameters.AddWithValue("@Mail", mail);
                    sqlCommand.Parameters.AddWithValue("@Enable", enable);



                    rowAffected = sqlCommand.ExecuteNonQuery();

                    myCon.Close();

                }



            };


            if (rowAffected > 0)
            {

                return new JsonResult("true");

            }
            else
            {
                return new JsonResult("false");
            }
        }




        [Route("[action]/{Info}")]
        [HttpPost]
        public async Task<JsonResult> AddUserStampAsync(IFormCollection info)
        {
            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            string mail = info["mail"];
            string name = info["name"];

            IFormFile firstFile = info.Files[0];

            var firstMemoryStream = new MemoryStream();

            await firstFile.CopyToAsync(firstMemoryStream);

            byte[] firstBinaryData = firstMemoryStream.ToArray();


            IFormFile secondFile = info.Files[1];

            var secondMemoryStream = new MemoryStream();

            await secondFile.CopyToAsync(secondMemoryStream);

            byte[] secondBinaryData = secondMemoryStream.ToArray();

            int rowAffected = 0;

            try
            {

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();

                using (SqlCommand sqlCommand = new SqlCommand("p_user_insert", myCon))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;


                    sqlCommand.Parameters.AddWithValue("@Mail", mail);
                    sqlCommand.Parameters.AddWithValue("@Name", name);
                    sqlCommand.Parameters.AddWithValue("@Stamp", SqlDbType.Binary).Value = firstBinaryData;
                    sqlCommand.Parameters.AddWithValue("@RejectedStamp", SqlDbType.Binary).Value = secondBinaryData;
                    
                    rowAffected = Convert.ToInt32(sqlCommand.ExecuteScalar());

                    myCon.Close();
                }
            }
            }catch (Exception ex)
            {
                return new JsonResult("false");
            }

            if (rowAffected > 0)
            {

                return new JsonResult("" + rowAffected);

            }
            else
            {
                return new JsonResult("false");
            }
        }


        [Route("[action]/{Info}")]
        [HttpPost]
        public async Task<JsonResult> UpdateUserStamp(IFormCollection info)
        {
            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            string userID = info["userID"];

            IFormFile firstFile = info.Files[0];

            var firstMemoryStream = new MemoryStream();

            await firstFile.CopyToAsync(firstMemoryStream);

            byte[] firstBinaryData = firstMemoryStream.ToArray();


            int rowAffected = 0;

            try
            {
                 
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {

                    myCon.Open();

                    using (SqlCommand sqlCommand = new SqlCommand("p_user_update_stamp", myCon))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;


                        sqlCommand.Parameters.AddWithValue("@UserId", userID);
                        sqlCommand.Parameters.AddWithValue("@Stamp", SqlDbType.Binary).Value = firstBinaryData;


                        rowAffected = sqlCommand.ExecuteNonQuery();

                        myCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult("false");
            }

            if (rowAffected > 0)
            {

                return new JsonResult("" + rowAffected);

            }
            else
            {
                return new JsonResult("false");
            }
        }

        [Route("[action]/{Info}")]
        [HttpPost]
        public async Task<JsonResult> UpdateUserRejectedStamp(IFormCollection info)
        {
            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            string userID = info["userID"];

            IFormFile firstFile = info.Files[0];

            var firstMemoryStream = new MemoryStream();

            await firstFile.CopyToAsync(firstMemoryStream);

            byte[] firstBinaryData = firstMemoryStream.ToArray();


            int rowAffected = 0;

            try
            {

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {

                    myCon.Open();

                    using (SqlCommand sqlCommand = new SqlCommand("p_user_update_rejected_stamp", myCon))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;


                        sqlCommand.Parameters.AddWithValue("@UserId", userID);
                        sqlCommand.Parameters.AddWithValue("@Stamp", SqlDbType.Binary).Value = firstBinaryData;


                        rowAffected = sqlCommand.ExecuteNonQuery();

                        myCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return new JsonResult("false");
            }

            if (rowAffected > 0)
            {

                return new JsonResult("" + rowAffected);

            }
            else
            {
                return new JsonResult("false");
            }
        }



        [Route("[action]/{UserId}/{UserMail}")]
        [HttpPost]
        public JsonResult UpdateUserMail(string userId, string userMail)
        {
            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("AppCon");


            SqlDataReader dataReader;

            int rowAffected = 0;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand sqlCommand = new SqlCommand("p_user_update_mail", myCon))
                {

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@UserId", userId);
                    sqlCommand.Parameters.AddWithValue("@NewMail", userMail);




                    rowAffected = sqlCommand.ExecuteNonQuery();

                    myCon.Close();

                }



            };


            if (rowAffected > 0)
            {

                return new JsonResult("true");

            }
            else
            {
                return new JsonResult("false");
            }
        }




        [Route("[action]/{Version}")]
        [HttpPost]
        public JsonResult UpdateVersion(string version)
        {
            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("AppCon");


            SqlDataReader dataReader;

            int rowAffected = 0;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand sqlCommand = new SqlCommand("p_version_update_form_version", myCon))
                {

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Version", version);



                    rowAffected = sqlCommand.ExecuteNonQuery();

                    myCon.Close();

                }



            };


            if (rowAffected > 0)
            {

                return new JsonResult("true");

            }
            else
            {
                return new JsonResult("false");
            }
        }

    }
}
