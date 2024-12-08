namespace Trace.Models.Logic
{
    public class Delta
    {
        public string Operation { get; set; } // e.g., "insert", "delete", "format"
        public int Index { get; set; }       // The position of the operation
        public string Text { get; set; }     // The text involved in the operation
        public Dictionary<string, string> Attributes { get; set; } // Formatting attributes (e.g., bold, italic)
    }
}
