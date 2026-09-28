namespace PageTransitions;

public sealed class Traveler
{
    public required string Name { get; init; }
    public required string Image { get; init; }
    public required string Bio { get; init; }
    public int Following { get; set; }
    public int Followers { get; set; }
    public required int Days { get; init; }
    public required string[] Trips { get; init; }
    public string FirstName => Name[..Name.LastIndexOf(' ')];
}

public sealed class Place
{
    public required string Name { get; init; }
    public required double Rating { get; init; }
    public required int Stars { get; init; }
    public required string Image { get; init; }
    public required string Description { get; init; }
}

public sealed class TravelState
{
    public string Page { get; private set; } = "index";
    public int IndexedUser { get; private set; }
    public bool Following { get; private set; }
    public bool Saved { get; private set; }
    public bool MenuOpen { get; private set; }

    public IReadOnlyList<Traveler> Users { get; } =
    [
        new()
        {
            Name = "Sophia Gonzalez",
            Image = "images/profile2.jpg",
            Bio = "Had a brief career with jack-in-the-boxes in Phoenix, AZ. Spent several months managing squirt guns and implementing toy elephants.",
            Following = 789,
            Followers = 2748,
            Days = 32,
            Trips = ["Honolulu", "Burmuda", "Los Cabos", "San Antonio"]
        },
        new()
        {
            Name = "Ben Allen",
            Image = "images/profile3.jpg",
            Bio = "Bacon nerd. Freelance twitter practitioner. Social media nerd. Pop culture junkie. Proud alcohol advocate. Food geek.",
            Following = 140,
            Followers = 789,
            Days = 5,
            Trips = ["Honolulu", "Peru", "San Francisco"]
        },
        new()
        {
            Name = "Jill Fernandez",
            Image = "images/profile4.jpg",
            Bio = "Prone to fits of apathy. Writer. Devoted gamer. Web scholar. Hipster-friendly music advocate. Problem solver. Student. Twitter fanatic.",
            Following = 590,
            Followers = 1705,
            Days = 12,
            Trips = ["Honolulu", "Tokyo", "Osaka"]
        },
        new()
        {
            Name = "Cynthia Obel",
            Image = "images/profile5.jpg",
            Bio = "Producing at the fulcrum of modernism and purpose to craft a compelling and authentic narrative. My opinions belong to myself.",
            Following = 590,
            Followers = 1705,
            Days = 12,
            Trips = ["Honolulu", "Tokyo", "Osaka"]
        }
    ];

    public IReadOnlyList<Place> Places { get; } =
    [
        new()
        {
            Name = "Honolulu",
            Stars = 4,
            Rating = 8.9,
            Image = "images/honolulu.jpg",
            Description = "Ocean breezes rustle palm trees along the harborfront, while in the cool, mist-shrouded Koʻolau Range, forested hiking trails offer postcard city views. At sunset, cool off with an amble around Magic Island or splash in the ocean at Ala Moana Beach."
        },
        new()
        {
            Name = "Santorini",
            Stars = 4,
            Rating = 7.8,
            Image = "images/santorini.jpg",
            Description = "With multicoloured cliffs soaring above a sea-drowned caldera, Santorini looks like a giant slab of layered cake. The main island of Thira will take your breath away with its snow-drift of white Cycladic houses lining the cliff tops."
        },
        new()
        {
            Name = "Cusco",
            Stars = 3,
            Rating = 7.4,
            Image = "images/peru.jpg",
            Description = "Wandered the cobblestone streets and quaint lanes of the town, which has been designated a UNESCO World Heritage site. A walking tour revealed historic architecture, colonial landmarks and alluring shops and restaurants."
        }
    ];

    public Traveler Selected => Users[IndexedUser];

    public void SetPage(string page) => Page = page;

    public void SelectUser(int index)
    {
        IndexedUser = index;
        Following = false;
    }

    public void ToggleFollow()
    {
        if (Following)
            Selected.Followers--;
        else
            Selected.Followers++;

        Following = !Following;
    }

    public void ToggleSaved() => Saved = !Saved;

    public void ToggleMenu() => MenuOpen = !MenuOpen;

    public static string PageFromUri(string location)
    {
        var path = Uri.TryCreate(location, UriKind.Absolute, out var absolute)
            ? absolute.AbsolutePath
            : location;
        var cut = path.IndexOfAny(['?', '#']);
        if (cut >= 0)
            path = path[..cut];

        // The route is the last segment, so a GitHub Pages prefix such as
        // /blazor-anime/travel/place still resolves to "place".
        var segment = "";
        foreach (var part in path.Split('/', StringSplitOptions.RemoveEmptyEntries))
            segment = part;

        return segment switch
        {
            "place" => "place",
            "group" => "group",
            _ => "index"
        };
    }
}
