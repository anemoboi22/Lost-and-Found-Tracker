using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using Test.Models;

public class Post
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Content { get; set; }
    public string ImagePathsJson { get; set; }
    public string Status { get; set; }
    public string CreationDate { get; set; }
    [Ignore]
    public List<string> ImagePaths
    {
        get => JsonConvert.DeserializeObject<List<string>>(ImagePathsJson);
        set => ImagePathsJson = JsonConvert.SerializeObject(value);
    }
    public int UserId { get; set; }

    [Ignore]
    public string UserName { get; set; }

    [Ignore]
    public User Owner { get; set; }
}

