using EasyBotFramework;

using Telegram.Bot;

namespace Labelissimo.Bot;

public class Bot : EasyBot
{
  public Bot(string token) : base(token, 0, 0) { }
}