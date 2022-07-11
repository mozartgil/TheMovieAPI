using System.Text.Json;

class TheMovieApi
{
    private static string APIKey = "7885b36c69c588986eec43c930cd9b7d";
    private static bool authorizedSession = false;

    public static void Main()
    {
        Console.Clear();

        Console.WriteLine("Authorizing session...");
        Console.WriteLine("========================");

        ValidatingAPIKey().GetAwaiter().GetResult();

        if (authorizedSession) {
            Console.WriteLine("AUTHORIZED!");
            Console.WriteLine("========================");
            Console.WriteLine("");

            GetMovieListGenre().Wait();
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

    public static async Task GetMovieListGenre() {
        var client = new HttpClient();
        string BaseUrl = $"https://api.themoviedb.org/3/genre/movie/list?api_key={APIKey}&language=en-US";

        try
        {
             client.BaseAddress = new Uri(BaseUrl);
             client.Timeout = new TimeSpan(0, 0, 90);

             HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

            if (!response.IsSuccessStatusCode) {
                Console.WriteLine($"Falha ao tentar buscar pelos generos dos filmes!");
                return;
            }

            var responseContent = response.Content.ReadAsStringAsync().Result;
            
            var index = responseContent.IndexOf("[");
            responseContent = responseContent.Substring(index);
            char[] charsToTrim = { '\''};
            responseContent = responseContent.Trim(charsToTrim);
            responseContent = responseContent.Remove(responseContent.Length -1);

            //Escrevendo o novo jogo
            var nameMovieGenreListFile = "MovieGenreList.json";
            File.Create(nameMovieGenreListFile).Close();
            File.AppendAllText(nameMovieGenreListFile, responseContent);

            var listMovieGenres = new List<MovieGenre>();
            var strMovieGenresList = File.ReadAllText(nameMovieGenreListFile);

            listMovieGenres = JsonSerializer.Deserialize<List<MovieGenre>>(strMovieGenresList);

            Console.WriteLine("Official GENRE list for MOVIES");
            Console.WriteLine("===============================");

            foreach (var genre in listMovieGenres)
            {
                Console.WriteLine(genre.name);
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
}