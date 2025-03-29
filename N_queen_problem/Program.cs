
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

enum Search
{
    BFS,
    DFS,
    BFSMod,
    BFSMod2,
    DFSMod,
    DFSMod2,
    BFSH,
    BFSHdebug
}

enum Heuristic
{
    None,
    H1,
    H2,
    HM
}

class Program //list od krotki to plansza
{
    #region Modified lab1 (poza zadaniem)
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

    #region Lab2 - BFS z heurystyką 
    static bool IsInList(IEnumerable<(List<(int, int)>, double)> list, List<(int, int)> checkItem) //czy lista jest w zbiorze list 
    {
        return list.Any(item => item.Item1.SequenceEqual(checkItem)); //czy ktorykolwiek element listy spełnia warunek, ze ten item(plansza) jest rowna elementom naszej planszy
    }
    static bool UpdateIfIsInList(PriorityQueue<List<(int, int)>, double> list, List<(int, int)> checkItem, double newH) //czy lista(plansze) jest w kolejce prirytetowej list (u nas zawsze kolejka open)
    {
        // szukamy stanu na kolejce
        for (int i = 0; i < list.Count; ++i)
        {
            if (list.UnorderedItems.ElementAt(i).Element.SequenceEqual(checkItem))
            {
                // jeśli jest na kolejce taki stan
                if (list.UnorderedItems.ElementAt(i).Priority > newH)
                {
                    // i ma większe priority niż to nowo wyliczone
                    List<(List<(int, int)> Element, double Priority)> dequeued = new List<(List<(int, int)> Element, double Priority)>(); //tworzyny pustą liste na elementy chwilowo wyjete z kolejki
                    // to przebudowujemy kolejkę, ściągając stany dopóki nie trafimy na ten szukany
                    while (list.TryDequeue(out List<(int, int)> element, out double priority))
                    {
                        
                        if (element.SequenceEqual(checkItem))//czy element ktory pobieramy to ten ktorego szukalismy(czy jest tak sam jak dziecko)
                        {
                            // szukany stan odkładamy też na listę, ale z poprawionym priority i kończymy ściąganie z kolejki
                            dequeued.Add((element, newH));
                            break;
                        }
                        else dequeued.Add((element, priority));// ściągnięte stany odkładamy na dodatkową listę
                    }
                    // wrzucamy wszystkie ściągnięte stany na kolejkę
                    foreach (var item in dequeued)
                    {
                        list.Enqueue(item.Element, item.Priority);
                    }
                }
                return true;
            }
        }
        return false;
    }
    static double H1(List<(int, int)> s, int n)
    {
        int rowsSum = 0;
        for (int i = 0; i < s.Count; ++i) //sprawdzamy po wszystkich hetmanach
        {
            int rowI = s[i].Item1 + 1; //numer wiersza(zwykły 1,2,3..) współrzędna hetmana+1
            if (rowI <= n / 2) //sprawdzamy do połowy
                rowsSum += n - rowI + 1; //numer wiersza(dziwny np. 4 3 3 4)
            else rowsSum += rowI;
        }
        return (n - s.Count) * rowsSum; // n - liczba hetmanow * suma wierszy
    }
    static int CountAttacks(List<(int, int)> s)
    {
        int attacks = 0;
        for (int i = 0; i < s.Count - 1; i++)
        {
            for (int j = i + 1; j < s.Count; j++)
            {
                //wiersz lub kolumna
                if (s[i].Item1 == s[j].Item1 || s[i].Item2 == s[j].Item2)
                    attacks++;

                //ta sama przekatna
                else if (Math.Abs(s[i].Item1 - s[j].Item1) == Math.Abs(s[i].Item2 - s[j].Item2))
                    attacks++;
            }
        }
        return attacks;
    }
    static double H2(List<(int, int)> s, int n)
    {
        return CountAttacks(s) + n - s.Count; // liczba atakow dodac wymiar odjac liczba hetmanow
    }
    static int Manhattan((int X, int Y) s1, (int X, int Y) s2) //dajemy dwoch hetmanow 
    {//faworyzujemy plansze na ktorych sa hetmani ktorzy sie bija w odleglosci 3 pol jak koń
        return Math.Abs(s2.X - s1.X) + Math.Abs(s2.Y - s1.Y);// suma wartosci bezwglednych z roznicy wspolrzednych(parami X z obu hetmanow potem Y z obu)
    }
    static double HM(List<(int, int)> s, int n)
    {
        //List<int> manhattans = new List<int>();
        double sum = 0;
        int distances = 0;
        for (int i = 0; i < s.Count - 1; ++i)
        {
            for (int j = i + 1; j < s.Count; ++j)
            {
                //manhattans.Add(Manhattan(s[i], s[j]));
                ++distances; //liczba dystansów
                sum += Manhattan(s[i], s[j]); //dystans miedzy dwoma hetmanami
            }
        }
        //if (manhattans.Count == 0) return 3;
        //return manhattans.OrderBy(item => item)
        //    .Skip((manhattans.Count - 1) / 2)
        //    .Take(2 - manhattans.Count % 2)
        //    .Average();
        if (distances == 0) return 3; //zeby nie dzielic przez 0
        return Math.Abs((sum / distances) - 3); //suma podzielic na ilosc dystansow - 3(minus 3 zeby dązyc do trójki)
    }
    static double H(List<(int, int)> s, int n, Heuristic hType)
    {
        switch (hType)
        {
            case Heuristic.H1:
                return H1(s, n); //plansza i wymiar n
            case Heuristic.H2:
                return H2(s, n);
            case Heuristic.HM:
                return HM(s, n);
            default:
                throw new NotImplementedException($"Heuristic {hType} not implemented.");
        }
    }
    static (List<(int, int)>, int, int, double) BFSH(List<(int, int)> s0, int n, Heuristic hType, bool debug = false) //przyjmuje stan początkowy i n, a zwraca krotke składającą się z planszy, oraz dwóch intów - open i closed
    {
        if (debug) Console.WriteLine($"\n######## DEBUG {Search.BFSH} {hType} {n} ##########");
        var closed = new List<(List<(int, int)>, double)>(); //plansze sprawdzone || double to heurystyka
        var open = new PriorityQueue<List<(int, int)>, double>(); // FIFO    plansze do sprawdzenia, kolejka ||najmniejszy priorytet jako 1 sciagany
        double h0 = H(s0, n, hType); //pierwsza heurystyka dla pierwszej planszy
        open.Enqueue(s0, h0); //wrzucamy na kolejke priorytetową planszę i nadajemy jej priorytet nadany przez heurystyke
        while (open.Count > 0) //dopoki na kolejce open cos jest
        {
            open.TryDequeue(out List<(int, int)> s, out double h); //sciagamy z kolejki element ||Dequeue sciaga tylko element a trydequeue sciaga i element i priorytet
            if (debug) //wypisanie stanow posrednich
            {
                Print(s, endLine: false);
                Console.WriteLine($" h: {h}");
            }
            if (IsLastState(s, n)) return (s, open.Count, closed.Count, h);//czy to ostatnia plansza, jesli tak to zwracamy liste, open i closed i heurystyke
            if (s.Count >= n)// nie ma w algorytmie, ale trzeba dać warunek, żeby wracało i nie wpadało w nieskończoną pętlę (generował dzieci w kolko). jesli jest n hetmanow nie generujemy juz dzieci
            {
                closed.Add((s, h));//plansza z n hetmanami nie wchodziła już w dodanie do closed wiec tutaj rekompensata
                continue;
            }
            var children = GenChildren(s, n);//generujemy dzieci i to co zwroci przypisujemy do zmiennej children
            foreach (var child in children) //przechodzimy po kazdym dziecku zeby sprawdzic czy closed i open nie zawiera w sobie dziecka
            {
                if (IsInList(closed, child)) continue;
                double hc = H(child, n, hType); //jesli go nie ma closed liczymy mu heurystyke
                if (!UpdateIfIsInList(open, child, hc)) // i sprawdzamy czy jest na liscie open i jaka ma tam heurystyke
                {
                    open.Enqueue(child, hc); //jesli nie ma go to go wrzuc na open z tą nową heurystyką
                }
            }
            closed.Add((s, h)); //wrzucamy na closed rodzica 
        }
        return ([], 0, 0, -1); //jakby nic nie znalazł to zwroci pusta lista
    }
    static (List<(int, int)>, int, int, double) BFSHmod(List<(int, int)> s0, int n, Heuristic hType, bool debug = false) //przyjmuje stan początkowy i n, a zwraca krotke składającą się z planszy, oraz dwóch intów - open i closed
    {
        if (debug) Console.WriteLine($"\n######## DEBUG {Search.BFSH} {hType} {n} ##########");
        var closed = new HashSet<string>(); //plansze sprawdzone
        int closedCnt = 0;
        var open = new PriorityQueue<List<(int, int)>, double>(); // FIFO    plansze do sprawdzenia, kolejka
        double h0 = H(s0, n, hType);
        open.Enqueue(s0, h0);
        while (open.Count > 0) //dopoki na kolejce open cos jest
        {
            open.TryDequeue(out List<(int, int)> s, out double h); //sciagamy z kolejki element
            if (debug)
            {
                Print(s, endLine: false);
                Console.WriteLine($" h: {h}");
            }
            if (IsLastState(s, n)) return (s, open.Count, closed.Count, h);//czy to ostatnia plansza, jesli tak to zwracamy liste, open i closed
            if (s.Count >= n)// nie ma w algorytmie, ale trzeba dać warunek, żeby wracało i nie wpadało w nieskończoną pętlę (generował dzieci w kolko). jesli jest n hetmanow nie generujemy juz dzieci
            {
                closed.Add(GetString(s));
                closedCnt++;
                continue;
            }
            var children = GenChildren(s, n);//generujemy dzieci i to co zwroci przypisujemy do zmiennej children
            foreach (var child in children) //przechodzimy po kazdym dziecku zeby sprawdzic czy closed i open nie zawiera w sobie dziecka
            {
                if (HasColisions(child) || closed.Contains(GetString(child))) continue;
                double hc = H(child, n, hType);
                if (!UpdateIfIsInList(open, child, hc))
                {
                    open.Enqueue(child, hc);
                }
            }
            closed.Add(GetString(s));
            closedCnt++;
        }
        return ([], 0, 0, -1); //jakby nic nie znalazł to zwroci pusta lista
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
        if (s.Count != n) return false;
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
                if (elements.Contains((i, j))) sb.Append('x');
                else sb.Append('o');
                sb.Append(' ');
            }
            sb.Append('\n');
        }
        Console.WriteLine(sb.ToString());
    }

    static void Print(List<(int, int)> elements, bool endLine = true)
    {
        var sb = new StringBuilder();
        sb.Append("[ ");
        foreach (var element in elements)
            sb.Append($"({element.Item1}, {element.Item2}), ");
        sb.Remove(sb.Length - 2, 2);
        sb.Append(" ]");
        if (endLine) sb.Append('\n');
        Console.Write(sb.ToString());
    }

    static (int, int, double, double) RunSearch(List<(int, int)> s0, int n, Search search, Heuristic heuristicType = Heuristic.None)
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Search {search} for n={n}:");
        Stopwatch sw = new Stopwatch();
        List<(int, int)> result;
        int open, closed;
        double heuristic = 0;
        bool showHeuristic = false;
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
            case Search.BFSH:
                (result, open, closed, heuristic) = BFSHmod(s0, n, heuristicType); //tu zAMIENIAMY
                showHeuristic = true;
                break;
            case Search.BFSHdebug:
                (result, open, closed, heuristic) = BFSHmod(s0, n, heuristicType, debug: true);//i tu tez
                showHeuristic = true;
                break;
            default:
                Console.WriteLine($"Search {search} not implemented.");
                return (0, 0, 0, 0);
        }
        sw.Stop();
        Console.WriteLine($"- Open: {open},");
        Console.WriteLine($"- Closed: {closed},");
        if (showHeuristic) Console.WriteLine($"- Heuristic ({heuristicType}): {heuristic},");
        Console.WriteLine($"- Elapsed: {sw.Elapsed.TotalSeconds}s.");
        Print(result);
        PrintBoard(result, n);
        Console.WriteLine("\n#########################\n");
        return (open, closed, sw.Elapsed.TotalSeconds, heuristic);
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

    static void CreateChart(List<int> x, List<double> y, string yAxisTitle, string label)
    {
        string title = $"{yAxisTitle} vs Number of Queens (n) in {label}";
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
        string fileName = $"{label.ToLower()}_{x.Last()}_";
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

    static void CompareTwoModes(List<(int, int)> s0, int min, int max, Search mode, Search mode2, Heuristic heuristic = Heuristic.None, Heuristic heuristic2 = Heuristic.None)
    {
        List<double> resultOpen = [];
        List<double> resultClosed = [];
        List<double> resultElapsed = [];
        List<double> resultHeuristic = [];
        List<int> resultQueens = [];
        for (int n = min; n <= max; n++)
        {
            var (open, closed, elapsed, h) = RunSearch(s0, n, mode, heuristic);
            resultQueens.Add(n);
            resultOpen.Add(open);
            resultClosed.Add(closed);
            resultElapsed.Add(elapsed);
            resultHeuristic.Add(h);
        }

        List<double> resultOpen2 = [];
        List<double> resultClosed2 = [];
        List<double> resultElapsed2 = [];
        List<double> resultHeuristic2 = [];
        List<int> resultQueens2 = [];
        for (int n = min; n <= max; n++)
        {
            var (open, closed, elapsed, h) = RunSearch(s0, n, mode2, heuristic2);
            resultQueens2.Add(n);
            resultOpen2.Add(open);
            resultClosed2.Add(closed);
            resultElapsed2.Add(elapsed);
            resultHeuristic2.Add(h);
        }

        string label1 = mode.ToString();
        string label2 = mode2.ToString();
        if (heuristic != Heuristic.None)
        {
            label1 += "-" + heuristic.ToString();
            label2 += "-" + heuristic2.ToString();
            CreateChart(resultQueens, resultHeuristic, resultHeuristic2, "Heuristic", label1, label2);
        }
        CreateChart(resultQueens, resultElapsed, resultElapsed2, "Elapsed time [s]", label1, label2);
        CreateChart(resultQueens, resultOpen, resultOpen2, "Open list size", label1, label2);
        CreateChart(resultQueens, resultClosed, resultClosed2, "Closed list size", label1, label2);
    }

    static void CheckOneMode(List<(int, int)> s0, int min, int max, Search mode, Heuristic heuristic = Heuristic.None)
    {
        List<double> resultOpen = [];
        List<double> resultClosed = [];
        List<double> resultElapsed = [];
        List<double> resultHeuristic = [];
        List<int> resultQueens = [];
        for (int n = min; n <= max; n++)
        {
            var (open, closed, elapsed, h) = RunSearch(s0, n, mode, heuristic);
            resultQueens.Add(n);
            resultOpen.Add(open);
            resultClosed.Add(closed);
            resultElapsed.Add(elapsed);
            resultHeuristic.Add(h);
        }

        string label1 = mode.ToString();
        if (heuristic != Heuristic.None)
        {
            label1 += "-" + heuristic.ToString();
            CreateChart(resultQueens, resultHeuristic, "Heuristic", label1);
        }
        CreateChart(resultQueens, resultElapsed, "Elapsed time [s]", label1);
        CreateChart(resultQueens, resultOpen, "Open list size", label1);
        CreateChart(resultQueens, resultClosed, "Closed list size", label1);
    }

    static void Main(string[] args)
    {
        int min = 4;
        int max = 5;
        Search mode = Search.BFSH;
        Search mode2 = Search.BFSH;
        Heuristic heuristic = Heuristic.HM;
        Heuristic heuristic2 = Heuristic.HM;
        List<(int, int)> s0 = new List<(int, int)>();

       // CompareTwoModes(s0, min, max, mode, mode2, heuristic, heuristic2);
        CheckOneMode(s0, min, max, mode2, heuristic2);
    }
}