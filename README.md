
# Live Bot
# Still Work In Progress
The bot has alot of code parts that are tied to a specific server.

## Requirements

* vs19
* .Net Core 3
* C# 8.0
* postgresql

## Database installation

Copy the code in console(psql), or query tool in pgadmin.
Change line 28 to "ALTER SCHEMA livebot OWNER TO [your username here];"

## Config File setup

Copy the Config.json.default file and rename it to Config.json and fill out the fields accordingly
For TCE key, ask Célian. It is the API key for [TheCrew-Exchange](https://thecrew-exchange.com/)