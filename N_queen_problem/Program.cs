
//czas wykonania, stan open i closed(ile tam elementów sie znajduje). Jeżeli ze wskaźnikami to podać w jakiej kolejności wstawiani byli hetmani
//mogą być wyniki pośrednie wypisane. moze sie to znalezc w karcie samooceny
//minimum to 4, max może być 4 i 5 ale moze tez byc duzo wieksze
using Microsoft.FSharp.Core;
using Plotly.NET;
using Plotly.NET.CSharp;
using Plotly.NET.ImageExport;
using Plotly.NET.LayoutObjects;
using System.Diagnostics;
using System.Text;
using static Plotly.NET.StyleParam;

enum Search
{
    BFS,
    DFS,
    BFSMod,
    BFSMod2,
    DFSMod,
    DFSMod2
}

class Program //list od krotki to plansza
{
    #region Modified
    static string GetString(List<(int, int)> s) => string.Join(",", s.Select(p => $"{p.Item1}-{p.Item2}")); //reprezentacja słowna planszy
    static bool HasColisionsMod(List<(int, int)> s)
    {
        for (int i = 0; i < s.Count - 1; i++)
        {
            //wiersz lub kolumna
            if (s[i].Item1 == s[^1].Item1 || s[i].Item2 == s[^1].Item2)
                return true;

            //ta sama przekatna
            if (Math.Abs(s[i].Item1 - s[^1].Item1) == Math.Abs(s[i].Item2 - s[^1].Item2))
                return true;
        }
        return false;
    }
    static List<List<(int, int)>> GenChildrenMod(List<(int, int)> s, int n)
    {
        List<List<(int, int)>> children = new List<List<(int, int)>>();
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                if (!s.Contains((i, j)))
                {
                    var newChild = new List<(int, int)>(s)
                    {
                        (i, j)
                    };
                    if (HasColisionsMod(newChild)) continue;
                    children.Add(newChild);
                }
            }
        }
        return children;
    }
    /// <summary>
    /// closed -> HashSet i porównywanie stringów zamiast listy
    /// brak generowania dzieci dla stanu, który ma kolizje
    /// </summary>
    /// <param name="s0"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    static (List<(int, int)>, int, int) BFSMod(List<(int, int)> s0, int n)
    {
        int closedCnt = 0;
        var closed = new HashSet<string>();
        var open = new Queue<List<(int, int)>>(); // LIFO
        open.Enqueue(s0);
        while (open.Count > 0)
        {
            var s = open.Dequeue();
            if (IsLastState(s, n)) return (s, open.Count, closedCnt);
            if (HasColisions(s))
            {
                closed.Add(GetString(s));
                closedCnt++;
                continue;
            }

            var children = GenChildren(s, n);
            foreach (var child in children)
            {
                if (!closed.Contains(GetString(child)) && !IsInList(open, child)) open.Enqueue(child);
            }
            closed.Add(GetString(s));
            closedCnt++;
        }
        return ([], 0, 0);
    }
    /// <summary>
    /// to, co BFSMod
    /// generowanie tylko dzieci, które nie mają kolizji
    /// sprawdzanie kolizji tylko dla najnowszego hetmana (poprzednie sprawdzone w swoich iteracjach)
    /// </summary>
    /// <param name="s0"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    static (List<(int, int)>, int, int) BFSMod2(List<(int, int)> s0, int n)
    {
        int closedCnt = 0;
        var closed = new HashSet<string>();
        var open = new Queue<List<(int, int)>>(); // LIFO
        open.Enqueue(s0);
        while (open.Count > 0)
        {
            var s = open.Dequeue();
            if (IsLastState(s, n)) return (s, open.Count, closedCnt);
            if (HasColisionsMod(s))
            {
                closed.Add(GetString(s));
                closedCnt++;
                continue;
            }

            var children = GenChildrenMod(s, n);
            foreach (var child in children)
            {
                if (!closed.Contains(GetString(child)) && !IsInList(open, child)) open.Enqueue(child);
            }
            closed.Add(GetString(s));
            closedCnt++;
        }
        return ([], 0, 0);
    }
    /// <summary>
    /// closed -> HashSet i porównywanie stringów zamiast listy
    /// brak generowania dzieci dla stanu, który ma kolizje
    /// </summary>
    /// <param name="s0"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    static (List<(int, int)>, int, int) DFSMod(List<(int, int)> s0, int n)
    {
        int closedCnt = 0;
        var closed = new HashSet<string>();
        var open = new Stack<List<(int, int)>>(); // LIFO
        open.Push(s0);
        while (open.Count > 0)
        {
            var s = open.Pop();
            if (IsLastState(s, n)) return (s, open.Count, closedCnt);
            if (s.Count >= n || HasColisions(s))
            {
                closed.Add(GetString(s));
                closedCnt++;
                continue;
            }

            var children = GenChildren(s, n);
            foreach (var child in children)
            {
                if (!closed.Contains(GetString(child)) && !IsInList(open, child)) open.Push(child);
            }
            closed.Add(GetString(s));
            closedCnt++;
        }
        return ([], 0, 0);
    }
    /// <summary>
    /// to, co DFSMod
    /// generowanie tylko dzieci, które nie mają kolizji
    /// sprawdzanie kolizji tylko dla najnowszego hetmana (poprzednie sprawdzone w swoich iteracjach)
    /// </summary>
    /// <param name="s0"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    static (List<(int, int)>, int, int) DFSMod2(List<(int, int)> s0, int n)
    {
        int closedCnt = 0;
        var closed = new HashSet<string>();
        var open = new Stack<List<(int, int)>>(); // LIFO
        open.Push(s0);
        while (open.Count > 0)
        {
            var s = open.Pop();
            if (IsLastState(s, n)) return (s, open.Count, closedCnt);
            if (s.Count >= n || HasColisionsMod(s))
            {
                closed.Add(GetString(s));
                closedCnt++;
                continue;
            }

            var children = GenChildrenMod(s, n);
            foreach (var child in children)
            {
                if (!closed.Contains(GetString(child)) && !IsInList(open, child)) open.Push(child);
            }
            closed.Add(GetString(s));
            closedCnt++;
        }
        return ([], 0, 0);
    }
    #endregion

    static List<List<(int, int)>> GenChildren(List<(int, int)> s, int n) //metoda generujaca potomkow przyjmuje stan(plansze) dla ktorej ma wygenerowac kolejne plansze
    { //ta metoda zwraca liste list(krotek)
        List<List<(int, int)>> children = new List<List<(int, int)>>();
        for (int i = 0; i < n; ++i)//bierzemy rodzica i od pierwszego wolnego miejsca wstawiamy hetmana
        {
            for (int j = 0; j < n; ++j)
            {
                if (!s.Contains((i, j))) //czy nie ma hetmana rodzica
                {
                    var newChild = new List<(int, int)>(s) //tworzymy dziecko typu list krotek (plansza) i przekazuje rodzica do konstruktora zeby ją skopiował(konstruktor kopiujacy)
                    {
                        (i, j) //dodanie nowegoi hetmana
                    };
                    children.Add(newChild);
                }
            }
        }
        return children;
    }

    static bool HasColisions(List<(int, int)> s)
    {
        for (int i = 0; i < s.Count; i++)
        {
            for (int j = i + 1; j < s.Count; j++)
            {
                //wiersz lub kolumna
                if (s[i].Item1 == s[j].Item1 || s[i].Item2 == s[j].Item2)
                    return true;

                //ta sama przekatna
                if (Math.Abs(s[i].Item1 - s[j].Item1) == Math.Abs(s[i].Item2 - s[j].Item2))
                    return true;
            }
        }
        return false;
    }

    static bool IsLastState(List<(int, int)> s, int n) //czy to koncowy
    {
        if (s.Count !=  n) return false;
        return !HasColisions(s);
    }

    static bool IsInList(IEnumerable<List<(int, int)>> list, List<(int, int)> checkItem) //czy lista jest w zbiorze list 
    {
        return list.Any(item => item.SequenceEqual(checkItem)); //czy ktorykolwiek element listy spełnia warunek, ze ten item(plansza) jest rowna elementom naszej planszy
    }
    //bfs idzie od dołu
    static (List<(int, int)>, int, int) BFS(List<(int, int)> s0, int n) //przyjmuje stan początkowy i n, a zwraca krotke składającą się z planszy, oraz dwóch intów - open i closed
    {
        var closed = new List<List<(int, int)>>(); //plansze sprawdzone
        var open = new Queue<List<(int, int)>>(); // FIFO    plansze do sprawdzenia, kolejka
        open.Enqueue(s0); 
        while (open.Count > 0) //dopoki na kolejce open cos jest
        {
            var s = open.Dequeue(); //sciagamy z kolejki element
            if (IsLastState(s, n)) return (s, open.Count, closed.Count);//czy to ostatnia plansza, jesli tak to zwracamy liste, open i closed
            var children = GenChildren(s, n);//generujemy dzieci i to co zwroci przypisujemy do zmiennej children
            foreach (var child in children) //przechodzimy po kazdym dziecku zeby sprawdzic czy closed i open nie zawiera w sobie dziecka
            {
                if (!closed.Contains(child) && !open.Contains(child)) open.Enqueue(child); //jezeli nie ma na obu listach do dodajemy do open do sprawdzenia
            }
            closed.Add(s); //wrzucamy na closed rodzica 
        }
        return ([], 0, 0); //jakby nic nie znalazł to zwroci pusta lista (niemozliwe w algorytmie)
    }

    static (List<(int, int)>, int, int) DFS(List<(int, int)> s0, int n)
    {
        var closed = new List<List<(int, int)>>();
        var open = new Stack<List<(int, int)>>(); // LIFO stos
        open.Push(s0); //wrzucamy 1 element na stos
        while (open.Count > 0) //krazy dopoki cos jest w zbiorze open
        {
            var s = open.Pop(); //pobieramy element ze stosu
            if (IsLastState(s, n)) return (s, open.Count, closed.Count); //sprawdzamy czy to rozwiazanie, jezeli tak to zwracamy open i closed
            if (s.Count >= n)// nie ma w algorytmie, ale trzeba dać warunek, żeby wracało i nie wpadało w nieskończoną pętlę (generował dzieci w kolko). jesli jest n hetmanow nie generujemy juz
            {
                closed.Add(s);//plansza z n hetmanami nie wchodziła już w dodanie do closed wiec tutaj rekompensata
                continue;
            } 
            var children = GenChildren(s, n);
            foreach (var child in children)
            {
                if (!IsInList(closed, child) && !IsInList(open, child)) open.Push(child); //tutaj nie moglo byc contains bo zwracalo bledne wartosci
            }
            closed.Add(s);
        }
        return ([], 0, 0);
    }

    static void PrintBoard(List<(int, int)> elements, int n)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (elements.Contains((j, i))) sb.Append('x');
                else sb.Append('o');
                sb.Append(' ');
            }
            sb.Append('\n');
        }
        Console.WriteLine(sb.ToString());
    }

    static void Print(List<(int, int)> elements)
    {
        var sb = new StringBuilder();
        sb.Append("[ ");
        foreach (var element in elements)
            sb.Append($"({element.Item1}, {element.Item2}), ");
        sb.Remove(sb.Length - 2, 2);
        sb.Append(" ]");
        Console.WriteLine(sb.ToString());
    }

    static (int, int, double) RunSearch(List<(int, int)> s0, int n, Search search)
    {
        Stopwatch sw = new Stopwatch();
        List<(int, int)> result;
        int open, closed;
        sw.Start();
        switch (search)
        {
            case Search.BFS:
                (result, open, closed) = BFS(s0, n);
                break;
            case Search.DFS:
                (result, open, closed) = DFS(s0, n);
                break;
            case Search.BFSMod:
                (result, open, closed) = BFSMod(s0, n);
                break;
            case Search.BFSMod2:
                (result, open, closed) = BFSMod2(s0, n);
                break;
            case Search.DFSMod:
                (result, open, closed) = DFSMod(s0, n);
                break;
            case Search.DFSMod2:
                (result, open, closed) = DFSMod2(s0, n);
                break;
            default:
                Console.WriteLine($"Search {search} not implemented.");
                return (0, 0, 0);
        }
        sw.Stop();
        Console.WriteLine($"Search {search} for n={n}:");
        Console.WriteLine($"- Open: {open},");
        Console.WriteLine($"- Closed: {closed},");
        Console.WriteLine($"- Elapsed: {sw.Elapsed.TotalSeconds}s.");
        Print(result);
        PrintBoard(result, n);
        Console.WriteLine("\n#########################\n");
        return (open, closed, sw.Elapsed.TotalSeconds);
    }

    static void CreateChart(List<int> x, List<double> y1, List<double> y2, string yAxisTitle, string y1Label, string y2Label)
    {
        string title = $"{yAxisTitle} vs Number of Queens (n)";
        var tickVals = x.Select(i => (double)i);
        var xAxis = LinearAxis.init<double, double, double, double, double, double, double, double>(
            Title: FSharpOption<Title>.Some(Title.init(Text: "Number of Queens (n)")),
            TickMode: FSharpOption<StyleParam.TickMode>.Some(StyleParam.TickMode.Array),
            TickVals: FSharpOption<IEnumerable<double>>.Some(tickVals)
        );

        var line1 = Chart2D.Chart
            .Line<int, double, string>(x, y1)
            .WithTraceInfo(Name: FSharpOption<string>.Some(y1Label));

        var line2 = Chart2D.Chart
            .Line<int, double, string>(x, y2)
            .WithTraceInfo(Name: FSharpOption<string>.Some(y2Label));

        var combined = Plotly.NET.Chart.Combine([line1, line2])
            .WithLegend(true)
            .WithTitle(title)
            .WithXAxis(xAxis)
            .WithYAxisStyle(Title.init(Text: yAxisTitle), ShowGrid: true);

        string dir = "plots";
        Directory.CreateDirectory(dir);
        string fileName = $"{y1Label.ToLower()}_{y2Label.ToLower()}_{x.Last()}_";
        fileName += yAxisTitle.Replace(" ", "_")
            .Replace("(", "").Replace(")", "")
            .Replace("[", "").Replace("]", "")
            .ToLower();
        string fullPath = Path.GetFullPath(Path.Combine(dir, fileName));

        combined.SavePNG(fullPath, Width: 800, Height: 600);
        fullPath += ".png";

        Console.WriteLine($"Plot saved to {fullPath}");

        Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
    }

    static void CreateChart(List<int> x, List<double> y, string yAxisTitle, Search mode)
    {
        string title = $"{yAxisTitle} vs Number of Queens (n) in {mode}";
        var tickVals = x.Select(i => (double)i);
        var xAxis = LinearAxis.init<double, double, double, double, double, double, double, double>(
            Title: FSharpOption<Title>.Some(Title.init(Text: "Number of Queens (n)")),
            TickMode: FSharpOption<StyleParam.TickMode>.Some(StyleParam.TickMode.Array),
            TickVals: FSharpOption<IEnumerable<double>>.Some(tickVals)
        );

        var chart = Chart2D.Chart.Line<int, double, string>(x, y)
            .WithTitle(title)
            .WithXAxis(xAxis)
            .WithYAxisStyle(Title.init(Text: yAxisTitle), ShowGrid: true);

        string dir = "plots";
        Directory.CreateDirectory(dir);
        string fileName = $"{mode.ToString().ToLower()}_{x.Last()}_";
        fileName += yAxisTitle.Replace(" ", "_")
            .Replace("(", "").Replace(")", "")
            .Replace("[", "").Replace("]", "")
            .ToLower();
        string fullPath = Path.GetFullPath(Path.Combine(dir, fileName));

        chart.SavePNG(fullPath, Width: 800, Height: 600);
        fullPath += ".png";

        Console.WriteLine($"Plot saved to {fullPath}");

        Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
    }

    static void CompareTwoModes(List<(int, int)> s0, int min, int max, Search mode, Search mode2)
    {
        List<double> resultOpen = [];
        List<double> resultClosed = [];
        List<double> resultElapsed = [];
        List<int> resultQueens = [];
        for (int n = min; n <= max; n++)
        {
            var (open, closed, elapsed) = RunSearch(s0, n, mode);
            resultQueens.Add(n);
            resultOpen.Add(open);
            resultClosed.Add(closed);
            resultElapsed.Add(elapsed);
        }

        List<double> resultOpen2 = [];
        List<double> resultClosed2 = [];
        List<double> resultElapsed2 = [];
        List<int> resultQueens2 = [];
        for (int n = min; n <= max; n++)
        {
            var (open, closed, elapsed) = RunSearch(s0, n, mode2);
            resultQueens2.Add(n);
            resultOpen2.Add(open);
            resultClosed2.Add(closed);
            resultElapsed2.Add(elapsed);
        }

        CreateChart(resultQueens, resultElapsed, resultElapsed2, "Elapsed time [s]", mode.ToString(), mode2.ToString());
        CreateChart(resultQueens, resultOpen, resultOpen2, "Open list size", mode.ToString(), mode2.ToString());
        CreateChart(resultQueens, resultClosed, resultClosed2, "Closed list size", mode.ToString(), mode2.ToString());
    }

    static void CheckOneMode(List<(int, int)> s0, int min, int max, Search mode)
    {
        List<double> resultOpen = [];
        List<double> resultClosed = [];
        List<double> resultElapsed = [];
        List<int> resultQueens = [];
        for (int n = min; n <= max; n++)
        {
            var (open, closed, elapsed) = RunSearch(s0, n, mode);
            resultQueens.Add(n);
            resultOpen.Add(open);
            resultClosed.Add(closed);
            resultElapsed.Add(elapsed);
        }

        CreateChart(resultQueens, resultElapsed, "Elapsed time [s]", mode);
        CreateChart(resultQueens, resultOpen, "Open list size", mode);
        CreateChart(resultQueens, resultClosed, "Closed list size", mode);
    }

    static void Main(string[] args)
    {
        int min = 4;
        int max = 6;
        Search mode = Search.DFSMod;
        Search mode2 = Search.BFSMod2;
        List<(int,int)> s0 = new List<(int,int)>();

        //CompareTwoModes(s0, min, max, mode, mode2);
       CheckOneMode(s0, min, max, mode2);
    }
}