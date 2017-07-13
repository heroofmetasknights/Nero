using System.Collections.Generic;

namespace Nero
{
    public class RootObject
{
    public int difficulty { get; set; }
    public int size { get; set; }
    public int kill { get; set; }
    public string name { get; set; }
    public List<Nero.Spec> specs { get; set; }
    public bool variable { get; set; }
    public int partition { get; set; }
}
}