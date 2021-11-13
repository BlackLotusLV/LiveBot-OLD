namespace LiveBot
{
    internal static class CustomLogEvents
    {
        public static EventId LiveBot { get; } = new EventId(200, "LiveBot");
        public static EventId CommandExecuted { get; } = new EventId(201, "CMDExecuted");
        public static EventId CommandError { get; } = new EventId(202, "CMDError");
        public static EventId ClientError { get; } = new EventId(202, "ClientError");
        public static EventId SlashExecuted { get; } = new EventId(203, "SlashExecuted");
        public static EventId SlashErrored { get; } = new EventId(204, "SlashErrored");
        public static EventId ContextMenuExecuted { get; } = new EventId(205, "ContextMenuExecuted");
        public static EventId ContextMenuErrored { get; } = new EventId(206, "ContextMenuErrored");
        public static EventId POSTGRESQL { get; } = new EventId(300, "PSQL");
        public static EventId TableLoaded { get; } = new EventId(301, "TableLoaded");
        public static EventId TCHub { get; } = new EventId(400, "TCHub");
        public static EventId TCE { get; } = new EventId(401, "TCE");
        public static EventId AutoMod { get; } = new EventId(203, "AutoMod");
        public static EventId DeleteLog { get; } = new EventId(204, "DeleteLog");
        public static EventId ModMail { get; } = new EventId(205, "ModMail");
        public static EventId PhotoCleanup { get; } = new EventId(206, "PhotoCleanup");
        public static EventId LiveStream { get; } = new EventId(207, "LiveStream");
    }
}