using System.Text.Json;

namespace WayfinderProjectAPI.Controllers
{
    public class Item
    {
        string m_strName = "";
        public string Name { get { return m_strName; } }

        public Item(string strName)
        {
            m_strName = strName;
        }
 
    }

    public class ItemCount
    {
        public string name { get;set; }

        public int count { get; set;}

    }

    public class Recipe
    {
        // string m_strName = "";
        public string name { get; set; }

        // List<ItemCount> m_itemCounts = new List<ItemCount>();
        // public List<ItemCount> Items
        // { 
        //     get { return materials;}
        //     // private set;
        // }
        public List<ItemCount> materials { get; set; }

        public Recipe()
        {
            name = "";
            materials = new List<ItemCount>();
        }
    }

    public class ItemTotals
    {
        Dictionary<string, int> m_totals = new Dictionary<string, int>();
        public Dictionary<string, int> Totals { get { return m_totals; } }

        public void Add(string strItem, int iCount)
        {
            if(m_totals.ContainsKey(strItem)){
                m_totals[strItem] += iCount;
            }
            else {
                m_totals[strItem] = iCount;
            }
        }

        public int GetCount(string strItem)
        {
            int iCount = 0;

            if(m_totals.ContainsKey(strItem)) {
                iCount = m_totals[strItem];
            }

            return iCount;
        }

        public bool HasAtLeast(string strItem, int iCount)
        {
            int iCurrentCount = GetCount(strItem);
            return iCurrentCount >= iCount;
        }

        // Takes the values in t1 and reduces the count from what's in t2
        // If t2 has values that dont exist in t1 they will get ingored instead of being a negative value
        // If t2's value is larger than t1 then it will count it as 0 instead of negative value
        // This might be better to call it a remove t2 from t1 but this looks nicer
        public static ItemTotals operator -(ItemTotals t1, ItemTotals t2)
        {
            ItemTotals results = new ItemTotals();

            foreach(KeyValuePair<string, int> total in t1.m_totals)
            {
                int iCount = total.Value - t2.GetCount(total.Key);
                if(iCount > 0) {
                    results.Add(total.Key, iCount);
                }
            }

            return results;
        }

        public static ItemTotals operator +(ItemTotals t1, ItemTotals t2)
        {
            ItemTotals results = new ItemTotals();

            foreach (KeyValuePair<string, int> total in t1.m_totals)
            {
                results.Add(total.Key, total.Value);
            }

            foreach (KeyValuePair<string, int> total in t2.m_totals)
            {
                results.Add(total.Key, total.Value);
            }

            return results;
        }

        public void Print()
        {
            Console.WriteLine("Material Totals");
            foreach(KeyValuePair<string, int> total in m_totals){
                Console.WriteLine("  {0}: {1}", total.Key, total.Value);
            }
        }

        public string Serialize() 
        {
            StringWriter sw = new StringWriter();
            List<string> list = new List<string>();
            foreach(KeyValuePair<string, int> total in m_totals)
            {
                list.Add(total.Key + ":" + total.Value);
            }

            return string.Join(",", list);
        }

        public void Deserialize(string strData)
        {
            var strArr = strData.Split(",");


            foreach(var strLine in strArr)
            {
                string[] entry = strLine.Split(":");

                if(entry.Length != 2){
                    Console.WriteLine("Error parsing {0}", strLine);
                    continue;
                }

                string strKey = entry[0];
                int iValue;
                if(!Int32.TryParse(entry[1], out iValue))
                {
                    Console.WriteLine("Error converting value to int {0}", strLine);
                    continue;
                }

                this.Add(strKey, iValue);
            }
        }
    }

    public class SynthSource
    {
        string m_strName = "";
        public string Name { get { return m_strName; } }

        List<Item> m_items;
        public List<Item> Items { get{ return m_items; } }

        List<Recipe> m_recipes;
        public List<Recipe> Recipes { get{ return m_recipes; } }

        public SynthSource(string strName, List<Item> items, List<Recipe> recipes)
        {
            m_strName = strName;
            m_items = items;
            m_recipes = recipes;
        }

        public Recipe? FindRecipe(string strRecipe)
        {
            foreach(Recipe r1 in m_recipes) 
            {
                if(r1.name == strRecipe) {
                    return r1;
                }
            }

            return null;
        }
    }

    public class UserSynthTracker
    {
        ItemTotals m_currentItems = new ItemTotals();
        public ItemTotals UsersItems { get { return m_currentItems; } }

        public HashSet<string> m_craftedRecipes = new HashSet<string>();
        public HashSet<string> CraftedRecipes { get{ return m_craftedRecipes; }  set{ m_craftedRecipes = value; }}


        public void AddItem(string strItem, int iCount)
        {
            int iCurrent = m_currentItems.GetCount(strItem);
            if(iCurrent + iCount >= 0){ 
                m_currentItems.Add(strItem, iCount);
            }
        }

        public void AddCraftedRecipe(string strRecipe)
        {
            m_craftedRecipes.Add(strRecipe);
        }

        public void RemoveItems(ItemTotals itemTotals)
        {
            m_currentItems = m_currentItems - itemTotals;
        }
    }

    public class SynthController
    {
        SynthSource m_synthSource;
        public SynthSource SynthSource { get{ return m_synthSource; }}

        public string GameName { get { return m_synthSource.Name; }}

        public void Print(string strGame)
        {
            Console.WriteLine("Game: {0}", m_synthSource.Name);

            Console.WriteLine("Materials: ({0})", m_synthSource.Items.Count());
            foreach(Item item in m_synthSource.Items)
            {
                Console.WriteLine("  " + item.Name);
            }
            
            Console.WriteLine("Recipes: ({0})", m_synthSource.Recipes.Count());
            foreach(Recipe recipe in m_synthSource.Recipes)
            {
                Console.WriteLine("  " + recipe.name);
            }
        }

        public void LoadGame(string strGame)
        {
            string strPath = Path.Combine(Environment.CurrentDirectory, @"wwwroot/data/" + strGame + "/");

            List<Item> items = new List<Item>();
            List<Recipe> recipes = new List<Recipe>();


            items = GetItemsFromFile(strPath);
            recipes = GetRecipesFromFile(strPath);

            m_synthSource = new SynthSource(strGame, items, recipes);
        }

        private List<Item> GetItemsFromFile(string strDirectory)
        {
            List<Item> items = new List<Item>();

            using var streamReader = new StreamReader(strDirectory + "Materials.json");
            var allItems = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(streamReader.ReadToEnd());
            if(allItems != null)
            { 
                List<Dictionary<string, string>> itemArr = allItems["materials"];
                foreach(Dictionary<string, string> element in itemArr) {
                    string strItem = element["name"];
                    items.Add(new Item(strItem));
                }

            }

            Console.WriteLine(streamReader.ReadToEnd());

            return items;
        }

        private List<Recipe> GetRecipesFromFile(string strDirectory) 
        {
            List<Recipe> recipes = new List<Recipe>();

            using var streamReader = new StreamReader(strDirectory + "Recipes.json");
            var recipesJson = JsonSerializer.Deserialize<Dictionary<string, List<Recipe>>>(streamReader.ReadToEnd());
            if(recipesJson != null) {
                recipes = recipesJson["recipes"];
            }


            return recipes;
        }

        public ItemTotals GetMaterialsRequredToCraft(List<Recipe> recipeList)
        {
            ItemTotals totals = new ItemTotals();


            foreach(Recipe recipe in recipeList) {
                foreach(ItemCount count in recipe.materials) {
                    totals.Add(count.name, count.count);
                }
            }

            return totals;
        }

        public ItemTotals GetTotalItemCounts()
        {
            return GetMaterialsRequredToCraft(m_synthSource.Recipes);
        }

        public List<Recipe> GetMissingRecipes(HashSet<string> craftedRecipes)
        {
            List<Recipe> missingRecipes = new List<Recipe>();

            foreach(Recipe recipe in m_synthSource.Recipes) {
                if(!craftedRecipes.Contains(recipe.name)) {
                    missingRecipes.Add(recipe);
                }
            }

            return missingRecipes;
        }


        public ItemTotals GetMissingTotals(UserSynthTracker usersData)
        {
            List<Recipe> missingRecipes = GetMissingRecipes(usersData.CraftedRecipes);

            ItemTotals results = GetMaterialsRequredToCraft(missingRecipes) - usersData.UsersItems;

            return results;
        }

        public bool Craft(UserSynthTracker userData, string strRecipe)
        {
            Recipe? recipe = m_synthSource.FindRecipe(strRecipe);
            if(recipe != null)
            {
                ItemTotals totals = new ItemTotals();
                foreach(ItemCount count in recipe.materials)
                {
                    if(userData.UsersItems.HasAtLeast(count.name, count.count))
                    {
                        totals.Add(count.name, count.count);
                    }
                    else
                    {
                        Console.WriteLine("Missing materials. Need " + count.count + " " + count.name + " have " + userData.UsersItems.GetCount(count.name));
                        return false;
                    }
                }

                userData.AddCraftedRecipe(strRecipe);
                userData.RemoveItems(totals);

            }

            return true;
        }
    }
}