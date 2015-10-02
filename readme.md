# uFrame Plugin - NodeCanvas

## Blurb
This is a plugin to allow NodeCanvas and uFrame to share the same variables and notify each other of changes.

So for example if you have a viewmodel with a property `Health` then you can get access to that property in NodeCanvas as a 2 way binding, so updating it in NC will notify uFrame and vice versa.

## Usage
To use it you just need to copy the folder into unity and make sure you have uFrame and NodeCanvas assets, then attach the SyncBlackboardWithViewModel component onto your relevant objects and refresh bindings.

It has been tested with uFrame 1.6.2 and NodeCanvas 2.3.8.

## Note
Personally I tend to make high level actions which use the view internally rather than doing such granular actions as this would enable, but I know some people prefer to just access the variables on the VM as if they were blackboard vars. So this is more for people who dont want to make their own custom actions but still want to be able to use the vars uFrame offers.

Also this was done as a proof of concept, it seems to work but I have not performance tested it, nor tested it on other platforms, so if you get any issues post them up.