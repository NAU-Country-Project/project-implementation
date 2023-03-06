using System.Data;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.FileIO;
using NAUCountryA;
using NAUCountryA.Models;
using NAUCountryA.Tables;
using Npgsql;
<<<<<<< HEAD
Service.ConstructUser();
=======
//CreatePDF.Run();
//Service.ConstructUser();
ICollection<string> commodityDataSet = Service.ToCollection("A23_Commodity");
Console.WriteLine(commodityDataSet.Count);
ICollection<int> commodityIds = new HashSet<int>();
IEnumerator<string> commodityEnum = commodityDataSet.GetEnumerator();
if (commodityEnum.MoveNext())
{
    string[] headers = commodityEnum.Current.Split(',');
    while (commodityEnum.MoveNext())
    {
        string[] values = commodityEnum.Current.Split(',');
        commodityIds.Add(Convert.ToInt32(values[4]));
    }
    Console.WriteLine(commodityIds.Count);
}
// string sqlCommand = $"SELECT * FROM public.\"State\" WHERE \"STATE_CODE\" = 01;";
// System.Data.DataTable table = Service.GetDataTable(sqlCommand);
// Service.GeneratePDF(new State(table.Rows[0]));
>>>>>>> b2a5fd01bc87f883e22bbfe499628390856aedf1

Service.ConstructUser();
Price testPrice = Service.PriceEntries.Values.First();
Service.GeneratePDF(testPrice.Offer.State, testPrice.Offer.Practice, testPrice.Offer.Type);


/*var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
*/