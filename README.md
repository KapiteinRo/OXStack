OXStack
=======

A library for basic SQLLite functions and to aid object relational mapping and basic configuration.

## Purpose
Rapid development of simple/fast business applications utilizing both a configuration XML and SQLLite Databases.

### Example
Let's assume we have a table *beers* containing the fields *brand* and *alc_perc*.
We can fetch these using:
```C#
public class Beer : BaseRTEntity
{
  public string Brand { get; set; }
  [RTDataCol("alc_perc")]
  public double Alcohol {get; set; }

  public Beer(DataRow dr, DataConnector dc) : base(dr, dc) { }
}

public class Beers : BaseRT<Beer>
{
  public Beers(DataConnector dc) : base(dc)
  {
    this.AddRange(Fill<Beer>("SELECT * FROM beers ORDER BY brand;"));
  }
}

````

## History
I've been developing this for over 6 years now. Forks of this library also contains connectors for MySQL and SQL Server and elaborate switches and replication functionality.
I'll add these to this project in a later stage.

### Requirements
SQLLite v3 or later. Haven't tested it on Mono yet.

## License
Copyright (c) 2014 Royi Eltink
License: MIT

