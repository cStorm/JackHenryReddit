namespace JackHenry.Reddit;

public record PostSummary(string Username, string Title, string Id = "", int UpvoteCount = 0);
