using System;
using System.Net.Http;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;

public class WebCrawlerService
{
    private static readonly HttpClient client;

    static WebCrawlerService()
    {
        // Configure HttpClient to use TLS 1.2 or higher
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        // Optionally, you can bypass SSL certificate validation (not recommended for production)
        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        client = new HttpClient();
    }

    public async Task<string> FetchPageAsync(string url)
    {
        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }

    public void ParseHtml(string html)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        // Example: Extract all links
        var links = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
        if (links != null)
        {
            foreach (var link in links)
            {
                Console.WriteLine(link.Attributes["href"].Value);
            }
        }
    }

    public async Task CrawlAsync(string url)
    {
        try
        {
            string html = await FetchPageAsync(url);
            ParseHtml(html);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public async Task<List<DatasetInfo>> ExtractDataAsync(string url)
    {
        var htmlContent = await FetchPageAsync(url);
        var document = new HtmlDocument();
        document.LoadHtml(htmlContent);

        var datasets = new List<DatasetInfo>();
        var nodes = document.DocumentNode.SelectNodes("//div[@class='dataset-content']");

        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                var titleNode = node.SelectSingleNode(".//h3/a");
                var descriptionNode = node.SelectSingleNode(".//div[@class='notes']");

                if (titleNode != null && descriptionNode != null)
                {
                    var dataset = new DatasetInfo
                    {
                        Title = titleNode.InnerText.Trim(),
                        Description = descriptionNode.InnerText.Trim()
                    };
                    datasets.Add(dataset);
                }
            }
        }

        return datasets;
    }
}

public class DatasetInfo
{
    public string Title { get; set; }
    public string Description { get; set; }
}
