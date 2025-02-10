using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace ISOMET_Dual_Tracking_System
{
    internal class SQLAccess
    {
        // Global Config Options
        string location1 = Preferences.Get("Location", string.Empty);
        string name1 = Preferences.Get("Name", string.Empty);

        string server1 = Preferences.Get("Server", string.Empty);

        // Inventory Database
        string database1 = Preferences.Get("Database", string.Empty);
        // Traveler Database
        string database2 = Preferences.Get("Database2", string.Empty);
        // Optical Scanning Databsae
        string database3 = Preferences.Get("Database3", string.Empty);

        bool preAssembly;

        // Variables
        public string? scannedCode;
        string? scannedStatus;

        static HttpClientHandler handler;
        static HttpClient client;

        public SQLAccess()
        {
            handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(30);
        }

        // ----------------------------------------------------------------- //
        //                           SEND SECTION                            //
        // ----------------------------------------------------------------- //

        private async Task scanInventory(string value, string stat)
        {
            try
            {
                //Debug.WriteLine("A");
                var entry = new InventoryEntry
                {
                    Barcode = scannedCode,
                    Name = name1,
                    Location = location1,
                    Status = value,
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                //Debug.WriteLine("B");

                var json = JsonSerializer.Serialize(entry);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //Debug.WriteLine("C");
                
                //Debug.WriteLine("D");
                //Debug.WriteLine($"Sending POST request to {server1}:443/{database1}/Inventory/add");
                var response = await client.PostAsync($"https://{server1}:443/{database1}/Inventory/add", content);
                //Debug.WriteLine("POST request sent.");
                response.EnsureSuccessStatusCode();
                

                if (response.IsSuccessStatusCode)
                {
                    if (location1 != "Pre-Assembly")
                    {
                        await App.Current.MainPage.DisplayAlert("Inventory", $"Successfully scanned {scannedCode} {stat}.", "OK");
                        scannedStatus = stat;
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Inventory", $"Successfully scanned {scannedCode} out.", "OK");
                        scannedStatus = "out";
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Inventory", "Scan failed.", "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                await App.Current.MainPage.DisplayAlert("HTTP Error", $"{httpEx.Message}", "OK");
            }
            catch (TaskCanceledException timeoutEx)
            {
                await App.Current.MainPage.DisplayAlert("Timeout", "The request timed out.", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"{ex.Message}", "OK");
            }
        }

        private async Task scanTraveler(string step, string value)
        {
            try
            {
                var entry = new TravelerEntry
                {
                    Barcode = scannedCode,
                    Name = name1,
                    Step = step,
                    Value = value,
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                var json = JsonSerializer.Serialize(entry);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://{server1}:443/{database2}/Traveler/add", content);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    await App.Current.MainPage.DisplayAlert("Traveler", $"Sucessfully scanned in {scannedCode}.", "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Traveler", "Scan failed.", "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                await App.Current.MainPage.DisplayAlert("HTTP Error", $"{httpEx.Message}", "OK");
            }
            catch (TaskCanceledException timeoutEx)
            {
                await App.Current.MainPage.DisplayAlert("Timeout", "The request timed out.", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"{ex.Message}", "OK");
            }
        }

        private async Task<string> scanSlicesTraveler(string step, string value)
        {
            try
            {
                var entry = new TravelerEntry
                {
                    Barcode = scannedCode,
                    Name = name1,
                    Step = step,
                    Value = value,
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                var json = JsonSerializer.Serialize(entry);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://{server1}:443/{database2}/Traveler/add", content);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var message = "Successfully scanned in slices.";
                    return message;
                }
                else
                {
                    var message = "Scan failed.";
                    return message;
                }
            }
            catch (HttpRequestException httpEx)
            {
                var message = httpEx.Message;
                return message;
            }
            catch (TaskCanceledException timeoutEx)
            {
                var message = "The request timed out.";
                return message;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return message;
            }
        }

        private async Task scanOpticalScanning(string wl, string l1, string l2, string sl)
        {
            try
            {
                var entry = new OScanningEntry
                {
                    Barcode = scannedCode,
                    Name = name1,
                    Wavelength = wl,
                    Laser1 = l1,
                    Laser2 = l2,
                    StaticLoss = sl,
                    Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                var json = JsonSerializer.Serialize(entry);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://{server1}:443/{database3}/OScanning/add", content);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    await App.Current.MainPage.DisplayAlert("Optical Testing", $"Sucessfully scanned in {scannedCode}.", "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Optical Testing", "Scan failed.", "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                await App.Current.MainPage.DisplayAlert("HTTP Error", $"{httpEx.Message}", "OK");
            }
            catch (TaskCanceledException timeoutEx)
            {
                await App.Current.MainPage.DisplayAlert("Timeout", "The request timed out.", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"{ex.Message}", "OK");
            }
        }

        // ----------------------------------------------------------------- //
        //                           READ SECTION                            //
        // ----------------------------------------------------------------- //

        private async Task<string> reportInventory(string text)
        {
            try
            {
                var response = await client.GetAsync($"https://{server1}:443/{database1}/Inventory/get?barcode={text}&report=True");
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Received JSON: {json}");
                    var entries = JsonSerializer.Deserialize<InventoryEntry[]>(json);

                    if (entries != null && entries.Length > 0)
                    {
                        StringBuilder messageBuilder = new StringBuilder();
                        foreach (var entry in entries)
                        {
                            messageBuilder.AppendLine($"ID: {entry.Barcode}\nName: {entry.Name}\nLocation: {entry.Location}\nStatus: {entry.Status}\nDate: {entry.Date}\n");
                            Debug.WriteLine($"Entry: {entry.Barcode}, {entry.Name}, {entry.Location}, {entry.Status}, {entry.Date}");
                        }
                        string message = messageBuilder.ToString();
                        return message;
                    }
                    else
                    {
                        return $"{text} not found in database.";
                    }
                }
                else
                {
                    return "Error connecting to database.";
                }
            }
            catch (HttpRequestException httpEx)
            {
                return $"{httpEx.Message}";
            }
            catch (TaskCanceledException timeoutEx)
            {
                return "The request timed out.";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        private async Task<string> reportInventoryRange(int start, int end)
        {
            try
            {
                var response = await client.GetAsync($"https://{server1}:443/{database1}/Inventory/getrange?startRange={start}&endRange={end}");
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Received JSON: {json}");
                    var entries = JsonSerializer.Deserialize<InventoryEntry[]>(json);

                    if (entries != null && entries.Length > 0)
                    {
                        StringBuilder messageBuilder = new StringBuilder();
                        foreach (var entry in entries)
                        {
                            messageBuilder.AppendLine($"ID: {entry.Barcode}\nName: {entry.Name}\nLocation: {entry.Location}\nStatus: {entry.Status}\nDate: {entry.Date}\n");
                            Debug.WriteLine($"Entry: {entry.Barcode}, {entry.Name}, {entry.Location}, {entry.Status}, {entry.Date}");
                        }
                        string message = messageBuilder.ToString();
                        return message;
                    }
                    else
                    {
                        return $"Unable to search given range.";
                    }
                }
                else
                {
                    return "Error connecting to database.";
                }
            }
            catch (HttpRequestException httpEx)
            {
                return $"{httpEx.Message}";
            }
            catch (TaskCanceledException timeoutEx)
            {
                return "The request timed out.";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        private async Task<string> reportTraveler(string text)
        {
            try
            {
                var response = await client.GetAsync($"https://{server1}:443/{database2}/Traveler/get?barcode={text}&report=True");
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var entries = JsonSerializer.Deserialize<TravelerEntry[]>(json);

                    if (entries != null && entries.Length > 0)
                    {
                        StringBuilder messageBuilder = new StringBuilder();
                        foreach (var entry in entries)
                        {
                            messageBuilder.AppendLine($"ID: {entry.Barcode}\nName: {entry.Name}\nStep: {entry.Step}\nValue: {entry.Value}\nDate: {entry.Date}\n");
                            Debug.WriteLine($"Entry: {entry.Barcode} reported by {entry.Name} at step {entry.Step} with reported value {entry.Value} on {entry.Date}");
                        }
                        string message = messageBuilder.ToString();
                        return message;
                    }
                    else
                    {
                        return $"{text} not found in database.";
                    }
                }
                else
                {
                    return "Error connecting to database.";
                }
            }
            catch (HttpRequestException httpEx)
            {
                return $"{httpEx.Message}";
            }
            catch (TaskCanceledException timeoutEx)
            {
                return "The request timed out.";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        private async Task<string> reportOS(string text)
        {
            try
            {
                var response = await client.GetAsync($"https://{server1}:443/{database3}/OScanning/get?barcode={text}&report=False");
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var entries = JsonSerializer.Deserialize<OScanningEntry[]>(json);

                    if (entries != null && entries.Length > 0)
                    {
                        StringBuilder messageBuilder = new StringBuilder();
                        foreach (var entry in entries)
                        {
                            messageBuilder.AppendLine($"ID: {entry.Barcode}\nName: {entry.Name}\nWavelength: {entry.Wavelength}\nValue 1: {entry.Laser1}\nValue 2: {entry.Laser2}\nStatic Loss: {entry.StaticLoss}\nDate: {entry.Date}\n");
                            Debug.WriteLine($"Entry: {entry.Barcode} by {entry.Name} with wavelength {entry.Wavelength} with value 1 {entry.Laser1} with value 2 {entry.Laser2} with {entry.StaticLoss} on {entry.Date}.");
                        }
                        string message = messageBuilder.ToString();
                        return message;
                    }
                    else
                    {
                        return $"{text} not found in database.";
                    }
                }
                else
                {
                    return "Error connecting to database.";
                }
            }
            catch (HttpRequestException httpEx)
            {
                return $"{httpEx.Message}";
            }
            catch (TaskCanceledException timeoutEx)
            {
                return "The request timed out.";
            }
            catch (Exception ex)
            {
                return $"{ex.Message}";
            }
        }

        private async Task searchInventory(string text)
        {
            try
            {
                var response = await client.GetAsync($"https://{server1}:443/{database1}/Inventory/get?barcode={text}&report=False");
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Received JSON: {json}");
                    var entries = JsonSerializer.Deserialize<InventoryEntry[]>(json);

                    if (entries != null && entries.Length > 0)
                    {
                        StringBuilder messageBuilder = new StringBuilder();
                        foreach (var entry in entries)
                        {
                            messageBuilder.AppendLine($"ID: {entry.Barcode}\nName: {entry.Name}\nLocation: {entry.Location}\nStatus: {entry.Status}\nDate: {entry.Date}");
                            Debug.WriteLine($"Entry: {entry.Barcode}, {entry.Name}, {entry.Location}, {entry.Status}, {entry.Date}");
                        }
                        string message = messageBuilder.ToString();
                        await App.Current.MainPage.DisplayAlert("Inventory", message, "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Inventory", $"{text} not found in database.", "OK");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Inventory", "Error connecting to database.", "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                await App.Current.MainPage.DisplayAlert("HTTP Error", $"{httpEx.Message}", "OK");
            }
            catch (TaskCanceledException timeoutEx)
            {
                await App.Current.MainPage.DisplayAlert("Timeout", "The request timed out.", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"{ex.Message}", "OK");
            }
        }

        private async Task searchTraveler(string text)
        {
            try
            {
                var response = await client.GetAsync($"https://{server1}:443/{database2}/Traveler/get?barcode={text}&report=False");
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var entries = JsonSerializer.Deserialize<TravelerEntry[]>(json);

                    if (entries != null && entries.Length > 0)
                    {
                        StringBuilder messageBuilder = new StringBuilder();
                        foreach (var entry in entries)
                        {
                            messageBuilder.AppendLine($"ID: {entry.Barcode}\nName: {entry.Name}\nStep: {entry.Step}\nValue: {entry.Value}\nDate: {entry.Date}");
                            Debug.WriteLine($"Entry: {entry.Barcode} reported by {entry.Name} at step {entry.Step} with reported value {entry.Value} on {entry.Date}");
                        }
                        string message = messageBuilder.ToString();
                        await App.Current.MainPage.DisplayAlert("Traveler", message, "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Traveler", $"{text} not found in database.", "OK");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Traveler", "Error connecting to database.", "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                await App.Current.MainPage.DisplayAlert("HTTP Error", $"{httpEx.Message}", "OK");
            }
            catch (TaskCanceledException timeoutEx)
            {
                await App.Current.MainPage.DisplayAlert("Timeout", "The request timed out.", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"{ex.Message}", "OK");
            }
        }

        private async Task searchOS(string text)
        {
            try
            {
                var response = await client.GetAsync($"https://{server1}:443/{database3}/OScanning/get?barcode={text}&report=False"); // URI says OScanning instead of OTesting because Optical Testing was formerly named Optical Scanning
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var entries = JsonSerializer.Deserialize<OScanningEntry[]>(json);

                    if (entries != null && entries.Length > 0)
                    {
                        StringBuilder messageBuilder = new StringBuilder();
                        foreach (var entry in entries)
                        {
                            messageBuilder.AppendLine($"ID: {entry.Barcode}\nName: {entry.Name}\nWavelength: {entry.Wavelength}\nValue 1: {entry.Laser1}\nValue 2: {entry.Laser2}\nStatic Loss: {entry.StaticLoss}\nDate: {entry.Date}.");
                            Debug.WriteLine($"Entry: {entry.Barcode} by {entry.Name} with wavelength {entry.Wavelength} with value 1 {entry.Laser1} with value 2 {entry.Laser2} with {entry.StaticLoss} on {entry.Date}.");
                        }
                        string message = messageBuilder.ToString();
                        await App.Current.MainPage.DisplayAlert("Optical Testing", message, "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Optical Testing", $"{text} not found in database.", "OK");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Optical Testing", "Error connecting to database.", "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                await App.Current.MainPage.DisplayAlert("HTTP Error", $"{httpEx.Message}", "OK");
            }
            catch (TaskCanceledException timeoutEx)
            {
                await App.Current.MainPage.DisplayAlert("Timeout", "The request timed out.", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"{ex.Message}", "OK");
            }
        }

        public async Task<string> scannerStatusChecker(string text)
        {
            try
            {
                Debug.WriteLine("Starting scannerStatusChecker");
                Debug.WriteLine($"Server URL: https://{server1}:443/{database1}/Inventory/get?barcode={text}");

                var response = await client.GetAsync($"https://{server1}:443/{database1}/Inventory/get?barcode={text}&report=False");
                Debug.WriteLine("Received response.");
                response.EnsureSuccessStatusCode();

                string status = string.Empty;

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Received JSON: {json}");
                    var entries = JsonSerializer.Deserialize<InventoryEntry[]>(json);

                    if (entries != null && entries.Length > 0)
                    {
                        var entry = entries[0];
                        Debug.WriteLine($"Entry: {entry.Barcode}, {entry.Name}, {entry.Location}, {entry.Status}, {entry.Date}");
                        if (entry.Status == "IN")
                        {
                            if (entry.Name != name1)
                            {
                                status = "OUT"; // Just like with V2 and V1 of the scanning system, OUT and IN are inverted still
                            }
                            else
                            {
                                status = entry.Status;
                            }
                        }
                        else
                        {
                            status = entry.Status;
                        }
                        if (entry.Location == "Pre-Assembly" || location1 == "Pre-Assembly")
                        {
                            status = "OUT";
                            preAssembly = true;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Error connecting to database.");
                    }
                }
                else
                {
                    Debug.WriteLine("Caught exception");
                }

                return status;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("DEBUG", $"{ex.Message}", "OK");
                return null;
            }
        }


        // ----------------------------------------------------------------- //
        //                          PUBLIC METHODS                           //
        // ----------------------------------------------------------------- //

        public async Task scannerMethod(string value)
        {
            Debug.WriteLine("AA");
            scannedCode = value;

            if (value != "")
            {
                string status = await scannerStatusChecker(value);
                if (status == "IN")
                {
                    if (preAssembly)
                    {
                        await scanInventory("IN", "in");
                    }
                    else
                    {
                        await scanInventory("OUT", "out");
                    }
                }
                else
                {
                    await scanInventory("IN", "in");
                }
            }
            Debug.WriteLine("BB");
        }

        public async Task readInventory(string value)
        {
            await searchInventory(value);
        }

        public async Task<string> repInventory(string value)
        {
            string report = await reportInventory(value);
            return report;
        }

        public async Task<string> repInventoryRange(int value1, int value2) // Range function for Inventory
        {
            string report = await reportInventoryRange(value1, value2);
            return report;
        }

        public async Task travelerMethod(string process, string value)
        {
            await scanTraveler(process, value);
        }

        public async Task<string> slicesTravelerMethod(string process, string value)
        {
            string status = await scanSlicesTraveler(process, value);
            return status;
        }

        public async Task readTraveler(string value)
        {
            await searchTraveler(value);
        }

        public async Task<string> repTraveler(string value)
        {
            string report = await reportTraveler(value);
            return report;
        }

        public async Task oscanningMethod(string val1, string val2, string val3, string val4)
        {
            await scanOpticalScanning(val1, val2, val3, val4);
        }

        public async Task readOScanning(string value)
        {
            await searchOS(value);
        }

        public async Task<string> repOS(string value)
        {
            string report = await reportOS(value);
            return report;
        }
    }
}

public class InventoryEntry
{
    [JsonPropertyName("barcode")]
    public string Barcode { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }
}

public class TravelerEntry
{
    [JsonPropertyName("barcode")]
    public string Barcode { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("step")]
    public string Step { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }
}

public class OScanningEntry
{
    [JsonPropertyName("barcode")]
    public string Barcode { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("wavelength")]
    public string Wavelength { get; set; }

    [JsonPropertyName("laser1")]
    public string Laser1 { get; set; }

    [JsonPropertyName("laser2")]
    public string Laser2 { get; set; }

    [JsonPropertyName("staticloss")]
    public string StaticLoss { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }
}



// ----------------------------------------------------------------- //
//                        INVENTORY SECTION                          //
// ----------------------------------------------------------------- //


/*
private async void searchInventory(string text)
{
    string connectionString = $@"Data Source={database1};";
    string query = $"SELECT TOP 1 * FROM Inventory WHERE barcode = '{text}' ORDER BY date DESC";

    bool wasRead = false;

    try
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand(query, connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.FieldCount > 0)
                    {
                        string message = $"{reader["barcode"].ToString()} reported by {reader["name"].ToString()} at {reader["location"].ToString()} with status {reader["status"].ToString()} on {reader["date"].ToString()}.";

                        await App.Current.MainPage.DisplayAlert("Inventory", message, "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Inventory", "Error connecting to database.", "OK");
                    }
                    wasRead = true;
                }

                if (!reader.Read() && !wasRead)
                {
                    await App.Current.MainPage.DisplayAlert("Inventory", $"{text} not found in database.", "OK");
                }
            }
        }
    }
    catch
    {
        await App.Current.MainPage.DisplayAlert("Traveler", "Caught exception at Traveler Search", "OK");
    }
}

private string scannerStatusChecker(string text)
{
    string connectionString = $@"Data Source={database1};";
    string query = $"SELECT TOP 1 * FROM Inventory WHERE barcode = '{text}' ORDER BY Date DESC";

    string status = "";

    try
    {
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.FieldCount > 0)
                    {
                        if (reader["Status"].ToString() == "IN")
                        {
                            if (reader["Name"].ToString() != name1)
                            {
                                status = "OUT"; // Just like with V2 and V1 of the scanning system, OUT and IN are inverted still
                            } 
                            else
                            {
                                status = reader["Status"].ToString();
                            }
                        }
                        else
                        {
                            status = reader["Status"].ToString();
                        }
                        if (reader["Location"].ToString() == "Pre-Assembly" || location1 == "Pre-Assembly")
                        {
                            status = "OUT";
                            preAssembly = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error connecting to database.");
                    }
                }
                return status;
            }
        }
    }
    catch
    {
        Console.WriteLine("Caught exception");
        return status;
    }
}

private void scanInventory(string value, string stat)
{
    string connectionString = $@"Data Source=/sdcard/Documents/InventoryDatabase.sql;";
    string query = "INSERT INTO Inventory (barcode, name, location, status, date) VALUES (@barcode, @name, @location, @status, @date)";

    string? barcode = scannedCode;
    string status = value;
    string scanLocation = location1;

    try
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@barcode", barcode);
                command.Parameters.AddWithValue("@name", name1);
                command.Parameters.AddWithValue("@location", location1);
                if (!preAssembly)
                {
                    command.Parameters.AddWithValue("@status", status);
                }
                else
                {
                    if (location1 != "Pre-Assembly")
                    {
                        command.Parameters.AddWithValue("@status", "IN");
                        preAssembly = false;
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@status", "OUT");
                    }
                }
                command.Parameters.AddWithValue("@date", DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"));

                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    if (scanLocation != "Pre-Assembly")
                    {
                        Console.WriteLine($"Successfully scanned item {stat}");
                        scannedStatus = stat;
                    }
                    else
                    {
                        Console.WriteLine("Successfully scanned item out");
                        scannedStatus = "out";
                    }
                }
                else
                {
                    // await App.Current.MainPage.DisplayAlert("Inventory", "Scan failed.", "OK");
                }
            }
        }
    }
    catch (Exception ex)
    {
        // await App.Current.MainPage.DisplayAlert("Inventory", ex.ToString(), "OK");
        Debug.WriteLine(ex.InnerException?.ToString());
    }
}

public void scannerMethod(string value)
{
    scannedCode = value;

    if (value != "")
    {
        if (scannerStatusChecker(value) == "IN")
        {
            if (preAssembly)
            {
                scanInventory("IN", "in");
            }
            else
            {
                scanInventory("OUT", "out");
            }
        }
        else
        {
            scanInventory("IN", "in");
        }
    }
}

public void readInventory(string value)
{
    searchInventory(value);
}
*/

// ----------------------------------------------------------------- //
//                         TRAVELER SECTION                          //
// ----------------------------------------------------------------- //

/*
private async void searchTraveler(string text)
{
    string connectionString = $@"Data Source={database2};";
    string query = $"SELECT TOP 1 * FROM Traveler WHERE barcode = '{text}' ORDER BY date DESC";

    bool wasRead = false;

    try
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand(query, connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.FieldCount > 0)
                    {
                        string message = $"{reader["barcode"].ToString()} reported by {reader["name"].ToString()} at step {reader["step"].ToString()} with reported value {reader["value"].ToString()} on {reader["date"].ToString()}.";

                        await App.Current.MainPage.DisplayAlert("Traveler", message, "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Traveler", "Error connecting to database.", "OK");
                    }
                    wasRead = true;
                }

                if (!reader.Read() && !wasRead)
                {
                    await App.Current.MainPage.DisplayAlert("Traveler", $"{text} not found in database.", "OK");
                }
            }
        }
    }
    catch
    {
        await App.Current.MainPage.DisplayAlert("Traveler", "Caught exception at Traveler Search", "OK");
    }
}

private async void scanTraveler(int step, string value)
{
    string connectionString = $@"Data Source={database2};";
    string query = "INSERT INTO Traveler (barcode, name, step, value, date) VALUES (@barcode, @name, @step, @value, @date)";

    string? barcode = scannedCode;
    string strStep = step.ToString();

    try
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@barcode", barcode);
                command.Parameters.AddWithValue("@name", name1);
                command.Parameters.AddWithValue("@step", strStep);
                command.Parameters.AddWithValue("@value", value);
                command.Parameters.AddWithValue("@date", DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"));

                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    Console.WriteLine("Successfully scanned crystal");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Traveler", "Scan failed.", "OK");
                }
            }
        }
    }
    catch
    {
        await App.Current.MainPage.DisplayAlert("Traveler", "Caught exception at Traveler Data Entry", "OK");
    }
}

public void travelerMethod(string process, string value)
{
    int step;

    switch (process)
    {
        case "1 Fab - Fabrication":
            step = 1;
            break;
        case "2 Fab - Off Axis Angle":
            step = 2;
            break;
        case "3 Fab - Mark Blocks":
            step = 3;
            break;
        case "4 Pol - Optical Polish":
            step = 4;
            break;
        case "5 QC - Inspect No AR":
            step = 5;
            break;
        case "6 Coat - AR Machine":
            step = 6;
            break;
        case "7 Coat - AR Vacuum":
            step = 7;
            break;
        case "8 Coat - Material/Thick":
            step = 8;
            break;
        case "9 Coat - AR Temp":
            step = 9;
            break;
        case "10 Coat - Actual Side 1":
            step = 10;
            break;
        case "11 Coat - Actual Side 2":
            step = 11;
            break;
        case "12 QC - Side A Min refl":
            step = 12;
            break;
        case "13 QC - Side B Min refl":
            step = 13;
            break;
        case "14 QC - Inspect AR":
            step = 14;
            break;
        case "15 Bond - Coat Op. Surf.":
            step = 15;
            break;
        case "16 Bond - Bonder/Runs":
            step = 16;
            break;
        case "17 Bond - Oxide/Seed":
            step = 17;
            break;
        case "18 Bond - Metal/Thickness":
            step = 18;
            break;
        case "19 Bond - Deposit Total":
            step = 19;
            break;
        case "20 Bond - Pressure":
            step = 20;
            break;
        case "21 Bond - Final Vacuum":
            step = 21;
            break;
        case "22 QC - Inspect Bond":
            step = 22;
            break;
        case "23 Pol - Lap Thickness":
            step = 23;
            break;
        case "24 Fab - Cut Angle":
            step = 24;
            break;
        case "25 Fab - Inspect":
            step = 25;
            break;
        case "26 Fab - Slicing":
            step = 26;
            break;
        case "27 Fab - Mark Slices":
            step = 27;
            break;
        case "28 Fab - Comp Angle":
            step = 28;
            break;
        case "29 Pol - Cleaning":
            step = 29;
            break;
        case "30 Bond - Absorber Bond":
            step = 30;
            break;
        case "31 Bond - Electrode Mask":
            step = 31;
            break;
        case "32 QC - Inspect Bond":
            step = 32;
            break;
        case "33 QC - Raw VSWR":
            step = 33;
            break;
        case "34 Assy - Attach Leads":
            step = 34;
            break;
        case "35 Assy - Mount in Case":
            step = 35;
            break;
        case "36 QC - Tune":
            step = 36;
            break;
        case "37 QC - Clean Optics":
            step = 37;
            break;
        case "38 QC - Mech. Inspect":
            step = 38;
            break;
        case "39 QC - Savvy Scr/Dig":
            step = 39;
            break;
        case "40 QC - Water Test":
            step = 40;
            break;
        case "41 QC - Attach Cover":
            step = 41;
            break;
        case "42 QC - Optical Test":
            step = 42;
            break;
        case "43 QC - Tuning Test":
            step = 43;
            break;
        case "44 QC - Serialize":
            step = 44;
            break;
        case "45 QC - Final Inspection":
            step = 45;
            break;
        default:
            step = 0;
            break;
    }
    scanTraveler(step, value);
}

public void readTraveler(string value)
{
    searchTraveler(value);
}
*/

// ----------------------------------------------------------------- //
//                     OPTICAL SCANNING SECTION                      //
// ----------------------------------------------------------------- //

/*
private async void searchOS(string text)
{
    string connectionString = $@"Data Source={database3};";
    string query = $"SELECT TOP 1 * FROM OScanning WHERE barcode = '{text}' ORDER BY date DESC";

    bool wasRead = false;

    try
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand(query, connection))
            {
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.FieldCount > 0)
                    {
                        string message = $"{reader["barcode"].ToString()} by {reader["name"].ToString} with wavelength {reader["wavelength"].ToString()} with value 1 {reader["laser1"].ToString()} with value 2 {reader["laser2"].ToString()} with {reader["staticloss"].ToString()} on {reader["date"].ToString()}.";

                        await App.Current.MainPage.DisplayAlert("Optical Scanning", message, "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Optical Scanning", "Error connecting to database.", "OK");
                    }
                    wasRead = true;
                }

                if (!reader.Read() && !wasRead)
                {
                    await App.Current.MainPage.DisplayAlert("Optical Scanning", $"{text} not found in database.", "OK");
                }
            }
        }
    }
    catch
    {
        await App.Current.MainPage.DisplayAlert("Optical Scanning", "Caught exception at Optical Scanning Search", "OK");
    }
}

private async void scanOpticalScanning(string wl, string l1, string l2, string sl)
{
    string connectionString = $@"Data Source={database3};";
    string query = "INSERT INTO OScanning (barcode, name, wavelength, laser1, laser2, staticloss, date) VALUES (@barcode, @name, @wavelength, @laser1, @laser2, @staticloss, @date)";

    string? barcode = scannedCode;
    string waveLength = wl;
    string laser1 = l1;
    string laser2 = l2;
    string staticLoss = sl;

    try
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@barcode", barcode);
                command.Parameters.AddWithValue("@name", name1);
                command.Parameters.AddWithValue("@wavelength", waveLength);
                command.Parameters.AddWithValue("@laser1", laser1);
                command.Parameters.AddWithValue("@laser2", laser2);
                command.Parameters.AddWithValue("@staticloss", staticLoss);
                command.Parameters.AddWithValue("@date", DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"));

                int result = command.ExecuteNonQuery();

                if (result > 0)
                {
                    Console.WriteLine("Successfully scanned crystal");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Optical Scanning", "Scan failed.", "OK");
                }
            }
        }
    }
    catch
    {
        await App.Current.MainPage.DisplayAlert("Optical Scanning", "Caught exception at Optical Scanning Data Entry", "OK");
    }
}

public void oscanningMethod(string val1, string val2, string val3, string val4)
{
    scanOpticalScanning(val1, val2, val3, val4);
}

public void readOScanning(string value)
{
    searchOS(value);
}
*/