
//czas wykonania, stan open i closed(ile tam elementów sie znajduje). Jeżeli ze wskaźnikami to podać w jakiej kolejności wstawiani byli hetmani
//mogą być wyniki pośrednie wypisane. moze sie to znalezc w karcie samooceny
//minimum to 4, max może być 4 i 5 ale moze tez byc duzo wieksze
class Hetman
{
    public int X { get; set; }
    public int Y { get; set; }
    public Hetman(int x, int y)
    {
        X = x;
        Y = y;
    }
}

class Program
{
    static bool CzyHetmaniSieAtakuja(List<Hetman> hetmani)
    {
        for (int i = 0; i < hetmani.Count; i++)
        {
            for (int j = i + 1; j < hetmani.Count; j++)
            {
                //wiersz lub kolumna
                if (hetmani[i].X == hetmani[j].X || hetmani[i].Y == hetmani[j].Y)
                    return true;

                //ta sama przekatna
                if (Math.Abs(hetmani[i].X - hetmani[j].X) == Math.Abs(hetmani[i].Y - hetmani[j].Y))
                    return true;
            }
        }

        return false;
    }

    static void Main(string[] args)
    {
        var hetmani = new List<Hetman>
        {
            new Hetman(2,0),
            new Hetman(1,1),
            new Hetman(3,3),
            new Hetman(0,1),
            new Hetman(0,3),
            new Hetman(1,3)
        };

        if (CzyHetmaniSieAtakuja(hetmani))
            Console.WriteLine("Hetmani się atakują.");
        else
            Console.WriteLine("Hetmani się nie atakują.");
    }
}