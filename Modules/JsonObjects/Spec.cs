using System.Collections.Generic;

namespace Nero
{
    public class Spec
{
    public string @class { get; set; }
    public string spec { get; set; }
    public bool combined { get; set; }
    public List<Nero.Datum> data { get; set; }
    public double best_persecondamount { get; set; }
    public int best_duration { get; set; }
    public int best_historical_percent { get; set; }
    public double best_allstar_points { get; set; }
    public int best_combined_allstar_points { get; set; }
    public int possible_allstar_points { get; set; }
    public int historical_total { get; set; }
    public int historical_median { get; set; }
    public double historical_avg { get; set; }
}
}