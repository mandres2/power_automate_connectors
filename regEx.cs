public override async Task<HttpResponseMessage> ExecuteAsync()
{
    // This line of code validates if the operation ID mathces with what is specified in the OpenAPI definition of the connector
    if (this.Context.OperationId == "RegexIsMatch")
    {
        return await this.HandleRegexIsMatchOperation().ConfigureAwait(false);
    }

    // Handle the invalid operation ID:
    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
    response.Content = CreateJsonContent($"Unknown operation ID '{this.Context.OperationId}'");
}

private async Task<HttpResponseMessage> HandleRegexIsMatchOperation()
{
    HttpResponseMessage response;

    // Assume the body of the incoming request looks like this:
    // {
    //   "textToCheck": "<some text>",
    //   "regex": "<some regex pattern>"
    // }

    var contentAsString = await this.Context.Request.Content.ReadAsStringAsync().ConfigureAwait(false);

    // Parase as JSON object
    var contentAsJson = JObject.Parse(contentAsString);

    // Get the value of text to check
    var textToCheck = (string)contentAsJson["textToCheck"];

    // Create a regex based on the requested content:
    var regexInput = (string)contentAsJson["regex"];
    var rx = new Regex(regexInput);

    JObject output = new JObject
    {
        ["textToCheck"] = textToCheck,
        ["isMatch"] = rx.IsMatch(textToCheck),
    };

    response = new HttpResponseMessage(HttpStatusCode.OK);
    response.Content = CreateJsonContent(output.ToString());
    return response;
}