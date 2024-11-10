using Microsoft.AspNetCore.Mvc;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Timer = System.Timers.Timer;

namespace KonkurLeft.Controllers;

[Route("api/messagehandle")]
[ApiController]
public class UpdateController : ControllerBase
{
    IWebHostEnvironment _environment;
    IConfiguration _configuration;

    public UpdateController(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    [HttpGet("setWebhook")]
    public async Task<IActionResult> SetWebhook()
    {
        using var httpClient = new HttpClient();
        var _client = new TelegramBotClient(_configuration.GetSection("botToken").Get<string>(), httpClient);
        await _client.SetWebhookAsync(_configuration.GetSection("webhookUrl").Get<string>());
        return Ok("Webhook seted");
    }

    [HttpGet("startReminder")]
    public IActionResult StartReminder()
    {
        var timer = new Timer(1000);
        timer.Elapsed += Timer_Elapsed;
        timer.Start();
        return Ok("Reminder started");
    }

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        if (DateTime.UtcNow.Hour == 4 && DateTime.UtcNow.Minute == 30 && DateTime.UtcNow.Second == 1 && DateTime.UtcNow.Millisecond < 999)
        //if (DateTime.UtcNow.Hour == 19 && DateTime.UtcNow.Minute == 24 && DateTime.UtcNow.Second == 1&& DateTime.UtcNow.Millisecond<999)
        {
            foreach (var group in _configuration.GetSection("groups").Get<List<Groups>>())
            {
                var _client = new TelegramBotClient(_configuration.GetSection("botToken").Get<string>());

                var date = new DateTime(2024, 4, 25, 0, 0, 0, 0, DateTimeKind.Utc);

                var left = date - DateTime.UtcNow;

                _client.SendTextMessageAsync(new ChatId(group.GroupId), "به کنکور " + left.Days + "روز مانده");
            }
        }
    }

    [HttpPost]
    public IActionResult Post([FromBody] Update update)
    {
        var _client = new TelegramBotClient(_configuration.GetSection("botToken").Get<string>());

        if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text && (update.Message.Chat!.Type == ChatType.Supergroup || update.Message.Chat!.Type == ChatType.Group))
        {
            if (update.Message.Text == "#konkur")
            {
                var date = new DateTime(2024, 4, 25, 0, 0, 0, 0, DateTimeKind.Utc);

                var left = date - DateTime.UtcNow;

                _client.SendTextMessageAsync(new ChatId(update.Message.Chat.Id), "به کنکور " + left.Days + "روز مانده", replyToMessageId: update.Message.MessageId);
            }
        }

        return Ok();
    }
}

enum KType
{
    Riyazi, Tacrobi, Insani, Zaban, Honar
}

class Groups
{
    public string GroupName { get; set; } = null!;
    public string GroupId { get; set; } = null!;
    public int Type { get; set; }
}
