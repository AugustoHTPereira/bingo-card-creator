public class Card
{
    public int[] B { get; set; }
    public int[] I { get; set; }
    public int[] N { get; set; }
    public int[] G { get; set; }
    public int[] O { get; set; }
    public string Code { get; set; }

    public int[] All => B.Concat(I).Concat(N).Concat(G).Concat(O).OrderBy(x => x).ToArray();

    public Card(string? code = null)
    {
        B = GenerateColumn((01, 15), 5);
        I = GenerateColumn((16, 30), 5);
        N = GenerateColumn((31, 45), 4);
        G = GenerateColumn((46, 60), 5);
        O = GenerateColumn((61, 75), 5);
        Code = code ?? Guid.NewGuid().ToString();
    }

    private int[] GenerateColumn((int min, int max) range, int count)
    {
        IList<int> numbers = new List<int>(count);
        int number = 0;

        var random = new Random();

        do
        {
            number = random.Next(range.min, range.max);
            if (!numbers.Any(x => x == number))
                numbers.Add(number);
        } while (numbers.Any(x => x == number) && numbers.Count != count);

        return numbers.OrderBy(x => x).ToArray();
    }

    public static IEnumerable<Card> GenerateCards(int count)
    {
        IList<Card> cards = new List<Card>();
        for (int index = 0; index < count; index++)
        {
            var card = new Card((index + 1).ToString().PadLeft(6, '0'));
            cards.Add(card);
        }

        return cards;
    }

    // public Table ToTable()
    // {
    //     var table = new Table(UnitValue.CreatePercentArray(5))
    //         .UseAllAvailableWidth();

    //     table.AddHeaderCell("B")
    //          .AddHeaderCell("I")
    //          .AddHeaderCell("N")
    //          .AddHeaderCell("G")
    //          .AddHeaderCell("O");

    //     if (N.Length == 5)
    //     {
    //         table.AddCell(B[0].ToString())
    //              .AddCell(I[0].ToString())
    //              .AddCell(N[0].ToString())
    //              .AddCell(G[0].ToString())
    //              .AddCell(O[0].ToString())

    //              .AddCell(B[1].ToString())
    //              .AddCell(I[1].ToString())
    //              .AddCell(N[1].ToString())
    //              .AddCell(G[1].ToString())
    //              .AddCell(O[1].ToString())

    //              .AddCell(B[2].ToString())
    //              .AddCell(I[2].ToString())
    //              .AddCell(N[2].ToString())
    //              .AddCell(G[2].ToString())
    //              .AddCell(O[2].ToString())

    //              .AddCell(B[3].ToString())
    //              .AddCell(I[3].ToString())
    //              .AddCell(N[3].ToString())
    //              .AddCell(G[3].ToString())
    //              .AddCell(O[3].ToString())

    //              .AddCell(B[4].ToString())
    //              .AddCell(I[4].ToString())
    //              .AddCell(N[4].ToString())
    //              .AddCell(G[4].ToString())
    //              .AddCell(O[4].ToString());
    //     }
    //     else
    //     {
    //         table.AddCell(B[0].ToString())
    //              .AddCell(I[0].ToString())
    //              .AddCell(N[0].ToString())
    //              .AddCell(G[0].ToString())
    //              .AddCell(O[0].ToString())

    //              .AddCell(B[1].ToString())
    //              .AddCell(I[1].ToString())
    //              .AddCell(N[1].ToString())
    //              .AddCell(G[1].ToString())
    //              .AddCell(O[1].ToString())

    //              .AddCell(B[2].ToString())
    //              .AddCell(I[2].ToString())
    //              .AddCell("")
    //              .AddCell(G[2].ToString())
    //              .AddCell(O[2].ToString())

    //              .AddCell(B[3].ToString())
    //              .AddCell(I[3].ToString())
    //              .AddCell(N[2].ToString())
    //              .AddCell(G[3].ToString())
    //              .AddCell(O[3].ToString())

    //              .AddCell(B[4].ToString())
    //              .AddCell(I[4].ToString())
    //              .AddCell(N[3].ToString())
    //              .AddCell(G[4].ToString())
    //              .AddCell(O[4].ToString());
    //     }

    //     table.SetTextAlignment(TextAlignment.CENTER);

    //     return table;
    // }


}