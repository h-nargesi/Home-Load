using System.Net.Mime;
using System.Text;

namespace Photon.HomeLoad;

class HttpRequests : IDisposable
{
    const string Domain = "samanapi.mrud.ir";
    const string BaseUrl = $"https://{Domain}/";
    const string BasePath = BaseUrl + "api/loan-applicant/allowed/";
    readonly HttpClient service = new();

    public HttpRequests(Config config)
    {
        service.DefaultRequestHeaders.Add("authority", Domain);
        service.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
        service.DefaultRequestHeaders.Add("authorization", config.Authorization);
        service.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.8");
        service.DefaultRequestHeaders.Add("origin", BaseUrl);
        service.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"116\", \"Not)A;Brand\";v=\"24\", \"Brave\";v=\"116\"");
        service.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
        service.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
        service.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
        service.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
        service.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
        service.DefaultRequestHeaders.Add("sec-gpc", "1");
        service.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");

        service.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<Bank[]> Banks()
    {
        using var response = await service.GetAsync(BasePath + "bank");
        var text = await response.Content.ReadAsStringAsync();

        try
        {
            var result = text.DeserializeJson<Bank.Result>();
            if (result.Message != null) throw new Exception(result.Message);

            return result.Data.BankData;
        }
        catch (Exception ex)
        {
            throw new Exception(text, ex);
        }
    }

    public async Task<Branch[]> Branches(int bankOrganizationUnitId)
    {
        var data = new
        {
            BankOrganizationUnitId = bankOrganizationUnitId,
            RentalPlaceCityId = 96
        };

        var content = new StringContent(
            data.SerializeJson(),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        using var response = await service.PostAsync(BasePath + "bank/branch", content);
        var text = await response.Content.ReadAsStringAsync();

        try
        {
            var result = text.DeserializeJson<Branch.Result>();
            if (result.Message != null) throw new Exception(result.Message);

            return result.Data;
        }
        catch (Exception ex)
        {
            throw new Exception(text, ex);
        }
    }

    public void Dispose()
    {
        service?.Dispose();
    }
}