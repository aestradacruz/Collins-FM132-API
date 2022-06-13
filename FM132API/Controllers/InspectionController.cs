using FM132API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace FM132API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InspectionController : Controller
    {

        private readonly IConfiguration _configuration;

        public InspectionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("[action]/{InspectionId}")]
        [HttpGet]
        public JsonResult GetInspectionToolById(int inspectionId)

        {

            DataTable table = new DataTable();


            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            SqlDataReader dataReader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();


                using (SqlCommand sqlCommand = new SqlCommand("p_inspection_tool_select_by_id", myCon))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@InspectionId", inspectionId);


                    dataReader = sqlCommand.ExecuteReader();
                    table.Load(dataReader);
                    dataReader.Close();
                    myCon.Close();




                }


            }

            return new JsonResult(table);
        }

        [Route("[action]/{InspectionId}")]
        [HttpGet]
        public JsonResult GetInspectionPhotoById(int inspectionId)

        {

            DataTable table = new DataTable();


            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            SqlDataReader dataReader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();


                using (SqlCommand sqlCommand = new SqlCommand("p_inspection_photo_select_by_id", myCon))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@InspectionId", inspectionId);


                    dataReader = sqlCommand.ExecuteReader();
                    table.Load(dataReader);
                    dataReader.Close();
                    myCon.Close();




                }


            }

            return new JsonResult(table);
        }

        [Route("[action]/{OwnerMail}")]
        [HttpGet]
        public JsonResult GetInspectionByOwner(string ownerMail)

        {

            DataTable table = new DataTable();


            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            SqlDataReader dataReader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();


                using (SqlCommand sqlCommand = new SqlCommand("p_inspection_select_by_owner", myCon))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@OwnerMail", ownerMail);


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
        public JsonResult GetAllInspections()

        {

            DataTable table = new DataTable();


            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            SqlDataReader dataReader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();


                using (SqlCommand sqlCommand = new SqlCommand("p_inspection_select", myCon))
                {
                    


                    dataReader = sqlCommand.ExecuteReader();
                    table.Load(dataReader);
                    dataReader.Close();
                    myCon.Close();




                }


            }

            return new JsonResult(table);
        }



        [Route("[action]/{Data}")]
        [HttpPost]
        public async Task<JsonResult> SendInspection(IFormCollection data)
        {
            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            Inspection inspection = new Inspection();

            inspection.Type = data["type"];

            inspection.TorqueSeal = data["torqueSeal"];
            inspection.Plaster = data["plaster"];
            inspection.BrokenCorners = data["brokenCorners"];
            inspection.BrokenEdges = data["brokenEdges"];
            inspection.IdLegible = data["idLegible"];
            inspection.ProgramColor = data["programColor"];
            inspection.Bushings = data["bushings"];
            inspection.Deterioration = data["deterioration"];
            inspection.WitnessLine = data["witnessLine"];
            inspection.ColorCode = data["colorCode"];
            inspection.LevelingInspection = data["levelingInspection"];


            inspection.Observation = data["observation"];
            inspection.ToolNotification = data["toolNotification"];
            inspection.ToolOrder = data["toolOrder"];

            inspection.Owner = data["userName"];
            inspection.OwnerMail = data["userMail"];


            var partNumberLines = data["partNumberLines"];
            var serialNumberLines = data["serialNumberLines"];
            var toolCodeLines = data["toolCodeLines"];

            inspection.PartNumberLines = partNumberLines.ToString().Split(","); ;            
            inspection.SerialNumbersLines = serialNumberLines.ToString().Split(",");
            inspection.ToolCodesLines = toolCodeLines.ToString().Split(",");

            inspection.FormatArrayLines();


            DataTable dataTable = new DataTable();

            dataTable.Clear();
            dataTable.Columns.Add("PartNumber");
            dataTable.Columns.Add("SerialNumber");
            dataTable.Columns.Add("ToolCodes");

            foreach(Tool tool in inspection.ToolLines)
            {
                DataRow dataRow = dataTable.NewRow();

                dataRow["PartNumber"] = tool.PartNumber;
                dataRow["SerialNumber"] = tool.SerialNumber;
                dataRow["ToolCodes"] = tool.ToolCode;


                dataTable.Rows.Add(dataRow);
            }

            inspection.Tools = dataTable;



         
            int rowAffected = 0;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();

                using (SqlCommand sqlCommand = new SqlCommand("p_inspection_insert", myCon))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    
                    sqlCommand.Parameters.AddWithValue("@Type", inspection.Type);
                    
                    sqlCommand.Parameters.AddWithValue("@TorqueSeal", inspection.TorqueSeal);
                    sqlCommand.Parameters.AddWithValue("@Plaster", inspection.Plaster);
                    sqlCommand.Parameters.AddWithValue("@BrokenCorners", inspection.BrokenCorners);
                    sqlCommand.Parameters.AddWithValue("@BrokenEdges", inspection.BrokenEdges);
                    sqlCommand.Parameters.AddWithValue("@IdLegible", inspection.IdLegible);
                    sqlCommand.Parameters.AddWithValue("@ProgramColor", inspection.ProgramColor);
                    sqlCommand.Parameters.AddWithValue("@Bushings", inspection.Bushings);
                    sqlCommand.Parameters.AddWithValue("@Deterioration", inspection.Deterioration);
                    sqlCommand.Parameters.AddWithValue("@WitnessLine", inspection.WitnessLine);
                    sqlCommand.Parameters.AddWithValue("@ColorCode", inspection.ColorCode);
                    sqlCommand.Parameters.AddWithValue("@LevelingInspection", inspection.LevelingInspection);

                    sqlCommand.Parameters.AddWithValue("@Observation", inspection.Observation);
                    sqlCommand.Parameters.AddWithValue("@ToolNotification", inspection.ToolNotification);
                    sqlCommand.Parameters.AddWithValue("@ToolOrder", inspection.ToolOrder);

 
                    sqlCommand.Parameters.AddWithValue("@OwnerName", inspection.Owner);
                    sqlCommand.Parameters.AddWithValue("@OwnerMail", inspection.OwnerMail);

                    sqlCommand.Parameters.AddWithValue("@ToolsList", inspection.Tools);


                    /*
                    sqlCommand.Parameters.AddWithValue("@ApproversList", dataTable);
                    sqlCommand.Parameters.AddWithValue("@To", to);*/



                    rowAffected = Convert.ToInt32(sqlCommand.ExecuteScalar());

                    myCon.Close();
                }

                if (rowAffected > 0)
                {
                    
                    // List<System.Net.Mail.Attachment> list = new List<Attachment>();

                    for (int i = 0; i < data.Files.Count; i++)
                    {


                        IFormFile firstFile = data.Files[i];

                        var firstMemoryStream = new MemoryStream();

                        await firstFile.CopyToAsync(firstMemoryStream);

                        byte[] firstBinaryData = firstMemoryStream.ToArray();


                        string fileExtension = firstFile.Name;

                        var mediaTypeName = "";
                        var extension = "";

                          mediaTypeName = MediaTypeNames.Image.Jpeg;

                        extension = ".png";


                        //if (fileExtension.Contains("png"))
                        //{
                        //    mediaTypeName = MediaTypeNames.Image.Jpeg;
                        //    extension = ".png";

                        //}
                        //else if (fileExtension.Contains("jpeg"))
                        //{
                        //    mediaTypeName = MediaTypeNames.Image.Jpeg;
                        //    extension = ".jpg";

                        //}

                        int filesInserted = 0;

                        using (SqlConnection myConFile = new SqlConnection(sqlDataSource))
                        {

                            myConFile.Open();

                            try
                            {


                                using (SqlCommand sqlCommand = new SqlCommand("p_inspection_photo_insert", myConFile))

                                {
                                    sqlCommand.CommandType = CommandType.StoredProcedure;
                                    sqlCommand.Parameters.AddWithValue("@Name", "File " + (i + 1));
                                    sqlCommand.Parameters.AddWithValue("@BinaryData", SqlDbType.Binary).Value = firstBinaryData;
                                    sqlCommand.Parameters.AddWithValue("@FileType", extension);

                                    sqlCommand.Parameters.AddWithValue("@InspectionId", rowAffected);




                                    filesInserted = sqlCommand.ExecuteNonQuery();

                                    myConFile.Close();
                                }
                            }catch(Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        };




                        /*
                        Attachment attachment = new Attachment(new MemoryStream(firstBinaryData), "File " + (i + 1) + extension, mediaTypeName);

                        list.Add(attachment); */
                    }

                    /*
                    MailAddress from = new MailAddress("abel.estrada@collins.com", "Prueba FM132"); // requestor

                    MailMessage message = new MailMessage();

                    message.From = from;
                    message.To.Add("abel.estrada@collins.com");

                    message.Subject = "Prueba FM132";

                    foreach (Attachment attachment in list)
                    {
                        message.Attachments.Add(attachment);

                    }

                    SmtpClient client = new SmtpClient("mail.utc.com");


                    client.UseDefaultCredentials = true;

                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                            ex.ToString());
                    } */

                    return new JsonResult("" + rowAffected);

                }
                /*
                Mail mail = new Mail();

                mail.RequestId = rowAffected.ToString();
                mail.UserName = userName;
                mail.UserMail = userMail;
                mail.MailList = newMailList;
                mail.Name = name;
                mail.Files = list;
                mail.To = to;




                SendMail(mail);
                    */
            
                else
                {
                    return new JsonResult("false");

                }

            };


        }





        [Route("[action]")]
        [HttpGet]
        public JsonResult GetWebVersion()

        {

            DataTable table = new DataTable();


            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            SqlDataReader dataReader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();


                using (SqlCommand sqlCommand = new SqlCommand("p_version_select_web_version", myCon))
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
        public JsonResult GetFormVersion()

        {

            DataTable table = new DataTable();


            string sqlDataSource = _configuration.GetConnectionString("AppCon");

            SqlDataReader dataReader;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {

                myCon.Open();


                using (SqlCommand sqlCommand = new SqlCommand("p_version_select_form_version", myCon))
                {



                    dataReader = sqlCommand.ExecuteReader();
                    table.Load(dataReader);
                    dataReader.Close();
                    myCon.Close();


                }


            }

            return new JsonResult(table);
        }



        [Route("[action]/{InspectionId}")]
        [HttpPost]
        public JsonResult DeleteInspection(string inspectionId)
        {
            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("AppCon");


            SqlDataReader dataReader;

            int rowAffected = 0;


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand sqlCommand = new SqlCommand("p_inspection_delete", myCon))
                {

                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@InspectionId", inspectionId);



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
