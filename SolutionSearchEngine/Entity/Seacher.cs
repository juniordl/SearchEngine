using System;
using System.Collections.Generic;

namespace Entity
{
    public  class Seacher
    {
        public  string baseuri { get; set; }
        public  string name_searcher { get; set; }
        public  Dictionary<string, string> headers { get; set; }
        public  string[] uri_params { get; set; }
        public  string Build_Uri
        {
            get
            {

                string parameters = "";

                foreach (string values in uri_params)
                {
                    parameters += values;
                }

                return String.Format("{0}{1}", baseuri, parameters);
            }
        }
        public  Dictionary<string, string> nodes_result { get; set; }
    }   
}
