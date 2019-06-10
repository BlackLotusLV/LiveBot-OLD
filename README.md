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

There are 4 .json.default files, copy the files and remove .default from the name. Fill out the needed fields in the .json files

* ConfigDev.json - Connection token and prefix for development bot(test bot).
* ConfigLive.json - Connection token and prefix for live build.
* DBCFG.json - Database connection parameters.
* TCECFG.json - TCE key for profile badge. Ask Célian for key.