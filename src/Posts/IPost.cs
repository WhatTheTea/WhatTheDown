namespace WhatTheDown.Posts;

public interface IPost
{
    string PostUrl { get; }

    Task<string> GetCaption();
    Task<string> GetContentUrlAsync();
    Task<PostType> GetPostTypeAsync(string? downloadUrl = null);
}