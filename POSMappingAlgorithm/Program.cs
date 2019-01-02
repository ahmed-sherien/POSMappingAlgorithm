using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public static int CallCount = 0;
    public static void Main()
    {
        var items = new List<Item>
        {
            new Item(1, "Item1", new List<Popup>
                     {
                         new Popup(0, new List<int>{2,3}),
                         new Popup(1, new List<int>{4,7}),
                         new Popup(1, new List<int>{6}),
                     }),
            new Item(2, "Item2", new List<Popup>
                     {
                         new Popup(0, new List<int>{5}),
                     }),
            new Item(3, "Item3", new List<Popup>
                     {
                         new Popup(1, new List<int>{8})
                     }),
            new Item(4, "Item4", new List<Popup>()),
            new Item(5, "Item5", new List<Popup>()),
            new Item(6, "Item6", new List<Popup>()),
            new Item(7, "Item7", new List<Popup>()),
            new Item(8, "Item8", new List<Popup>()),
        };
        var result = Map(items.First(), items);
        PrintList(result);
    }

    public static List<List<Item>> Map(Item item, List<Item> items)
    {
        var local = CallCount++;
        Console.WriteLine($"-------Map Call-[{local}]-Start-----");
        var result = new List<List<Item>> { new List<Item> { item } };
        PrintList(result);
        item.Popups.ForEach(popup =>
        {
            if (popup.Min < 1)
            {
                var visited = new List<int>();
                popup.ModIds.ForEach(id =>
                {
                    var localItem = items.Find(i => i.Id == id);
                    var localItemList = Map(localItem, items);
                    localItemList.Add(new List<Item>());
                    var excludedVisited = result.Where(l => !l.Any(i => visited.Contains(i.Id))).ToList();
                    var remainingNodes = result.Where(l => l.Any(i => visited.Contains(i.Id))).ToList();
                    result = Product(excludedVisited, localItemList).Union(remainingNodes).ToList();
                    visited.Add(id);
                });
            }
            else
            {
                var visited = new List<int>();
                var localResult = new List<List<Item>>();
                popup.ModIds.ForEach(id =>
                {
                    var localItem = items.Find(i => i.Id == id);
                    var localItemList = Map(localItem, items);
                    var excludedVisited = result.Where(l => !l.Any(i => visited.Contains(i.Id))).ToList();
                    var remainingNodes = result.Where(l => l.Any(i => visited.Contains(i.Id))).ToList();
                    localResult.AddRange(Product(excludedVisited, localItemList).Union(remainingNodes).ToList());
                    visited.Add(id);
                });
                result = localResult;
            }
        });
        PrintList(result);
        Console.WriteLine($"-------Map Call-[{local}]-End-----");
        return result;
    }

    private static List<List<Item>> Product(List<List<Item>> first, List<List<Item>> second)
    {
        Console.WriteLine("------Product Start-----");
        Console.Write("First:");
        PrintList(first);
        Console.Write("Second:");
        PrintList(second);
        var crossJoin = from f in first
                        from s in second
                        select f.Union(s).ToList();

        Console.WriteLine("Product:");
        PrintList(crossJoin.ToList());
        Console.WriteLine("------Product End-----");
        return crossJoin.ToList();
    }

    private static void PrintList(List<List<Item>> list)
    {
        list.ForEach(l =>
        {
            Console.WriteLine(l.Aggregate<Item, string, string>("", (name, item) => $"{name}>{item.Name}", (a) => a));
        });
    }
}

public class Item
{
    public Item(int id, string name, List<Popup> popups)
    {
        Id = id;
        Name = name;
        Popups = popups;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Popup> Popups { get; set; }
    public override string ToString()
    {
        return Name;
    }
}

public class Popup
{
    public Popup(int min, List<int> modIds)
    {
        Min = min;
        ModIds = modIds;
    }
    public int Min { get; set; }
    public List<int> ModIds { get; set; }
}