# uFrame Plugin - NodeCanvas

This is a plugin to allow NodeCanvas and uFrame to share the same variables and notify each other of changes.

So for example if you have a viewmodel with a property `Health` then you can get access to that property in NodeCanvas as a 2 way binding, so updating it in NC will notify uFrame and vice versa.

To use it you just need to copy the folder into unity and make sure you have uFrame and NodeCanvas assets, then attach the SyncBlackboardWithViewModel component onto your relevant objects.

It has been tested with uFrame 1.4 and NodeCanvas 1.5.0, it is a prototype but should be functional enough.