using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

namespace FM132API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SerialNumberController : Controller
    {
        private readonly IConfiguration _configuration;

        public SerialNumberController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public JsonResult Get()

        {

            DataTable table = new DataTable();


            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            SqlDataReader dataReader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();


                using (SqlCommand sqlCommand = new SqlCommand("p_serial_number_select", myCon))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                  

                    dataReader = sqlCommand.ExecuteReader();
                    table.Load(dataReader);
                    dataReader.Close();
                    myCon.Close();
                }


            }

            return new JsonResult(table);
        }


        [Route("[action]/{SerialNumber}/{Description}/{ToolCode}/{Area}/{Location}/{ToolOwner}")]
        [HttpPost]
        public JsonResult AddSerialNumber   (string serialNumber, string description, string toolCode, string area, string location, string toolOwner)
        {
            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            int rowAffected = 0;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();

                using (SqlCommand sqlCommand = new SqlCommand("p_serial_number_insert", myCon))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;


                    sqlCommand.Parameters.AddWithValue("@SerialNumber", serialNumber);
                    sqlCommand.Parameters.AddWithValue("@Description", description);
                    sqlCommand.Parameters.AddWithValue("@ToolCode", toolCode);
                    sqlCommand.Parameters.AddWithValue("@Area", area);
                    sqlCommand.Parameters.AddWithValue("@Location", location);
                    sqlCommand.Parameters.AddWithValue("@ToolOwner", toolOwner);




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


        [Route("[action]/{ToolId}/{SerialNumber}/{Description}/{ToolCode}")]
        [HttpPost]
        public JsonResult UpdateSerialNumber(string toolId, string serialNumber, string description, string toolCode)
        {
            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("AppCon");


            SqlDataReader dataReader;

            int rowAffected = 0;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand sqlCommand = new SqlCommand("p_serial_number_update", myCon))
                {

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@ToolId", toolId);
                    sqlCommand.Parameters.AddWithValue("@SerialNumber", serialNumber);
                    sqlCommand.Parameters.AddWithValue("@Description", description);
                    sqlCommand.Parameters.AddWithValue("@ToolCode", toolCode);





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
