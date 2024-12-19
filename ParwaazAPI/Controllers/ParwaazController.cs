using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using System.DateTime;

namespace ParwaazAPI.Controllers
{
    public class ParwaazController : ApiController
    {
        DBConnection db = new DBConnection();


        [ActionName("PostEntries")]
        [Route("api/Parwaaz/PostEntries")]
        [HttpPost]
        public HttpResponseMessage PostEntries([FromBody] List<clsParwaaz> parwaaz)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, new { });
            clsResponse overallResponse = new clsResponse();
            List<clsResponse> responses = new List<clsResponse>();

            try
            {
                foreach (clsParwaaz info in parwaaz)
                {
                    clsResponse resp = new clsResponse();

                    if (!IsValidDateTime(info.PostingDate))
                    {
                        clsResponse resp2 = new clsResponse
                        {
                            StatusCode = 400,
                            Status = "PostingDate should be DateTime Type."
                        };
                        responses.Add(resp2);
                        continue;
                    }
                    else if (!IsValidDecimal(info.CreditAmount) || !IsValidDecimal(info.DebitAmount))
                    {
                        clsResponse resp2 = new clsResponse
                        {
                            StatusCode = 400,
                            Status = "Debit Amount or Credit Amount should be Decimal Type."
                        };
                        responses.Add(resp2);
                        continue;
                    }

                    else if (string.IsNullOrWhiteSpace(info.EntryID))
                    {
                        clsResponse resp1 = new clsResponse
                        {
                            StatusCode = 110,
                            Status = "EntryID cannot be empty or null."
                        };
                        responses.Add(resp1);
                        continue;
                    }
                    else if (info.PostingDate == null)
                    {
                        clsResponse resp1 = new clsResponse
                        {
                            StatusCode = 110,
                            Status = "PostingDate cannot be empty or null."
                        };
                        responses.Add(resp1);
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(info.DocumentType))
                    {
                        clsResponse resp1 = new clsResponse
                        {
                            StatusCode = 110,
                            Status = "Failed, DocumentType is missing"
                        };
                        responses.Add(resp1);
                        continue;

                    }
                    else if (string.IsNullOrWhiteSpace(info.DocumentNo))
                    {
                        clsResponse resp1 = new clsResponse
                        {
                            StatusCode = 110,
                            Status = "Failed, DocumentNo is missing"
                        };
                        responses.Add(resp1);
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(info.AccountType))
                    {
                        clsResponse resp1 = new clsResponse
                        {
                            StatusCode = 110,
                            Status = "Failed, AccountType is missing"
                        };
                        responses.Add(resp1);
                        continue;
                    }

                    else if (string.IsNullOrWhiteSpace(info.AccountNo))
                    {
                        clsResponse resp1 = new clsResponse
                        {
                            StatusCode = 110,
                            Status = "Failed, AccountNo is missing"
                        };
                        responses.Add(resp1);
                        continue;
                    }
                    else if (info.CreditAmount == 0 && info.DebitAmount == 0)
                    {
                        clsResponse resp1 = new clsResponse
                        {
                            StatusCode = 110,
                            Status = "Failed, CreditAmount or DebitAmount parameter is missing"
                        };
                        responses.Add(resp1);
                        continue;
                    }

                    else
                    {
                        using (SqlConnection conn = db.GetDBConnection())
                        {
                            using (SqlCommand cmd = new SqlCommand("InsertEntries"))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Connection = conn;
                                cmd.Parameters.AddWithValue("@id", info.id);
                                cmd.Parameters.AddWithValue("@EntryID", info.EntryID);
                                cmd.Parameters.AddWithValue("@PostingDate", info.PostingDate);
                                cmd.Parameters.AddWithValue("@DocumentType", info.DocumentType);
                                cmd.Parameters.AddWithValue("@DocumentNo", info.DocumentNo);
                                cmd.Parameters.AddWithValue("@AccountType", info.AccountType);
                                cmd.Parameters.AddWithValue("@Comment", info.Comment);
                                cmd.Parameters.AddWithValue("@ShortcutDimension1Code", info.ShortcutDimension1Code);
                                cmd.Parameters.AddWithValue("@ShortcutDimension2Code", info.ShortcutDimension2Code);
                                cmd.Parameters.AddWithValue("@ShortcutDimension3Code", info.ShortcutDimension3Code);
                                cmd.Parameters.AddWithValue("@AccountNo", info.AccountNo);
                                cmd.Parameters.AddWithValue("@CreditAmount", info.CreditAmount);
                                cmd.Parameters.AddWithValue("@DebitAmount", info.DebitAmount);

                                conn.Open();
                                int st = cmd.ExecuteNonQuery();

                                if (st == 1)
                                {
                                    resp.StatusCode = 200;
                                    resp.Status = "Posted";
                                }
                                else
                                {
                                    resp.StatusCode = 300;
                                    resp.Status = "Failed";
                                }
                            }
                        }
                    }

                    responses.Add(resp);
                }

                if (responses.Any(r => r.StatusCode == 200))
                {
                    response = Request.CreateResponse(HttpStatusCode.OK, new { responses });

                }
                else
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new { responses });
                }
            }
            catch (SqlException ex)
            {
                clsResponse sqlErrorResponse = new clsResponse
                {
                    StatusCode = 500,
                    Status = "An error occurred while processing your request: " + ex.Message
                };

                responses.Add(sqlErrorResponse);

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new { responses });
            }
            catch (Exception ex)
            {
                clsResponse otherErrorResponse = new clsResponse
                {
                    StatusCode = 500,
                    Status = "An unexpected error occurred while processing your request: " + ex.Message
                };

                responses.Add(otherErrorResponse);

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, new { responses });
            }

            return response;
        }
        private bool IsValidDateTime(object value)
        {
            DateTime result;
            return DateTime.TryParse(value.ToString(), out result);
        }

        private bool IsValidDecimal(object value)
        {
            decimal result;
            return Decimal.TryParse(value.ToString(), out result);
        }



        [ActionName("GetUnfetchedEntries")]
        [Route("api/Parwaaz/GetUnfetchedEntries")]
        [HttpGet]
        public HttpResponseMessage GetUnfetchedEntries()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, new { });
            var result = GetEntriesFromDB();
            response = Request.CreateResponse(HttpStatusCode.OK, new { result });
            return response;
        }

        public List<clsParwaaz> GetEntriesFromDB()
        {
            try
            {
                List<clsParwaaz> lst = new List<clsParwaaz>();
                using (SqlConnection conn = db.GetDBConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("GetUnfetchedEntries"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            clsParwaaz info = new clsParwaaz();
                            info.id = Convert.ToInt32(reader["id"]);
                            info.EntryID = reader["EntryID"].ToString();
                            info.PostingDate = Convert.ToDateTime(reader["PostingDate"]);
                            info.DocumentType = reader["DocumentType"].ToString();
                            info.DocumentNo = reader["DocumentNo"].ToString();
                            info.AccountType = reader["AccountType"].ToString();
                            info.AccountNo = reader["AccountNo"].ToString();
                            info.Comment = reader["Comment"].ToString();
                            info.ShortcutDimension1Code = reader["ShortcutDimension1Code"].ToString();
                            info.ShortcutDimension2Code = reader["ShortcutDimension2Code"].ToString();
                            info.ShortcutDimension3Code = reader["ShortcutDimension3Code"].ToString();
                            //info.ExternalDocumentNo = reader["ExternalDocumentNo"].ToString();
                            info.CreditAmount = Convert.ToDecimal(reader["CreditAmount"]);
                            info.DebitAmount = Convert.ToDecimal(reader["DebitAmount"]);
                            lst.Add(info);
                        }
                        conn.Close();
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [ActionName("PostBanks")]
        [Route("api/Parwaaz/PostBanks")]
        [HttpPost]
        public HttpResponseMessage PostBanks([FromBody] List<clsBank> bank)
        {
            int st = 0;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, new { });
            //List<clsResponse> Response = new List<clsResponse>();
            foreach (clsBank info in bank)
            {
                using (SqlConnection conn = db.GetDBConnection())
                {
                    {
                        using (SqlCommand cmd = new SqlCommand("InsertBanks"))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@No", info.BankNo);
                            cmd.Parameters.AddWithValue("@Name", info.BankName);
                            conn.Open();
                            st = cmd.ExecuteNonQuery();
                        }
                        clsResponse resp = new clsResponse();
                        resp.StatusCode = st;
                        if (st == 1)
                        {
                            resp.Status = "Posted";
                        }
                        if (st == 0)
                        {
                            resp.Status = "Failed";
                        }
                        response = Request.CreateResponse(HttpStatusCode.OK, new { resp });
                    }
                    conn.Close();
                }
            }
            return response;
        }

        [ActionName("GetBanks")]
        [Route("api/Parwaaz/GetBanks")]
        [HttpGet]
        public List<clsBank> GetBanks()
        {
            try
            {
                List<clsBank> lst = new List<clsBank>();
                using (SqlConnection conn = db.GetDBConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("GetBanks"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            clsBank info = new clsBank();
                            info.BankNo = reader["BankNo"].ToString();
                            info.BankName = reader["BankName"].ToString();

                            lst.Add(info);
                        }
                        conn.Close();
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Erro API Starts 

        [ActionName("PostError")]
        [Route("api/Parwaaz/PostError")]
        [HttpPost]
        public HttpResponseMessage PostError([FromBody] List<clsError> error)
        {
            int st = 0;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, new { });
            foreach (clsError info in error)
            {
                using (SqlConnection conn = db.GetDBConnection())
                {
                    {
                        using (SqlCommand cmd = new SqlCommand("InsertError"))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@EntryID", info.EntryID);
                            cmd.Parameters.AddWithValue("@DocumentNo", info.DocumentNo);
                           // cmd.Parameters.AddWithValue("@ErrorMsg", info.ErrorMsg);
                            conn.Open();
                            st = cmd.ExecuteNonQuery();
                        }
                        clsResponse resp = new clsResponse();
                        resp.StatusCode = st;
                        if (st == 1)
                        {
                            resp.Status = "Posted";
                        }
                        if (st == 0)
                        {
                            resp.Status = "Failed";
                        }
                        response = Request.CreateResponse(HttpStatusCode.OK, new { resp });
                    }
                    conn.Close();
                }
            }
            return response;
        }

        [ActionName("GetError")]
        [Route("api/Parwaaz/GetError")]
        [HttpGet]
        public List<clsError> GetError()
        {
            try
            {
                List<clsError> lst = new List<clsError>();
                using (SqlConnection conn = db.GetDBConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("GetError"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            clsError info = new clsError();
                            info.EntryID = reader["EntryID"].ToString();
                            info.DocumentNo = reader["DocumentNo"].ToString();
                           // info.ErrorMsg = reader["ErrorMsg"].ToString();

                            lst.Add(info);
                        }
                        conn.Close();
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Error API ends!



        [ActionName("InsertChartOfAccount")]
        [Route("api/Parwaaz/PostChartOfAccount")]
        [HttpPost]
        public HttpResponseMessage InsertChartOfAccount([FromBody] List<clsChartOfAccount> chart)
        {
            int st = 0;
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, new { });
            //List<clsResponse> Response = new List<clsResponse>();
            foreach (clsChartOfAccount info in chart)
            {
                using (SqlConnection conn = db.GetDBConnection())
                {
                    {

                        using (SqlCommand cmd = new SqlCommand("InsertChartOfAccount"))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@No", info.No);
                            cmd.Parameters.AddWithValue("@Name", info.Name);

                            conn.Open();
                            st = cmd.ExecuteNonQuery();
                        }

                        clsResponse resp = new clsResponse();
                        resp.StatusCode = st;
                        if (st == 1)
                        {
                            resp.Status = "Posted";
                        }
                        if (st == 0)
                        {
                            resp.Status = "Failed";
                        }

                        response = Request.CreateResponse(HttpStatusCode.OK, new { resp });

                    }
                    conn.Close();
                }
            }

            return response;
        }

        [ActionName("GetChartOfAccount")]
        [Route("api/Parwaaz/GetChartOfAccount")]
        [HttpGet]
        public List<clsChartOfAccount> GetChartOfAccount()
        {
            try
            {
                List<clsChartOfAccount> lst = new List<clsChartOfAccount>();
                using (SqlConnection conn = db.GetDBConnection())
                {
                    using (SqlCommand cmd = new SqlCommand("GetChartOfAccount"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            clsChartOfAccount info = new clsChartOfAccount();
                            info.No = reader["No"].ToString();
                            info.Name = reader["Name"].ToString();

                            lst.Add(info);
                        }
                        conn.Close();
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
