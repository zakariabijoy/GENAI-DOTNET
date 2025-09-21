using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

// get credentials from user secrets

IConfigurationRoot config =  new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string githubToken = config["GitHubModels:Token"] ?? throw new InvalidProgramException();

var credentials = new ApiKeyCredential(githubToken);
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference"),
};

//create a chat client
var client = new OpenAIClient(credentials, options).GetChatClient("openai/gpt-5-mini").AsIChatClient();

#region Basic Completion

//send prompt and get response
//string prompt = "What is AI? explain max 20 words";
//Console.WriteLine($"user >>>: {prompt}");

//ChatResponse response = await client.GetResponseAsync(prompt);

//Console.WriteLine($"assistant >>> {response}");
//Console.WriteLine($"Tokens used: in={response.Usage?.InputTokenCount}, out={response.Usage?.OutputTokenCount}");

#endregion

#region Streaming

//string prompt = "What is AI? explain max 200 words";
//Console.WriteLine($"user >>>: {prompt}");

//var responseStream =  client.GetStreamingResponseAsync(prompt);

//await foreach (var message in responseStream)
//{
//    Console.WriteLine(message.Text);
//}

#endregion

#region Classification

//var classificationPrompt = """
//Please classify the following sentences into categories: 
//- 'complaint' 
//- 'suggestion' 
//- 'praise' 
//- 'other'.

//1) "I love the new layout!"
//2) "You should add a night mode."
//3) "When I try to log in, it keeps failing."
//4) "This app is decent."
//""";

//Console.WriteLine($"user >>> {classificationPrompt}");

//ChatResponse classificationResponse = await client.GetResponseAsync(classificationPrompt);

//Console.WriteLine($"assistant >>>\n{classificationResponse}");

#endregion

#region Summarization

//var summaryPrompt = """
//Summarize the following blog in 1 concise sentences:

//"Microservices architecture is increasingly popular for building complex applications, but it comes with additional overhead. It's crucial to ensure each service is as small and focused as possible, and that the team invests in robust CI/CD pipelines to manage deployments and updates. Proper monitoring is also essential to maintain reliability as the system grows."
//""";

//Console.WriteLine($"user >>> {summaryPrompt}");

//ChatResponse summaryResponse = await client.GetResponseAsync(summaryPrompt);

//Console.WriteLine($"assistant >>> \n{summaryResponse}");

#endregion

#region Sentiment Analysis

//var analysisPrompt = """
//        You will analyze the sentiment of the following product reviews. 
//        Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a generate sentiment of all reviews.

//        I bought this product and it's amazing. I love it!
//        This product is terrible. I hate it.
//        I'm not sure about this product. It's okay.
//        I found this product based on the other reviews. It worked for a bit, and then it didn't.
//        """;

//Console.WriteLine($"user >>> {analysisPrompt}\n");

//ChatResponse responseAnalysis = await client.GetResponseAsync(analysisPrompt);

//Console.WriteLine($"assistant >>> \n{responseAnalysis}");

#endregion

#region ChatApp

// Start the conversation with context for the AI model
List<ChatMessage> chatHistory = new()
    {
        new ChatMessage(ChatRole.System, """
            You are a friendly hiking enthusiast who helps people discover fun hikes in their area.
            You introduce yourself when first saying hello.
            When helping people out, you always ask them for this information
            to inform the hiking recommendation you provide:

            1. The location where they would like to hike
            2. What hiking intensity they are looking for

            You will then provide three suggestions for nearby hikes that vary in length
            after you get that information. You will also share an interesting fact about
            the local nature on the hikes when making a recommendation. At the end of your
            response, ask if there is anything else you can help with.
        """)
    };

while (true)
{
    // Get user prompt and add to chat history
    Console.WriteLine("Your prompt:");
    var userPrompt = Console.ReadLine();
    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    // Stream the AI response and add to chat history
    Console.WriteLine("AI Response:");
    var response = "";
    await foreach (var item in
        client.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(item.Text);
        response += item.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
    Console.WriteLine();
}

#endregion