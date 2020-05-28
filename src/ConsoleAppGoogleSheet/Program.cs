using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppGoogleSheet
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        static SheetsService service;

        static readonly string _sheetId = "1gE8UQb3w5fAdEvOmVcIGZIvZfOqGg3nU2m4nk_CSjkM";

        static readonly string _sheet = "legislators-current";
        static readonly string _sheetUpdate = "Update";

        static async Task Main(string[] args)
        {
            await Authenticate();

            ReadSheet();

            CreateEntry();
            UpdateEntry();
            DeleteEntry();
            Console.ReadKey();
        }

        static Task Authenticate()
        {
            GoogleCredential credential;
            string credFile = "project-b4d2409aa25b.json";//"client_secret.json";
            using (var stream = new FileStream(credFile, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return Task.CompletedTask;
            //UserCredential credential;

            //using (var stream =
            //    new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            ////new FileStream("project-b4d2409aa25b.json", FileMode.Open, FileAccess.Read))
            //{
            //    // The file token.json stores the user's access and refresh tokens, and is created
            //    // automatically when the authorization flow completes for the first time.
            //    string credPath = Path.Combine(Directory.GetCurrentDirectory(), "token.json");
            //    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            //        GoogleClientSecrets.Load(stream).Secrets,
            //        Scopes,
            //        "user",
            //        CancellationToken.None,
            //        new FileDataStore(credPath, true));
            //    Console.WriteLine("Credential file saved to: " + credPath);
            //}

            //return credential;

            //// Create Google Sheets API service.
            //var service = new SheetsService(new BaseClientService.Initializer()
            //{
            //    HttpClientInitializer = credential,
            //    ApplicationName = ApplicationName,
            //});
        }

        private static void ReadSheet()
        {
            // Define request parameters.
            var range = $"{_sheet}!A1:F10";
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(_sheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                Console.WriteLine("Name, Major");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}", row[0], row[4]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
        }

        static void CreateEntry()
        {
            var range = $"{_sheetUpdate}!A:F";
            var valueRange = new ValueRange();

            var oblist = new List<object>() { "Hello!", "This", "was", "insertd", "via", "C#" };
            valueRange.Values = new List<IList<object>> { oblist };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, _sheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = appendRequest.Execute();
        }

        static void UpdateEntry()
        {
            var range = $"{_sheetUpdate}!D543";
            var valueRange = new ValueRange();

            var oblist = new List<object>() { "updated" };
            valueRange.Values = new List<IList<object>> { oblist };

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, _sheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = updateRequest.Execute();
        }

        static void DeleteEntry()
        {
            var range = $"{_sheetUpdate}!A543:F";
            var requestBody = new ClearValuesRequest();

            var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, _sheetUpdate, range);
            var deleteReponse = deleteRequest.Execute();
        }

        static void ReadEntries()
        {
            var range = $"{_sheet}!A1:F10";
        }
    }
}

