using Newtonsoft.Json;
using Questao2;

public class Program
{
    public static async Task Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = await getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = await getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static async Task<int> getTotalScoredGoals(string team, int year)
    {
        var matches = await GetMatches(team, year);

        var goals = matches.Sum(m => int.Parse(m.Team1Goals));

        return goals;
    }

    public static async Task<List<MatchResponse>> GetMatches(string team, int year)
    {
        string URL_API = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={team}";

        List<MatchResponse> matches = new();

        var matchesResponse = await GetMatch(URL_API);

        if (matchesResponse.Data.Any()) matches.AddRange(matchesResponse.Data);

        if (matchesResponse.TotalPages > 1)
        {
            for (int page = 2; page < matchesResponse.TotalPages + 1; page++)
            {
                var novaUrl = URL_API + $"&page={page}";

                var novoResponse = await GetMatch(novaUrl);

                if (matchesResponse.Data.Any()) matches.AddRange(novoResponse.Data);
            }
        }

        return matches;
    }

    public static async Task<MatchesResponse> GetMatch(string urlApi)
    {
        using HttpClient client = new();

        HttpResponseMessage response = await client.GetAsync(urlApi);

        string responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode.Equals(false))
        {
            throw new Exception("Error searching for match");
        }

        var matchesResponse = JsonConvert.DeserializeObject<MatchesResponse>(responseBody);

        return matchesResponse;
    }

}