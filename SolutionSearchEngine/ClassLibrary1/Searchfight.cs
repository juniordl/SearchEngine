using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Entity;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace Models
{
    public class Searchfight
    {
        private List<Seacher> seachers;

        public string FindResults() {
            //get api config from json

            string jsonString = "";

            jsonString = File.ReadAllText("ApiConfig.js");
            seachers = JsonSerializer.Deserialize<Seacher>(jsonString);
        }
    }
}
