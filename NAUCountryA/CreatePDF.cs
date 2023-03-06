using System;
using Microsoft.VisualBasic.FileIO;
using NAUCountryA.Exceptions;
using NAUCountryA.Models;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;

namespace NAUCountryA
{
    public class CreatePDF
    {
        /*
        public static void Run()
        {
            Document document = new Document();

            Page page = new Page(PageSize.Letter, PageOrientation.Portrait, 54.0f);
            document.Pages.Add(page);

            string state = "State";
            string commodity = "Commodity";
            string practice = "Practice";
            string title = "2023 Expected Yield";
            string map = "Resources/TestImage.png";
            Label label1 = new Label(state + ", " + commodity + ", " + practice, 0, 0, 504, 100, Font.Helvetica, 15, TextAlign.Left);
            page.Elements.Add(label1);
            Label label4 = new Label(title, 0, 20, 504, 100, Font.Helvetica, 15, TextAlign.Left);
            page.Elements.Add(label4);
            Image image = new Image((Util.GetPath(map)), 0, 40);
            page.Elements.Add(image);
            document.Draw(Util.GetPath("PDFOutput/TestPDF.pdf"));
        }

        class Util
        {
            // This is a helper function to get the full path to a file from the root of the project.
            internal static string GetPath(string filePath)
            {
                var exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
                var appRoot = appPathMatcher.Match(exePath).Value;
                return System.IO.Path.Combine(appRoot, filePath);
            }

        } */
    }
}
