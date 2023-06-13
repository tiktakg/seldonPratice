using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TGBot
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Basic
    {
        public string fullName { get; set; }
        public string shortName { get; set; }
        public string inn { get; set; }
        public string ogrn { get; set; }
        public Status status { get; set; }
    }

    public class Ceo
    {
        public Person person { get; set; }
        public DateTime recordDate { get; set; }
        public string post { get; set; }
        public int massHead { get; set; }
        public int massCoowner { get; set; }
        public int disqualificationStatus { get; set; }
    }

    public class CompaniesList
    {
        public Basic basic { get; set; }
        public Registration registration { get; set; }
        public string address { get; set; }
        public string kpp { get; set; }
        public string region_code { get; set; }
        public List<Ceo> ceo { get; set; }
        public Management management { get; set; }
        public List<string> email_list { get; set; }
        public List<string> site_list { get; set; }
        public List<string> phone_list { get; set; }
        public List<PhoneFormattedList> phoneFormattedList { get; set; }
    }

    public class Management
    {
        public object companies { get; set; }
        public List<Person> persons { get; set; }
    }

    public class Person
    {
        public string inn { get; set; }
        public string fullName { get; set; }
    }

    public class Person2
    {
        public Person person { get; set; }
        public DateTime recordDate { get; set; }
        public string post { get; set; }
        public int massHead { get; set; }
        public int massCoowner { get; set; }
        public int disqualificationStatus { get; set; }
    }

    public class PhoneFormattedList
    {
        public int type { get; set; }
        public string number { get; set; }
    }

    public class Registration
    {
        public DateTime date { get; set; }
    }

    public class RegOrgan
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Root
    {
        public Status status { get; set; }
        public List<CompaniesList> companies_list { get; set; }
        public SearchSummary search_summary { get; set; }
        public string excerpt_body { get; set; }
        public int orderNum { get; set; }
    }

    public class SearchSummary
    {
        public int sample_size { get; set; }
        public bool next_page { get; set; }
        public double response_time { get; set; }
    }

    public class Status
    {
        public int itemsFound { get; set; }
        public string methodStatus { get; set; }
        public object paramsCheckError { get; set; }
        public bool paramsAreValid { get; set; }
        public RegOrgan regOrgan { get; set; }
        public DateTime date { get; set; }
        public string egrulName { get; set; }
        public int code { get; set; }
        public string name { get; set; }
    }


}
