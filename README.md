protobuf-net for Unity 5的工具包。

最新的protobuf-net代码使用了很多C#的新语法和特性，但是这些特性在Unity 5上都没法使用，需要做特定的修改才行。而官方提供的protobuf for unity的dll，在启用
il2cpp的时候，会因为使用了反射特性而运行失败，需要做一些修改才行。

本工程就是基于老版本的protobuf-net做的对应的修改。可以直接拿来放到你的Unity 5工程里面。

如果使用的是Unity 2017之后的版本，直接使用官方protobuf网站上的csharp库就行了。
