using Bumba;
using Newtonsoft.Json;
using RestSharp;

class RestApi
{
    RestClient client = new RestClient("https://dog.ceo");

    public Response GetPicture(string endPoint)
    {
        var sendRequest = new RestRequest(endPoint);
        var getResponse = client.Get(sendRequest);
        Response questionApiResponse = JsonConvert.DeserializeObject<Response>(getResponse.Content);
        return questionApiResponse;
    }
}