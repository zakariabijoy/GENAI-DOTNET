﻿using Microsoft.Extensions.AI;
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

//send prompt and get response
string prompt = "What is AI? explain max 20 words";
Console.WriteLine($"user >>>: {prompt}");

ChatResponse response = await client.GetResponseAsync(prompt);

Console.WriteLine($"assistant >>> {response}"); 
Console.WriteLine($"Tokens used: in={response.Usage?.InputTokenCount}, out={response.Usage?.OutputTokenCount}");