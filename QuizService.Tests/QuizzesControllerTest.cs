using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using QuizService.Model;
using Xunit;

namespace QuizService.Tests;

public class QuizzesControllerTest
{
    const string QuizApiEndPoint = "/api/quizzes/";

    [Fact]
    public async Task PostNewQuizAddsQuiz()
    {
        var quiz = new QuizCreateModel("Test title");
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
                content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }
    }

    [Fact]
    public async Task PostNewQuizWithTwoQuestionsAndAnswersRetursAnswers()
    {
        //Arrange
        var quiz = new QuizCreateModel("Test title2");
        var question = new QuestionCreateModel("question");
        var question2 = new QuestionCreateModel("question2");
        var answer = new AnswerCreateModel("answer");
        var answer2 = new AnswerCreateModel("answer1");
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(quiz));
            var questionContent = new StringContent(JsonConvert.SerializeObject(question));
            var question2Content = new StringContent(JsonConvert.SerializeObject(question2));
            var answerContent = new StringContent(JsonConvert.SerializeObject(answer));
            var answer2Content = new StringContent(JsonConvert.SerializeObject(answer2));

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            questionContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            question2Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            answerContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            answer2Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //Act
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}"),
                content);
            var questionResponse = await client.PostAsync(new Uri(testHost.BaseAddress, $"{response.Headers.Location}/questions"),
                questionContent);
            var question2Response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{response.Headers.Location}/questions"),
                question2Content);
            var answerResponse = await client.PostAsync(new Uri(testHost.BaseAddress, $"{questionResponse.Headers.Location}/answers"),
                                                                                     answerContent);
            var answer2Response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{question2Response.Headers.Location}/answers"),
                                                                                      answer2Content);

            var countOfAnswers = answerResponse.StatusCode == HttpStatusCode.Created && answerResponse.Headers.Location != null ? 1 : 0;
            countOfAnswers = answer2Response.StatusCode == HttpStatusCode.Created && answer2Response.Headers.Location != null ? countOfAnswers + 1 : countOfAnswers;
            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(HttpStatusCode.Created, questionResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Created, question2Response.StatusCode);
            Assert.Equal(HttpStatusCode.Created, answerResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Created, answer2Response.StatusCode);
            Assert.Equal(2, countOfAnswers);

            Assert.NotNull(response.Headers.Location);
            Assert.NotNull(questionResponse.Headers.Location);
            Assert.NotNull(question2Response.Headers.Location);
            Assert.NotNull(answerResponse.Headers.Location);
            Assert.NotNull(answer2Response.Headers.Location);
        }
    }

    [Fact]
    public async Task AQuizExistGetReturnsQuiz()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 1;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
            var quiz = JsonConvert.DeserializeObject<QuizResponseModel>(await response.Content.ReadAsStringAsync());
            Assert.Equal(quizId, quiz.Id);
            Assert.Equal("My first quiz", quiz.Title);
        }
    }

    [Fact]
    public async Task AQuizDoesNotExistGetFails()
    {
        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var response = await client.GetAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}"));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]
        
    public async Task AQuizDoesNotExists_WhenPostingAQuestion_ReturnsNotFound()
    {
        const string QuizApiEndPoint = "/api/quizzes/";
        const string QuestionsExtension = "/questions";

        using (var testHost = new TestServer(new WebHostBuilder()
                   .UseStartup<Startup>()))
        {
            var client = testHost.CreateClient();
            const long quizId = 999;
            var question = new QuestionCreateModel("The answer to everything is what?");
            var content = new StringContent(JsonConvert.SerializeObject(question));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(new Uri(testHost.BaseAddress, $"{QuizApiEndPoint}{quizId}{QuestionsExtension}"),content);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}