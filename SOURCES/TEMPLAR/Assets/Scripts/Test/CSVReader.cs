namespace Templar
{
    public class CSVReader
    {
        // outputs the content of a 2D array, useful for checking the importer
        public static void DebugOutputGrid(string[,] grid)
        {
            string textOutput = "";
            
            for (int y = 0; y < grid.GetUpperBound(1); y++)
            {	
                for (int x = 0; x < grid.GetUpperBound(0); x++)
                {
                    textOutput += grid[x,y]; 
                    textOutput += "|"; 
                }
                
                textOutput += "\n"; 
            }
            
            UnityEngine.Debug.Log(textOutput);
        }
     
        public static string[,] SplitCSVGrid(string csvText)
        {
            string[] lines = csvText.Split("\n"[0]);
     
            // Finds the max width of row.
            int width = 0; 
            for (int i = 0; i < lines.Length; i++)
            {
                string[] row = SplitCSVLine(lines[i]);
                if (row.Length > width)
                    width = row.Length;
            }
     
            // Creates new 2D string array to output to.
            string[,] outputGrid = new string[width, lines.Length]; 
            for (int y = 0; y < lines.Length; y++)
            {
                string[] row = SplitCSVLine(lines[y]); 
                for (int x = 0; x < row.Length; x++) 
                    outputGrid[x,y] = row[x]; 
            }

            return outputGrid;
        }
     
        public static string[] SplitCSVLine(string line)
        {
            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
            string pattern = @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)";
            
            foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(line, pattern, System.Text.RegularExpressions.RegexOptions.ExplicitCapture))
                list.Add(match.Groups[1].Value);

            return list.ToArray();
        }
    }
}