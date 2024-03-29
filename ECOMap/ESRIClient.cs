using ECOMap.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ceTe.DynamicPDF.PageElements;

namespace ECOMap
{
    public class ESRIClient
    {
        private readonly State state;
        public ESRIClient(State state)
        {
            RequestParamsList = new HashSet<ESRIRequestParams>();
            this.state = state;
        }

        public ICollection<ESRIRequestParams> RequestParamsList
        {
            get;
            private set;
        }

        public Image GetImage(float x, float y)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage result = client.GetAsync(UrlConent.ToString()).Result;
            byte[] fileBytes = result.Content.ReadAsByteArrayAsync().Result;
            //string filePath = $"{EcoGeneralService.InitialPathLocation}\\Resources\\Output\\Images\\{Guid.NewGuid()}.jpg";
            string filePath = System.IO.Path.Combine(EcoGeneralService.InitialPathLocation, "Resources", "Output", "Images", $"{Guid.NewGuid()}.jpg");
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(filePath));
            writer.Write(fileBytes);
            Console.WriteLine("Image done");
            writer.Close();
            return new Image(filePath, x, y);
        }

        private JObject ESRIRequest
        {
            get
            {
                JObject obj;
                //string filePath = $"{EcoGeneralService.InitialPathLocation}\\Resources\\ESRIRequest.json";
                string filePath = System.IO.Path.Combine(EcoGeneralService.InitialPathLocation, "Resources", "ESRIRequest.json");
                using (StreamReader file = File.OpenText(filePath))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    obj = (JObject)JToken.ReadFrom(reader);
                }

                String stateCode = EcoGeneralService.RemoveQuotationsFromCurrentFormat(state.FormatStateCode());
                UpdateUniqueValueInfos((JObject)(obj["operationalLayers"]?[0]?["layerDefinition"]?["drawingInfo"]?["renderer"]));
                obj["operationalLayers"][0]["layerDefinition"]["definitionExpression"] = $"CNT_STATE_ = '{stateCode}'";
                obj["mapOptions"]["extent"] = Extent["extent"];
                obj["exportOptions"]["outputSize"] = GetOutputSize(state);
                Console.WriteLine(obj);
                return obj;
            }
        }
        public JArray GetOutputSize(State state)
        {
            JArray sizes = new JArray();
            if (state.StateName.Equals("Alaska") ||
            state.StateName.Equals("Texas"))
            {
                sizes.Add(682);
                sizes.Add(698);
            }
            else if (state.StateName.Equals("Alabama"))
            {
                sizes.Add(264);
                sizes.Add(270);

            }
            else if (state.StateName.Equals("South Dakota") ||
            state.StateName.Equals("Florida"))
            {
                sizes.Add(381);
                sizes.Add(389);

            }
            else if (state.StateName.Equals("Virginia"))
            {
                sizes.Add(421);
                sizes.Add(429);

            }
            else if (state.StateName.Equals("Arizona") ||
            state.StateName.Equals("New Mexico")  ||
            state.StateName.Equals("Illinois") ||
            state.StateName.Equals("Wisconsin"))
            {
                sizes.Add(321);
                sizes.Add(329);

            }
            else if (state.StateName.Equals("Washington") ||
            state.StateName.Equals("Colorado") ||
            state.StateName.Equals("Wyoming") ||
            state.StateName.Equals("Nebraska") ||
            state.StateName.Equals("North Dakota") ||
            state.StateName.Equals("Kansas") ||
            state.StateName.Equals("Oklahoma") ||            
            state.StateName.Equals("North Carolina") ||
            state.StateName.Equals("Michigan") ||
            state.StateName.Equals("Kentucky") ||
            state.StateName.Equals("Missouri") ||
            state.StateName.Equals("New York")
            )
            {
                sizes.Add(341);
                sizes.Add(349);

            }
            else
            {
                sizes.Add(256);
                sizes.Add(262);
            }

            return sizes;
        }

        private JObject Extent
        {
            get
            {
                HttpClient client = new HttpClient();
                string codeFormat = EcoGeneralService.RemoveQuotationsFromCurrentFormat(state.FormatStateCode());
                string queryUrl = $"https://services3.arcgis.com/hDXM0jVzBTOKdR4i/arcgis/rest/services/NAUMap/FeatureServer/1/query?where=CNT_STATE_+%3D+%27{codeFormat}%27&objectIds=&time=&geometry=&geometryType=esriGeometryEnvelope&inSR=&spatialRel=esriSpatialRelIntersects&resultType=none&distance=0.0&units=esriSRUnit_Meter&relationParam=&returnGeodetic=false&outFields=&returnGeometry=true&returnCentroid=false&featureEncoding=esriDefault&multipatchOption=xyFootprint&maxAllowableOffset=&geometryPrecision=&outSR=&defaultSR=&datumTransformation=&applyVCSProjection=false&returnIdsOnly=false&returnUniqueIdsOnly=false&returnCountOnly=false&returnExtentOnly=true&returnQueryGeometry=false&returnDistinctValues=false&cacheHint=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&having=&resultOffset=&resultRecordCount=&returnZ=false&returnM=false&returnExceededLimitFeatures=true&quantizationParameters=&sqlFormat=none&f=pjson&token= ";
                HttpResponseMessage response = client.GetAsync(queryUrl).Result;
                string content = response.Content.ReadAsStringAsync().Result;
                return JObject.Parse(content);
            }
        }
        private JToken UrlConent
        {
            get
            {
                HttpClient client = new HttpClient();
                FormUrlEncodedContent encodedContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("Web_Map_as_JSON", ESRIRequest.ToString(Formatting.None)),
                    new KeyValuePair<string, string>("Format", "JPG"),
                    new KeyValuePair<string, string>("Layout_Template", "MAP_ONLY"),
                    new KeyValuePair<string, string>("f", "pjson")
                });
                string mapUrl = "https://sampleserver6.arcgisonline.com/arcgis/rest/services/Utilities/PrintingTools/GPServer/Export%20Web%20Map%20Task/execute";
                HttpResponseMessage response = client.PostAsync(mapUrl, encodedContent).Result;
                string content = response.Content.ReadAsStringAsync().Result;
                JObject pngResult = JObject.Parse(content);
                return pngResult["results"][0]["value"]["url"];
            }
        }

        private void UpdateUniqueValueInfos(JObject renderer)
        {
            renderer.Remove("uniqueValueInfos");
            JArray uniqueValueInfos = new JArray();
            foreach (ESRIRequestParams request in RequestParamsList)
            {
                uniqueValueInfos.Add(request.UniqueValueInfo);
            }
            renderer.Add("uniqueValueInfos", uniqueValueInfos);
        }
    }
}