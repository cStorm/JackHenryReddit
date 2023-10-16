namespace JackHenry.Reddit.Api.Contracts;

public record Listing<T>(string after, List<T> children);
