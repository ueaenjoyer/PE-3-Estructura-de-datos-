using System;
using System.Collections.Generic;
using System.Linq;

class Player
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Skill { get; set; } // 1-100
    public int TeamId { get; set; }

    public Player(int id, string name, int skill)
    {
        Id = id;
        Name = name;
        Skill = skill;
    }
}

class Team
{
    public int Id { get; set; }
    public string Name { get; set; }
    public HashSet<Player> Players { get; set; }
    public int Points { get; set; }
    public int GoalsScored { get; set; }
    public int GoalsConceded { get; set; }

    public Team(int id, string name)
    {
        Id = id;
        Name = name;
        Players = new HashSet<Player>();
        Points = 0;
        GoalsScored = 0;
        GoalsConceded = 0;
    }

    public void AddPlayer(Player player)
    {
        if (Players.Count >= 11)
        {
            throw new InvalidOperationException("El equipo ya tiene 11 jugadores");
        }
        player.TeamId = Id;
        Players.Add(player);
    }

    public double GetTeamSkill()
    {
        return Players.Average(p => p.Skill);
    }
}

class Tournament
{
    private List<Team> teams;
    private List<Player> allPlayers;
    private Random random;
    private const int TOTAL_PLAYERS = 100;
    private const int PLAYERS_PER_TEAM = 11;

    public Tournament()
    {
        teams = new List<Team>();
        allPlayers = new List<Player>();
        random = new Random();
        GeneratePlayers();
    }

    private void GeneratePlayers()
    {
        for (int i = 1; i <= TOTAL_PLAYERS; i++)
        {
            int skill = random.Next(50, 101); // Skill between 50-100
            allPlayers.Add(new Player(i, $"Jugador {i}", skill));
        }
    }

    public void CreateTeams(int numberOfTeams)
    {
        if (numberOfTeams < 2 || numberOfTeams > 8)
        {
            throw new ArgumentException("El torneo debe tener entre 2 y 8 equipos");
        }

        // Clear existing teams
        teams.Clear();
        
        // Create teams
        for (int i = 0; i < numberOfTeams; i++)
        {
            teams.Add(new Team(i + 1, $"Equipo {i + 1}"));
        }

        // Shuffle players and assign to teams
        var shuffledPlayers = allPlayers.OrderBy(x => random.Next()).ToList();
        
        for (int i = 0; i < numberOfTeams * PLAYERS_PER_TEAM; i++)
        {
            int teamIndex = i % numberOfTeams;
            teams[teamIndex].AddPlayer(shuffledPlayers[i]);
        }
    }

    public void SimulateMatches()
    {
        for (int i = 0; i < teams.Count; i++)
        {
            for (int j = i + 1; j < teams.Count; j++)
            {
                SimulateMatch(teams[i], teams[j]);
            }
        }
    }

    private void SimulateMatch(Team team1, Team team2)
    {
        // Base goals based on team skill (0-3 goals)
        int goals1 = (int)(team1.GetTeamSkill() / 33);
        int goals2 = (int)(team2.GetTeamSkill() / 33);

        // Add some randomness
        goals1 += random.Next(0, 3);
        goals2 += random.Next(0, 3);

        // Update team stats
        team1.GoalsScored += goals1;
        team1.GoalsConceded += goals2;
        team2.GoalsScored += goals2;
        team2.GoalsConceded += goals1;

        // Update points
        if (goals1 > goals2)
        {
            team1.Points += 3; // Win
        }
        else if (goals2 > goals1)
        {
            team2.Points += 3; // Win
        }
        else
        {
            team1.Points += 1; // Draw
            team2.Points += 1; // Draw
        }
    }

    public void PrintStandings()
    {
        Console.WriteLine("\n=== TABLA DE POSICIONES ===");
        Console.WriteLine("Equipo\tPJ\tPG\tPE\tPP\tGF\tGC\tPTS");
        
        foreach (var team in teams.OrderByDescending(t => t.Points)
                                 .ThenByDescending(t => t.GoalsScored - t.GoalsConceded)
                                 .ThenByDescending(t => t.GoalsScored))
        {
            int wins = team.Points / 3;
            int draws = team.Points % 3;
            int losses = (teams.Count - 1) - wins - draws;
            
            Console.WriteLine($"{team.Name}\t{teams.Count - 1}\t{wins}\t{draws}\t{losses}\t{team.GoalsScored}\t{team.GoalsConceded}\t{team.Points}");
        }
    }

    public void PrintTeamDetails(int teamId)
    {
        var team = teams.FirstOrDefault(t => t.Id == teamId);
        if (team == null)
        {
            Console.WriteLine("Equipo no encontrado.");
            return;
        }

        Console.WriteLine($"\n=== {team.Name} ===");
        Console.WriteLine("Jugadores (Habilidad):");
        foreach (var player in team.Players.OrderByDescending(p => p.Skill))
        {
            Console.WriteLine($"{player.Name}: {player.Skill}");
        }
        Console.WriteLine($"Habilidad promedio: {team.GetTeamSkill():F1}");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== SISTEMA DE TORNEO DE FÚTBOL ===\n");
        
        Tournament tournament = new Tournament();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\n=== MENÚ PRINCIPAL ===");
            Console.WriteLine("1. Configurar torneo");
            Console.WriteLine("2. Simular partidos");
            Console.WriteLine("3. Ver tabla de posiciones");
            Console.WriteLine("4. Ver detalles de equipo");
            Console.WriteLine("5. Salir");
            Console.Write("\nSeleccione una opción: ");

            if (!int.TryParse(Console.ReadLine(), out int option))
            {
                Console.WriteLine("Opción no válida. Intente de nuevo.");
                continue;
            }

            try
            {
                switch (option)
                {
                    case 1:
                        Console.Write("\nIngrese el número de equipos (2-8): ");
                        if (int.TryParse(Console.ReadLine(), out int teamCount))
                        {
                            tournament.CreateTeams(teamCount);
                            Console.WriteLine($"\n¡Torneo configurado con {teamCount} equipos!");
                        }
                        else
                        {
                            Console.WriteLine("Número de equipos no válido.");
                        }
                        break;

                    case 2:
                        tournament.SimulateMatches();
                        Console.WriteLine("\n¡Partidos simulados exitosamente!");
                        break;

                    case 3:
                        tournament.PrintStandings();
                        break;

                    case 4:
                        Console.Write("\nIngrese el ID del equipo: ");
                        if (int.TryParse(Console.ReadLine(), out int teamId))
                        {
                            tournament.PrintTeamDetails(teamId);
                        }
                        else
                        {
                            Console.WriteLine("ID de equipo no válido.");
                        }
                        break;

                    case 5:
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Opción no válida. Intente de nuevo.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
