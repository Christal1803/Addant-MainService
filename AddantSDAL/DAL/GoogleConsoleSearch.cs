using AddantService;
using AddantService.DAL;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Google.Apis.Analytics.v3.Data.GaData.DataTableData.RowsData;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.AnalyticsReporting.v4;
using Newtonsoft.Json;
using System.Collections;
using System.Xml;

namespace AddantSDAL.DAL
{
    public class GoogleConsoleSearch
    {

        public static ServiceAccountCredential GetP12Credential(string p12FilePath, string accountEmail, string from = " ")
        {
            try
            {
                //Logger.WriteLog($"GetP12Credential case 1 from {from} {p12FilePath} {accountEmail}");
                //    using (var certificate = new X509Certificate2(p12FilePath, "notasecret", X509KeyStorageFlags.MachineKeySet | 
                //      X509KeyStorageFlags.Exportable))
                using (var certificate = new X509Certificate2(p12FilePath, "notasecret",
              X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet))
                // bugged me a lot at first i gave X509KeyStorageFlags.Exportable only for local. In server change
                // to X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable
                {

                    Logger.WriteLog("Credential-" + certificate.Subject);
                    var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(accountEmail)
                    {
                        Scopes = new[] {
               // AnalyticsService.Scope.AnalyticsReadonly
                        AnalyticsService.Scope.AnalyticsReadonly
                    }
                    }.FromCertificate(certificate));
                    Logger.WriteLog("Inside GetP12Credential 1" + certificate.PublicKey);
                    return credential;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog("Inside GetP12Credential ex" + ex.Message.ToString());
                return null;
            }

        }


        public static DALResult<DataTable> GetWebsiteVisitCountrywise(int category, string startDate = "today", string endDate = "today")
        {

            /// 1 ---- daily
            /// 2-----weekly
            /// 3 -----monthly
            try
            {
                var credential = GetP12Credential(Path.Combine(HttpContext.Current.Server.MapPath("~/Log"), "adddantwebsite-86b8d564d794.p12"), "addant@adddantwebsite.iam.gserviceaccount.com", "isCountrywise");
                using (var service = new AnalyticsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "AddantWebsite", // application name goes here
                    ApiKey = "AIzaSyAOBksXmmnNuQNW9K_5Gy7LyRU1NSgTpnE", //commented on tuesday
                }))
                {
                    ///newVisits
                    if (category == 1) { startDate = "today"; endDate = "today"; }
                    else if (category == 2) { startDate = "7daysAgo"; endDate = "today"; }
                    else { endDate = "today"; startDate = "31daysAgo"; }
                    Logger.WriteLog("GetWebsiteVisitCountrywise Case1" + service);
                    var apiRequest = service.Data.Ga.Get(
                         ids: "ga:282189311",// ID GOES after ga prefrix
                         startDate: startDate,
                         endDate: endDate,
                         metrics: "ga:pageviews"
                         );
                    apiRequest.IncludeEmptyRows = false;
                    apiRequest.Dimensions = "ga:country";
                    apiRequest.Sort = "ga:country,ga:pageviews";
                    apiRequest.StartIndex = 1;
                    var data = apiRequest.Execute();
                    //IList<string> oIList1 = new List<string>() // new
                    //{
                    //    "Country","Visitor count"
                    //};
                    //data.Rows.Insert(0, oIList1);
                    DataTable dataT = new DataTable();
                    dataT.Columns.Add("Country");
                    dataT.Columns.Add("VisitorCount");
                    foreach (var row in data.Rows)
                    {
                        dataT.Rows.Add(row.ToArray());
                    }
                    Logger.WriteLog("GetWebsiteVisitCountrywise result" + dataT);
                    return new DALResult<DataTable>(Status.Found, dataT, null, null);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog("GetWebsiteVisitCountrywise" + ex.Message.ToString()); return null;
            }
        }

        public static DALResult<GoogleAnalyticsRes> GetWebsiteVisit(int Category, string startDate = "today", string endDate = "today")
        {
            try
            {
                var credential = GetP12Credential(Path.Combine(HttpContext.Current.Server.MapPath("~/Log"), "adddantwebsite-86b8d564d794.p12"), "addant@adddantwebsite.iam.gserviceaccount.com");

                using (var service = new AnalyticsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "AddantWebsite", // application name goes here
                                                       //ApiKey = "AIzaSyAOBksXmmnNuQNW9K_5Gy7LyRU1NSgTpnE",
                }))
                {

                    if (Category == 1) { startDate = "today"; endDate = "today"; }
                    else if (Category == 2) { startDate = "today"; endDate = "7daysAgo"; }
                    else { startDate = "today"; endDate = "31daysAgo"; }
                    Logger.WriteLog("Inside GetWebsiteVisit 1" + service);
                    var apiRequest = service.Data.Ga.Get(
                         ids: "ga:282189311",// ID GOES after ga prefrix
                         startDate: startDate,
                         endDate: endDate,
                         metrics: "ga:pageviews"
                         );
                    apiRequest.MaxResults = 10;
                    apiRequest.Dimensions = "ga:pagePath,ga:pageTitle";
                    apiRequest.IncludeEmptyRows = false;
                    apiRequest.Sort = "-ga:pageviews";
                    //apiRequest.Key = "b59e78d2bb438e88068980f86ca3a35585b632af";
                    var data = apiRequest.Execute();

                    //object result = data.Rows.Select(r => new
                    //{
                    //    PagePath = r[0],
                    //    PageTitle = r[1],
                    //    ViewCount = int.Parse(r[2])
                    //});
                    GoogleAnalyticsRes result = data?.Rows?.Select(r => new GoogleAnalyticsRes
                    {
                        PagePath = r[0],
                        PageTitle = r[1],
                        ViewCount = r[2]
                    }).FirstOrDefault();
                    if (result == null)
                    {
                        result = new GoogleAnalyticsRes
                        {
                            PagePath = "/",
                            PageTitle = "",
                            ViewCount = "0"
                        };
                    }

                    return new DALResult<GoogleAnalyticsRes>(Status.Found, result, null, null);

                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog("Inside GetWebsiteVisit" + ex.Message.ToString()); return null;
            }
        }

        public static DALResult<List<List<object>>> GetWebsiteVisit1(int Category, string startDate = "today", string endDate = "today")
        {
            DataTable dataT = new DataTable();
            DataTable dataU = new DataTable();
            try
            {
                var credential = GetP12Credential(Path.Combine(HttpContext.Current.Server.MapPath("~/Log"), "adddantwebsite-86b8d564d794.p12"), "addant@adddantwebsite.iam.gserviceaccount.com");

                using (var service = new AnalyticsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "AddantWebsite", // application name goes here
                }))
                {
                    if (Category == 1)
                    {
                        startDate = "today";
                        endDate = "today";

                        int i = -24;
                        dataT.Columns.Add("Daily");
                        dataU.Columns.Add("Daily");
                        dataT.Columns.Add("TotalCount");
                        dataU.Columns.Add("UniqueCount");

                        int t = 0;
                        for (int j = 0; j < 24; j++)
                        {
                            startDate = DateTime.Now.AddHours(i).ToString("yyyy-MM-dd"); endDate = DateTime.Now.AddHours(i + 6).ToString("yyyy-MM-dd");
                            dataT.Rows.Add();

                            dataT.Rows[t][0] = DateTime.Now.AddHours(i).Hour + "-- " + DateTime.Now.AddHours(i + 6).Hour;//"day" + (j + 1);

                            var apiRequest = service.Data.Ga.Get(
                             ids: "ga:282189311",// ID GOES after ga prefrix
                             startDate: startDate,
                             endDate: endDate,
                            metrics: "ga:users"
                             );
                            apiRequest.MaxResults = 10;
                            apiRequest.Dimensions = "ga:pagePath,ga:pageTitle";
                            apiRequest.IncludeEmptyRows = false;
                            apiRequest.Sort = "-ga:users";
                            var data = apiRequest.Execute();
                            dataT.NewRow();
                            dataT.Rows[t][1] = data.Rows[0][2];
                            j = j + 6;
                            t++;
                            i = i + 6;
                        }
                        t = 0;
                        i = -24;
                        for (int j = 0; j < 24; j++)
                        {
                            startDate = DateTime.Now.AddHours(i).ToString("yyyy-MM-dd"); endDate = DateTime.Now.AddHours(i + 6).ToString("yyyy-MM-dd");
                            dataU.Rows.Add();

                            dataU.Rows[t][0] = DateTime.Now.AddHours(i).Hour + "-- " + DateTime.Now.AddHours(i + 6).Hour;//"day" + (j + 1);

                            var apiRequest = service.Data.Ga.Get(
                             ids: "ga:282189311",// ID GOES after ga prefrix
                             startDate: startDate,
                             endDate: endDate,
                            metrics: "ga:sessions"
                             );
                            apiRequest.MaxResults = 10;
                            apiRequest.Dimensions = "ga:pagePath,ga:pageTitle";
                            apiRequest.IncludeEmptyRows = false;
                            apiRequest.Sort = "-ga:sessions";
                            var data = apiRequest.Execute();
                            dataU.NewRow();
                            dataU.Rows[t][1] = data.Rows[0][2];
                            j = j + 6;
                            t++;
                            i = i + 6;
                        }

                        dataU.Columns.Add("TotalCount");
                        foreach (DataRow rowT in dataU.Rows)
                        {
                            foreach (DataRow rowU in dataT.Rows)
                            {
                                if (rowT["Daily"].Equals(rowU["Daily"]))
                                {
                                    rowT["TotalCount"] = rowU["TotalCount"];
                                    break;
                                }
                            }
                        }
                    }
                    else if (Category == 2)
                    {
                        int i = 0;
                        dataT.Columns.Add("Weekly");
                        dataU.Columns.Add("Weekly");
                        dataT.Columns.Add("TotalCount");
                        dataU.Columns.Add("UniqueCount");
                        for (int j = 0; j < 7; j++)
                        {
                            startDate = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd"); endDate = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd");
                            dataT.Rows.Add();

                            dataT.Rows[j][0] = DateTime.Now.AddDays(i).DayOfWeek;//"day" + (j + 1);
                            i--;
                            var apiRequest = service.Data.Ga.Get(
                             ids: "ga:282189311",// ID GOES after ga prefrix
                             startDate: startDate,
                             endDate: endDate,
                             metrics: "ga:users"
                             );
                            apiRequest.MaxResults = 10;
                            apiRequest.Dimensions = "ga:pagePath,ga:pageTitle";
                            apiRequest.IncludeEmptyRows = false;
                            apiRequest.Sort = "-ga:users";
                            var data = apiRequest.Execute();
                            dataT.NewRow();
                            dataT.Rows[j][1] = data.Rows[0][2];
                        }
                        i = 0;
                        for (int j = 0; j < 7; j++)
                        {
                            startDate = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd"); endDate = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd");
                            dataU.Rows.Add();

                            dataU.Rows[j][0] = DateTime.Now.AddDays(i).DayOfWeek;//"day" + (j + 1);
                            i--;
                            var apiRequest = service.Data.Ga.Get(
                             ids: "ga:282189311",// ID GOES after ga prefrix
                             startDate: startDate,
                             endDate: endDate,
                             metrics: "ga:sessions"
                             );
                            apiRequest.MaxResults = 10;
                            apiRequest.Dimensions = "ga:pagePath,ga:pageTitle";
                            apiRequest.IncludeEmptyRows = false;
                            apiRequest.Sort = "-ga:sessions";
                            var data = apiRequest.Execute();
                            dataU.NewRow();
                            dataU.Rows[j][1] = data.Rows[0][2];
                        }

                        dataU.Columns.Add("TotalCount");
                        foreach (DataRow rowT in dataU.Rows)
                        {
                            foreach (DataRow rowU in dataT.Rows)
                            {
                                if (rowT["Weekly"].Equals(rowU["Weekly"]))
                                {
                                    rowT["TotalCount"] = rowU["TotalCount"];
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        endDate = "today";
                        startDate = "31daysAgo";
                        int i = -30;
                        dataT.Columns.Add("Monthly");
                        dataU.Columns.Add("Monthly");
                        dataT.Columns.Add("TotalCount");
                        dataU.Columns.Add("UniqueCount");

                        int t = 0;
                        for (int j = 0; j < 30;)
                        {
                            startDate = DateTime.Now.AddDays(i - j).ToString("yyyy-MM-dd"); endDate = DateTime.Now.AddDays(i + 7).ToString("yyyy-MM-dd");
                            dataT.Rows.Add();

                            dataT.Rows[t][0] = "Week" + (t + 1);
                            var apiRequest = service.Data.Ga.Get(
                             ids: "ga:282189311",// ID GOES after ga prefrix
                             startDate: startDate,
                             endDate: endDate,
                             metrics: "ga:users"
                             );
                            apiRequest.MaxResults = 10;
                            apiRequest.Dimensions = "ga:pagePath,ga:pageTitle";
                            apiRequest.IncludeEmptyRows = false;
                            apiRequest.Sort = "-ga:users";
                            var data = apiRequest.Execute();
                            dataT.NewRow();
                            dataT.Rows[t][1] = data.Rows[0][2];
                            j = j + 7;
                            t++;
                        }
                        t = 0;
                        i = -30;
                        for (int j = 0; j < 30;)
                        {
                            startDate = DateTime.Now.AddDays(i - j).ToString("yyyy-MM-dd"); endDate = DateTime.Now.AddDays(i + 7).ToString("yyyy-MM-dd");
                            dataU.Rows.Add();

                            dataU.Rows[t][0] = "Week" + (t + 1);
                            var apiRequest = service.Data.Ga.Get(
                             ids: "ga:282189311",// ID GOES after ga prefrix
                             startDate: startDate,
                             endDate: endDate,
                             metrics: "ga:users"
                             );
                            apiRequest.MaxResults = 10;
                            apiRequest.Dimensions = "ga:pagePath,ga:pageTitle";
                            apiRequest.IncludeEmptyRows = false;
                            apiRequest.Sort = "-ga:users";
                            var data = apiRequest.Execute();
                            dataU.NewRow();
                            dataU.Rows[t][1] = data.Rows[0][2];
                            j = j + 7;
                            t++;
                        }

                        dataU.Columns.Add("TotalCount");
                        foreach (DataRow rowT in dataU.Rows)
                        {
                            foreach (DataRow rowU in dataT.Rows)
                            {
                                if (rowT["Monthly"].Equals(rowU["Monthly"]))
                                {
                                    rowT["TotalCount"] = rowU["TotalCount"];
                                    break;
                                }
                            }
                        }
                    }


                    List<List<object>> resultList = new List<List<object>>();
                    foreach (DataRow row in dataU.Rows)
                    {
                        List<object> rowList = new List<object>();
                        foreach (var item in row.ItemArray)
                        {
                            rowList.Add(item.ToString());
                        }
                        resultList.Add(rowList);
                    }
                    for (int i = 0; i < resultList.Count; i++)
                    {
                        resultList[i][1] = Convert.ToInt32(resultList[i][1]);
                        resultList[i][2] = Convert.ToInt32(resultList[i][2]);
                    }


                    //List<object> oIList1 = new List<object>() // new
                    //{
                    //    (Category==1?"Daily":Category==2?"Weekly":"Monthly"),"Unique Visitors","Total Visitors"
                    //};
                    //resultList.Insert(0, oIList1);
                    return new DALResult<List<List<object>>>(Status.Found, resultList, null, null);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog("Inside GetWebsiteVisit" + ex.Message.ToString());
                return null;
            }
        }
        //public string getVisitorsCount()
        //{
        //    try
        //    {

        //        // set username and password of your google analytics account account.
        //        string userName = "";
        //        string passWord = "";
        //        const string dataFeedUrl = "https://www.google.com/analytics/feeds/data";
        //        AccountQuery query = new AccountQuery();
        //        AnalyticsService service = new AnalyticsService("AnalyticsSampleApp");
        //        if (!string.IsNullOrEmpty(userName))
        //        {
        //            service.setUserCredentials(userName, passWord);
        //        }
        //        string str = "";
        //        AccountFeed accountFeed = service.Query(query);
        //        foreach (AccountEntry entry in accountFeed.Entries)
        //        {
        //            str = entry.ProfileId.Value;
        //        }
        //    }
        //    catch (Exception ex){ Logger.WriteLog(ex.Message.ToString()); return null; }
        //}

    }
    public class GoogleAnalyticsRes
    {
        public string PagePath { get; set; }
        public string PageTitle { get; set; }
        public string ViewCount { get; set; }
    }
    public class GoogleAnalyticsCountryRes
    {
        public string PagePath { get; set; }
        public string PageTitle { get; set; }
        public string ViewCount { get; set; }
    }
}
