using System.Text.Json;

using ValidationClass;

class TheMovieApi
{
    private static string APIKey = "7885b36c69c588986eec43c930cd9b7d";
    private static bool authorizedSession = false;

    public static void Main()
    {
        var keyOption = "0";

        Console.Clear();

        Console.WriteLine("Authorizing session...");
        Console.WriteLine("========================");

        ValidatingAPIKey().GetAwaiter().GetResult();

        if (authorizedSession) {
            Console.WriteLine("AUTHORIZED!");
            Console.WriteLine("========================");
            Console.WriteLine("");

            keyOption = GetMenuOption();

            if (keyOption == "1") {
                GetGenres(1).Wait();
            } else if (keyOption == "2") {
                GetGenres(2).Wait();
            }
        }
    }

    public static async Task ValidatingAPIKey()
    {   
        var client = new HttpClient();
        var BaseUrl = "https://api.themoviedb.org/3/movie/550";
        
        try
        {
            client.BaseAddress = new Uri(BaseUrl + "?api_key=" + APIKey);
            client.Timeout = new TimeSpan(0, 0, 90);
        
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

            if (!response.IsSuccessStatusCode) {
                Console.WriteLine($"Failed Authorizing the Session. Please chek your API Key.");
                authorizedSession = false;
            }

            authorizedSession = true;

        }
        catch (System.Exception e)
        {
            Console.WriteLine("\nException Caught!");	
            Console.WriteLine("Message :{0} ",e.Message);
            authorizedSession = false;
        }
    }

    public static string GetMenuOption() {
        var keyOption = "0";
        
        Console.WriteLine("CHOOSE A NUMBER TO NAVIGATE THROUGH MENU");
        Console.WriteLine("========================");
        Console.WriteLine("");

        Console.WriteLine("1- Get MOVIE LIST Genre");
        Console.WriteLine("2- Get TV SHOWS LIST Genre");
        Console.WriteLine("========================");
        Console.WriteLine("");
        Console.Write("Option: ");
        keyOption = Console.ReadLine();
        Console.WriteLine("");
        Console.WriteLine("========================");

        while (!Validation.ValidatesMenuInputNumber(1, 2, keyOption)) {
            Console.WriteLine("Please choose a valid option");
            Console.WriteLine("");
            Console.WriteLine("Type 1 to get MOVIE LIST Genre");
            Console.WriteLine("Type 2 to get TV SHOWS LIST Genre");
            Console.WriteLine("========================");
            Console.WriteLine("");
            Console.Write("Option: ");
            keyOption = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("========================");
        }

        return keyOption;
    }

    public static async Task GetGenres(int genreKey) {
        var client = new HttpClient();
        var baseUrl = string.Empty;
        var genreLabel = string.Empty;
        var genreJSonFileName = string.Empty;

        // 1 - MOVIES
        // 2 - TV SHOWS
        if (genreKey == 1) {
            genreLabel = "MOVIES";
            genreJSonFileName = "MoviesGenreList.json";
            baseUrl = $"https://api.themoviedb.org/3/genre/movie/list?api_key={APIKey}&language=en-US";
        } else if (genreKey == 2) {
            genreLabel = "TV SHOWS";
            genreJSonFileName = "TVShowsGenreList.json";
            baseUrl = $"https://api.themoviedb.org/3/genre/tv/list?api_key={APIKey}&language=en-US";
        }
        
        try
        {
             client.BaseAddress = new Uri(baseUrl);
             client.Timeout = new TimeSpan(0, 0, 90);

             HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

            if (!response.IsSuccessStatusCode) {
                Console.WriteLine($"Something went wrong trying to get {genreLabel} genres. Try again soon!");
                return;
            }

            var responseContent = response.Content.ReadAsStringAsync().Result;
            
            var index = responseContent.IndexOf("[");
            responseContent = responseContent.Substring(index);
            char[] charsToTrim = { '\''};
            responseContent = responseContent.Trim(charsToTrim);
            responseContent = responseContent.Remove(responseContent.Length -1);

            //WRITING GENRE JSON
            File.Create(genreJSonFileName).Close();
            File.AppendAllText(genreJSonFileName, responseContent);

            if (genreKey == 1) {
                var listGenres = new List<MovieGenre>();
                var strGenresList = File.ReadAllText(genreJSonFileName);

                listGenres = JsonSerializer.Deserialize<List<MovieGenre>>(strGenresList);

                Console.WriteLine($"Official GENRE list for {genreLabel}");
                Console.WriteLine("===============================");

                foreach (var genre in listGenres)
                {
                    Console.WriteLine(genre.name);
                }
            } else if (genreKey == 2) {
                var listGenres = new List<TVShowGenre>();
                var strGenresList = File.ReadAllText(genreJSonFileName);

                listGenres = JsonSerializer.Deserialize<List<TVShowGenre>>(strGenresList);

                Console.WriteLine($"Official GENRE list for {genreLabel}");
                Console.WriteLine("===============================");

                foreach (var genre in listGenres)
                {
                    Console.WriteLine(genre.name);
                }
            }
            
        }
        catch (System.Exception e)
        {
            Console.WriteLine("\nException Caught!");	
            Console.WriteLine("Message :{0} ",e.Message);
        }
    }

    class MovieGenre {
        public int id { get; }
        public string? name { get; }
        public MovieGenre(int id, string name) {
            this.id = id;
            this.name = name;
        }
    }

    class TVShowGenre {
        public int id { get; }
        public string? name { get; }
        public TVShowGenre(int id, string name) {
            this.id = id;
            this.name = name;
        }
    }
}