using System.Collections.Generic;

namespace Nero
{
    public class Spec
{
    public string @class { get; set; }
    public string spec { get; set; }
    public bool combined { get; set; }
    public List<Nero.DatumFight> data { get; set; }
    public double best_persecondamount { get; set; }
    public int best_duration { get; set; }
    public double best_historical_percent { get; set; }
    public double best_allstar_points { get; set; }
    public double best_combined_allstar_points { get; set; }
    public double possible_allstar_points { get; set; }
    public double historical_total { get; set; }
    public double historical_median { get; set; }
    public double historical_avg { get; set; }
}
}