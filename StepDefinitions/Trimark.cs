using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V119.Debugger;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using System.Net;
using System.Net.Http;

//using SpecFlowProject1.Pages;
using System.Xml.Linq;
using static MongoDB.Driver.WriteConcern;


namespace SpecFlowProject1.StepDefinitions
{
    [Binding]
    public sealed class Trimark
    {
        

        HttpResponseMessage response;

        [Given(@"Execute a test API")]
        public void GivenExecuteATestAPI()
        {
           
            // Navigate to a webpage to set cookies (replace with your URL)
            

            // Get cookies from the browser
            using (IWebDriver driver = new ChromeDriver())
            {
                try
                {
                    // Navigate to the website to set the cookies
                    driver.Url = "http://10.3.4.7/";
                    driver.FindElement(By.Id("cphMain_txtUsername")).SendKeys("ajain");
                    driver.FindElement(By.Id("cphMain_txtPassword")).SendKeys("TriPass99@1");
                    driver.FindElement(By.Id("cphMain_btnLogIn")).Click();
                    Thread.Sleep(3000);
                    driver.FindElement(By.Id("MainContent_ctl01_gvCompanies_lnkSites_5")).Click();
                    Thread.Sleep(5000);
                    driver.Url = "http://10.3.4.7/admin/devices?companyId=4&siteId=1000";
                    Thread.Sleep(4000);
                    
                    // Get cookies from the browser
                    var cookies = driver.Manage().Cookies.AllCookies;


                    // updating timeoutTracker cookie
                     var cookieToUpdate = cookies.FirstOrDefault(cookie => cookie.Name == "timeoutTracker");

                        if (cookieToUpdate != null)
                        {
                            // Update the value of the cookie
                            driver.Manage().Cookies.DeleteCookieNamed("timeoutTracker");

                            // Create a new cookie with the updated value
                            var updatedCookie = new OpenQA.Selenium.Cookie("timeoutTracker", "2025-04-09T11:20:52.4198343-07:00",cookieToUpdate.Domain, cookieToUpdate.Path, cookieToUpdate.Expiry);
                    
                            // Add the updated cookie
                            driver.Manage().Cookies.AddCookie(updatedCookie);
                        }
                        cookies = driver.Manage().Cookies.AllCookies;

                    // Add cookies to HttpClient

                          HttpClientHandler handler = new HttpClientHandler();
                              handler.CookieContainer = new CookieContainer();
                              Console.WriteLine("Cookie Starting");
                             foreach (var cookie in cookies)
                             {
                                handler.CookieContainer.Add(new Uri("http://10.3.4.7/"), new System.Net.Cookie(cookie.Name, cookie.Value));
                               // handler.CookieContainer.Add(new System.Net.Cookie(cookie.Name, cookie.Value, "/", "10.3.4.7"));
                                Console.WriteLine(cookie.Name + ": " + cookie.Value);
                             }


                              Console.WriteLine("Cookie printing ends");



                            //executing api
                            HttpClient client = new HttpClient(handler);
                             // Example REST API request
                             response = client.GetAsync("http://10.3.4.7/api/company/4/site/1000/device/getalldevicesalarmdefinitions?status=All&pageNumber=1&pageSize=10").Result;




                       
                   




                    // Example: Display response content
                    

                           }
                finally
                {
                    // Close the browser
                    driver.Quit();
                } 
            } 

           // ScenarioContext.Current["Response"] = response;

           


        }


        [When(@"verify if success message is there")]
        public void WhenVerifyIfSuccessMessageIsThere()
        {
           
            if (response.IsSuccessStatusCode)
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("API Response:");
                Console.WriteLine(responseContent);
            }
            else
            {
                Console.WriteLine("API request failed. Status code: " + response.StatusCode);
            }







        }



        [Then(@"Search a node for its value")]
        public async void ThenSearchANodeForItsValue()
        {
            if (response.IsSuccessStatusCode)
            {
                // Read the response content
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                JArray jsonArray = JArray.Parse(responseBody);

                // Search for the second occurrence of a node (e.g., "userId")
                string nodeToSearch = "DeviceTypeName"; // Example node name
                string? secondOccurrence = FindSecondOccurrence(jsonArray, nodeToSearch);

                // Output the result
                if (secondOccurrence!=null)
                {
                    Console.WriteLine($"Second occurrence of '{nodeToSearch}': {secondOccurrence}");
                }
                else
                {
                    Console.WriteLine($"Node '{nodeToSearch}' not found or less than two occurrences.");
                }
            }

        }

        static string? FindSecondOccurrence(JArray jsonArray, string nodeName)
        {
            int count = 0;
            foreach (JObject jsonObject in jsonArray)
            {
                if (jsonObject.TryGetValue(nodeName, out JToken value))
                {
                    count++;
                    if (count == 2)
                    {
                        return (string)value; // Assuming the node value is an integer, adjust if necessary
                    }
                }
            }
            return null;
        }

    }
}
