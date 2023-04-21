﻿using System.Text.RegularExpressions;
using ceTe.DynamicPDF;
using ceTe.DynamicPDF.PageElements;
using ECOMap.Models;

namespace ECOMap
{
    public class EcoPdfGenerator
    {
        public static void GeneratePDF(ECODataService service, State state, Commodity commodity, int year)
        {
            try 
            {
                ceTe.DynamicPDF.Document document = new ceTe.DynamicPDF.Document();
                Page page1 = new Page(PageSize.Letter, PageOrientation.Portrait, 54.0f);
                document.Pages.Add(page1);
                string labelText1 = $"{state.StateName} {commodity.CommodityName} {year}";
                Label label1 = new Label(labelText1, 0, 0, 504, 100, Font.TimesBold, 20, TextAlign.Center);
                page1.Elements.Add(label1);

                Console.WriteLine("right here 1");
                List<PageGroup> pages = new List<PageGroup>();
                foreach (Price price in service.PriceEntries.Values)
                {
                    if (price.Offer.County.State.Equals(state) && price.Offer.Type.Commodity.Equals(commodity) && price.Offer.Year == year)
                    {
                        Console.WriteLine("right here 2");
                        NAUType type = price.Offer.Type;
                        Practice practice = price.Offer.Practice;
                        PageGroup pg = new PageGroup(practice, type);
                        foreach (PageGroup p in pages)
                        {
                            if (p.Equals(pg))
                            {
                                pages.Remove(p);
                                pg = p;
                                break;
                            }
                        }
                        pg.Prices.Add(price);
                        pages.Add(pg);
                    }
                }
                foreach (PageGroup pg in pages)
                {
                    Console.WriteLine("right here 3");
                    Page page = new Page(PageSize.Letter, PageOrientation.Portrait, 54.0f);
                    document.Pages.Add(page);
                    string labelText = $"{pg.Practice.PracticeName} {pg.Type.TypeName}";
                    Label label = new Label(labelText, 0, 0, 504, 100, Font.TimesBold, 18, TextAlign.Center);
                    page.Elements.Add(label);
                    ESRIClient client = new ESRIClient(state);
                    // foreach (Price price in pg.Prices)
                    // {
                    //     client.RequestParamsList.Add(GetESRIRequstParams(service, commodity, price.Offer.County, price.Offer.Practice, state, year));
                    // }
                    //page.Elements.Add(client.GetImage(50,100));
                    ContentArea legend = GetLegend();
                    page.Elements.Add(legend);

                }
                Console.WriteLine("right here 4");
                document.Draw($"{EcoGeneralService.InitialPathLocation}\\Resources\\Output\\PDFs\\{state.StateName}_{commodity.CommodityName}_{year}_PDF.pdf");
                document.Draw(System.IO.Path.Combine(EcoGeneralService.InitialPathLocation, "Resources", "Output", $"{state.StateName}_{commodity.CommodityName}_{year}_PDF.pdf"));
            } 
            catch(Exception ex) 
            {
                Console.WriteLine(ex);
            }
            
        }
        public static ContentArea GetLegend()
        {
            ContentArea legend = new ContentArea(400, 300, 200, 200);
            legend.Add(new Label("Percent Change", 0, 0, 200, 20, Font.TimesBold, 14, TextAlign.Left));
            legend.Add(new Rectangle(0, 25, 20, 20, RgbColor.Gray, RgbColor.Red, 2, LineStyle.Solid));
            legend.Add(new Label("< -4%", 30, 25, 200, 20, Font.TimesRoman, 12, TextAlign.Left));
            legend.Add(new Rectangle(0, 45, 20, 20, RgbColor.Gray, RgbColor.Coral, 2, LineStyle.Solid));
            legend.Add(new Label("-4% to -2%", 30, 45, 200, 20, Font.TimesRoman, 12, TextAlign.Left));
            legend.Add(new Rectangle(0, 65, 20, 20, RgbColor.Gray, RgbColor.LightPink, 2, LineStyle.Solid));
            legend.Add(new Label("-2% to 0%", 30, 65, 200, 20, Font.TimesRoman, 12, TextAlign.Left));
            legend.Add(new Rectangle(0, 85, 20, 20, RgbColor.Gray, RgbColor.AntiqueWhite, 2, LineStyle.Solid));
            legend.Add(new Label("No Change", 30, 85, 200, 20, Font.TimesRoman, 12, TextAlign.Left));
            legend.Add(new Rectangle(0, 105, 20, 20, RgbColor.Gray, RgbColor.DarkSeaGreen, 2, LineStyle.Solid));
            legend.Add(new Label("0% to 2%", 30, 105, 200, 20, Font.TimesRoman, 12, TextAlign.Left));
            legend.Add(new Rectangle(0, 125, 20, 20, RgbColor.Gray, RgbColor.SeaGreen, 2, LineStyle.Solid));
            legend.Add(new Label("2% to 4%", 30, 125, 200, 20, Font.TimesRoman, 12, TextAlign.Left));
            legend.Add(new Rectangle(0, 145, 20, 20, RgbColor.Gray, RgbColor.DarkGreen, 2, LineStyle.Solid));
            legend.Add(new Label("> 4%", 30, 145, 200, 20, Font.TimesRoman, 12, TextAlign.Left));
            return legend;
        }
        public static void TestLegend()
        {
            ceTe.DynamicPDF.Document document = new ceTe.DynamicPDF.Document();
            Page page = new Page(PageSize.Letter, PageOrientation.Portrait, 54.0f);
            document.Pages.Add(page);
            ContentArea legend = GetLegend();
            page.Elements.Add(legend);
            //document.Draw($"{EcoGeneralService.InitialPathLocation}\\Resources\\Output\\PDFs\\TestLegend.pdf");
            document.Draw(System.IO.Path.Combine(EcoGeneralService.InitialPathLocation, "Resources", "Output", "TestLegend.pdf"));

        }

        public static void GeneratePDFGroup(ECODataService service, string stateName, int year)
        {
            Console.WriteLine("Data Service log 1");
            HashSet<Commodity> commodities = new HashSet<Commodity>();
            State state = null;
            Parallel.ForEach(service.StateEntries, stateIter =>
            {
                if(stateIter.Value.StateName.Equals(stateName)) {
                    state = new State(stateIter.Value.StateCode, stateIter.Value.StateName, stateIter.Value.StateAbbreviation, stateIter.Value.RecordType.RecordTypeCode, stateIter.Value.RecordType);
                }
            });
            foreach (Price price in service.PriceEntries.Values)
            {
                if (price.Offer.County.State.StateName.Equals(stateName) && price.Offer.Year == year && !commodities.Contains(price.Offer.Type.Commodity))
                {
                    Console.WriteLine("inside for loop");
                    commodities.Add(price.Offer.Type.Commodity);
                }
            }
            
            Parallel.ForEach(commodities, commodity =>
            {
                GeneratePDF(service, state, commodity, year);
            });
            

        }

        public static void GenerateAllPDFs(ECODataService service, int year)
        {
            Parallel.ForEach(service.StateEntries.Values, state =>
            {
                GeneratePDFGroup(service, state.StateName, year);
                Console.WriteLine(state.StateName);
            });
        }

        public static ESRIRequestParams GetESRIRequstParams(ECODataService service, Commodity commodity, County county, Practice practice, State state, int year)
        {
            IDictionary<int,Price> values = new Dictionary<int,Price>();
            foreach (Price price in service.PriceEntries.Values)
            {
                Commodity currentCommodity = price.Offer.Practice.Commodity;
                County currentCounty = price.Offer.County;
                Practice currentPractice = price.Offer.Practice;
                State currentState = price.Offer.County.State;
                if (currentCommodity == commodity && currentCounty == county && currentPractice == practice && currentState == state)
                {
                    if (price.Offer.Year == year - 1)
                    {
                        values.Add(year - 1, price);
                    }
                    else if (price.Offer.Year == year)
                    {
                        values.Add(year, price);
                    }
                }
            }
            double percentChange = (values[year].ExpectedIndexValue - values[year - 1].ExpectedIndexValue) / values[year - 1].ExpectedIndexValue;
            return new ESRIRequestParams(county, percentChange);
        }

        
    }
}