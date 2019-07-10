
# Live Bot
Live bot is a utility chat bot that gives moderators certain tools to make moderating easier and have a way to track warnings, kicks, bans as well as post when someone starts streaming. Also it has a honour system where you can gain experience and virtual currency that can be spent on purchasing the profile customisation for the bot.
The Bot is very specific to a certain Discord server called The Crew Community, so taking it and using for personal use as is might not be recommended, but taking any part of the code is also fine. And this is more to be open and show how the bot works.

## Requirements

* vs19
* .Net Core 3
* C# 8.0
* postgresql

## Database installation
THE DB FILE NEEDS AN UPDATE, OUTDATED IN ITS CURRENT FORM
Copy the code in console(psql), or query tool in pgadmin.
Change line 28 to "ALTER SCHEMA livebot OWNER TO [your username here];"

## Config File setup

Copy the Config.json.default file and rename it to Config.json and fill out the fields accordingly
For TCE key, ask Célian. It is the API key for [TheCrew-Exchange](https://thecrew-exchange.com/)