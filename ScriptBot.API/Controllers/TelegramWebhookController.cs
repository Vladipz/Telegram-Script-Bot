using Microsoft.AspNetCore.Mvc;

using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Models.Telegram;

using Telegram.Bot.Types;

namespace ScriptBot.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class TelegramWebhookController : ControllerBase
    {
        private readonly ICommandService _commandService;

        public TelegramWebhookController(ICommandService commandService)
        {
            _commandService = commandService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            if (update?.Message?.Text == null || update.Message.Chat == null)
            {
                return Ok();
            }

            var telegramUpdate = new TelegramUpdate
            {
                ChatId = update.Message.Chat.Id,
                Username = update.Message.From?.Username,
                FirstName = update.Message.From?.FirstName,
                LastName = update.Message.From?.LastName,
                MessageText = update.Message.Text,
            };

            await _commandService.HandleCommandAsync(telegramUpdate);

            return Ok();
        }
    }
}