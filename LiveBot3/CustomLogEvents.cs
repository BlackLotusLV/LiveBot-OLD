using Microsoft.Extensions.Logging;

namespace LiveBot
{
    internal static class CustomLogEvents
    {
        public static EventId LiveBot { get; } = new EventId(200, "LiveBot");
        public static EventId CommandExecuted { get; } = new EventId(201, "CMDExecuted");
        public static EventId CommandError { get; } = new EventId(202, "CMDError");
        public static EventId ClientError { get; } = new EventId(202, "ClientError");
        public static EventId POSTGRESQL { get; } = new EventId(300, "PSQL");
        public static EventId TableLoaded { get; } = new EventId(301, "TableLoaded");
        public static EventId TCHub { get; } = new EventId(400, "TCHub");

    }
}
