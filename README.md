# RoboHash.Net

Create RoboHashes with C# and .NET

## Overview

RoboHashes are great a way to visualize something that is hard to distinguish by humans like user ids, thumbprints, public keys.
This implementation generates for any input the same RoboHash as the [original implementation][robohash-src].

## Why?

The problem I had with them is that they are generated using [phython][robohash-src] but my complete environment is based on C# and ASP.NET.
I wanted to create them using C# code and also be able to integrate them in intranet-only applications. That is the reason why
I ported the original [phython code provided by Colin Davis][robohash-src] to C#.

## Examples

For the input _test_ this library generates the following RoboHash

![test for C#](https://dl.dropboxusercontent.com/s/q6ygqgj0gowiml6/test.png)

which is the same as the RoboHash generated by [Robohash.org][robohash] (keeping fingers crossed):

![test for Robohash.org](http://robohash.org/test)

If you think that one robot on a image is to lonely, you can give him some backup and generate an armada of robots:

![armada](https://dl.dropboxusercontent.com/s/n8g9zmglcu0ulp1/test6.armada.png)

and they also come with backgrounds:

![armada with background](https://dl.dropboxusercontent.com/s/hwqpwgvnt519obe/test6.armada.bg.png)

## RoboHash Images

The RoboHash images are copied from the [original robohash repository][robohash-src] and are available under the CC-BY-3.0 license.
* Set 1 artwork created by Zikri Kader
* Set 2 artwork created by Hrvoje Novakovic.
* Set 3 artwork created by Julian Peter Arias.

## TODO

* integrate this code in an ASP.NET web service that behaves the same as RoboHash.org
* build a nuget package

## See Also

[RoboHash Webpage][robohash]

[robohash]: http://robohash.org/
[robohash-src]: https://github.com/e1ven/RoboHash

