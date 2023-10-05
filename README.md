# JackHenryReddit

Create an OAuth app here: https://www.reddit.com/prefs/apps

## JackHenry.Reddit.ConsoleApp

Get API refresh_token:
```shell
> dotnet run  auth <api-app-id> --port=8080
```

Monitor specific subreddit:
```shell
> dotnet run  watch AskReddit <api-app-id> --refresh <api-refresh-token>
```

## Programming Assignment (Requirements)

Your app should consume the posts from your chosen subreddit in near real time and keep track of the following statistics between the time your application starts until it ends:

-   Posts with most up votes
-   Users with most posts

Your app should also provide some way to report these values to a user (periodically log to terminal, return from RESTful web service, etc.). If there are other interesting statistics youÅfd like to collect, that would be great. There is no need to store this data in a database; keeping everything in-memory is fine. That said, you should think about how you would persist data if that was a requirement.

To acquire near real time statistics from Reddit, you will need to continuously request data from Reddit's rest APIs. Reddit implements rate limiting and provides details regarding rate limit used, rate limit remaining, and rate limit reset period via response headers. Your application should use these values to control throughput in an even and consistent manner while utilizing a high percentage of the available request rate.

ItÅfs very important that the various application processes do not block each other as Reddit can have a high volume on many of their subreddits. The app should process posts as concurrently as possible to take advantage of available computing resources. While we are only asking to track a single subreddit, you should be thinking about his you could scale up your app to handle multiple subreddits.

While designing and developing this application, you should keep SOLID principles in mind. Although this is a code challenge, we are looking for patterns that could scale and are loosely coupled to external systems / dependencies. In that same theme, there should be some level of error handling and unit testing. The submission should contain code that you would consider production ready.
