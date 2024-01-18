using System.Net.Mime;
using System.Text;

namespace Photon.HomeLoad;

class HttpRequests : IDisposable
{
    const string Domain = "samanapi.mrud.ir";
    const string BaseUrl = $"https://{Domain}/";
    const string BasePath = BaseUrl + "api/loan-applicant/allowed/";
    const string Authorization = "Bearer eyJhbGciOiJBMTI4S1ciLCJlbmMiOiJBMTI4Q0JDLUhTMjU2IiwidHlwIjoiSldUIiwiY3R5IjoiSldUIn0.JURfrg6cQXT_zeE6G-Oc9sbU7H-g82vMQLklsYL0bp3kSV930G492g.jFHsbIggJwNWBFO7fOZkXA.6GDVqTSLsAQpu18VYE0JMXn7mN3Yi0ycuRQZeW12gjw_w7gsl2jcXxVtIABKGrFE222oEbZigUJFHZ7i1uJcR1Itgm9I94g2tYnoLhYkKDVQveU_bgV3NTFrN0kh3J00VwiQZXmYDOfhtbRd1aKV5Y6-RqFR6J0EABrWYa2YMIk0he61O5rGgP2zm9_RCeGXIBe6STT61NxHAlo9FJgfXqOmNEyXf8p5KBKn6JjG-D2__dGuu6MX10G-wnboTNI7iS06QGkyyx4By_JTqJkenCyTsa4AHge4zIZehd9Ktvaux2yFgqjynco0HGZZxPp_uawzkb8WB-GN7N_AABnMTEevHNdVLIRHiwNG_F7ypiSl042eW-vzDpf-voqiH5lK5AqK54EkoYmqbuBxVpLftJaKMh0fkLiIGknzHoNU6PEHe7ryCLUjI_E_Po4tqSNsAWiYw0LlnG-1K5dPTAgeb-vGz09m9Vc82WSgYNuTHBtmZqjRXz4lsf18HsTvxDwaS26S9no-Br_aiwwtgHvx5rn_yI6hmb9L-jr1rwAiGpdXLqUL2Z5W6qoqDNOS_2nh88ezaWA2TJO8Lf4_dummbT-7qusQBER4iDvrAgQjwpjdjU1MMShUBX8u7M22ufRXq5HMeHWQkkwrVXuJtVOWZg.afqJ40d8efh5siqBcc0UKw";
    readonly HttpClient service = new();

    public HttpRequests()
    {
        service.DefaultRequestHeaders.Add("authority", Domain);
        service.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
        service.DefaultRequestHeaders.Add("authorization", Authorization);
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