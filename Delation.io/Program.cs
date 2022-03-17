using Azure.Identity;
using Microsoft.Graph;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder
                          .AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader();
                        });
});
// Add services to the container.
builder.Services.AddCors();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("/", (Delation message) =>
{
    var PATH = ".\\Json\\delations.json";
    Liste delations = new Liste();
    using (StreamReader reader = new StreamReader(PATH))
    {
        var myJson = reader.ReadToEnd();
        delations = JsonConvert.DeserializeObject<Liste>(myJson);
        delations.liste.Add(message);
        reader.Close();
        reader.Dispose();
    }
    System.IO.File.WriteAllText(PATH,JsonConvert.SerializeObject(delations));
    return message;
});

app.MapGet("/", () =>
{
    var PATH = ".\\Json\\delations.json";
    var r = new StreamReader(PATH);
    var myJson = r.ReadToEnd();
    Liste delations = JsonConvert.DeserializeObject<Liste>(myJson);
    r.Close();
    r.Dispose();
    return delations.liste;
});


app.MapGet("/send",async () =>
{
    string to = "t.vigneron@boostmymail.com";
    string from = "pma.bastien.aubry@gmail.com";
    MailMessage message = new MailMessage(from, to);
    message.Subject = "Using the new SMTP client.";
    message.Body = @"Using this new feature, you can send an email message from an application very easily.";
    SmtpClient client = new SmtpClient("smtp.gmail.com");
    client.EnableSsl = true;
    client.DeliveryMethod = SmtpDeliveryMethod.Network;
    client.Port = 587;
    // Credentials are necessary if the server requires the client
    // to authenticate before it will send email on the client's behalf.
    client.UseDefaultCredentials = true;
    client.Credentials = new NetworkCredential("pma.bastien.aubry@gmail.com", "syfp bssm foxn kuto");

    try
    {
        
        client.Send(message);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
            ex.ToString());
    }

    
});

app.UseCors(MyAllowSpecificOrigins);
app.Run();

internal class Liste
{
    [JsonProperty(PropertyName = "delations")]
    public List<Delation> liste { get; set; }
}
internal class Delation
{
    public string Date { get; set; }
    public string Message { get; set; }
}
