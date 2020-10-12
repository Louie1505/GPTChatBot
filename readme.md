Discord bot for generating responses from a finetuned GPT-2 model and occasionally chiming in because I think that's funny.

------------------------------------------------------------------------

Requires a finetuned GPT-2 model ran against discord logs. Below is the process I used, no idea if it's the best:

• Rip discord chat. I used https://github.com/Tyrrrz/DiscordChatExporter

• It's a lot easier to sanatise data if you can query it, I stuck it in an SQL db

• Query data to get it more sanitary, might want to remove thing like bot commands, URLs, 'x pinned a message/joined', author is a bot, content like ':%:', etc

• Format data to optimise for training. I found formatting messages like a script worked fairly well (ie "[Author]: [Content]") 

• Finetune a model (I used the 355M) on your dataset, either locally or using this colab - https://colab.research.google.com/drive/1VLG8e7YSEwypxU-noRNhsv5dW4NfTGce

------------------------------------------------------------------------

Setup:

Program requires appsettings.json creating in root with the folliwing keys/structure:

{

  "Token": Required, Your bot token as a string

  "Prefix": Required, Whichever symbol you want to use as a command prefix, e.g "+"

  "numMessages": Required, The number of messages to get from the conversation as input for the model eg 10

  "respChance": Required, 1 in x chance to respond eg 25

  "weightFactor": Optional, 1 in x-y chance to respond if the previous messages appear to be a conversation with the bot eg 10

  "adminUsers": Optional, comma seperated list of users who can toggle debug eg "username,username"

  "messageFormat": Optional, does a replace on [author] and [content] if populated to match how the training data was formatted. Defaults to "[Author]: [Content]"

}

Remember to set to copy to output directory before building!

Build, copy contents of ~\bin\Debug\netcoreapp3.1 to install location

cmd as admin, run:
sc.exe create [servicename] binpath= "[install location]"

------------------------------------------------------------------------

Debug commands:

+debug

Enables all other commands, can only be run by users in adminUsers if key is populated

+input

Print out the messages formatted in the way they will send as input to the model

+config

Safe config debug, returns keys you would want to chance on the fly in a formatted manor

+setconfig [key] [value]

Set given key to given value

+configjson

Prints out appsettings.json, including any keys/tokens. Probably don't use in public server 😊

+respond

Toggle always respond mode, which stops running 1/x chance of responding and replies to every message
