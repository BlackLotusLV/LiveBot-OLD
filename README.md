# LIve Bot
# Still Work In Progress
The bot has alot of code parts that are tied to a specific server.

## Requirements

* vs19
* .Net Core 3
* C# 8.0
* postgresql

## Database installation

In the folder DBBackup there is a file Backup.sql. If you don't use the postgres user for your database, you have to rename postgres on line 28 to your username. Then simply copy the code in console.
If you are using PG admin, connect to your database, and under tools drop down, select "Query Tool". Past the code in, and execute it.

## Config File setup

There are 4 .json.default files, copy the files and remove .default from the name. Fill out the needed fields in the .json files

* ConfigDev.json - Connection token and prefix for development bot(test bot).
* ConfigLive.json - Connection token and prefix for live build.
* DBCFG.json - Database connection parameters.
* TCECFG.json - TCE key for profile badge. Ask Célian for key.